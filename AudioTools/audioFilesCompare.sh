#!/bin/bash

# Audio PCM Comparison Script
# Usage: ./audioFilesCompare.sh [--normalize] file1 file2

getAudioInfo()
{
    local file="$1"
    echo "=== Audio Info for: $file ==="

    # Get detailed audio information
    ffprobe -v quiet -select_streams a:0 -show_entries stream=sample_rate,channels,bits_per_sample,codec_name,bit_rate -of csv=p=0 "$file" 2>/dev/null | while IFS=',' read -r sample_rate channels bits_per_sample codec_name bit_rate; do
        echo "  Codec: $codec_name"
        echo "  Sample Rate: ${sample_rate} Hz"
        echo "  Channels: $channels"
        echo "  Bits per Sample: $bits_per_sample"
        echo "  Bit Rate: $bit_rate bps"
    done
    echo ""
}

showHelp()
{
	echo "Audio PCM Comparison Script"
	echo ""
	echo "Usage: $0 [command] [--normalize] file1 file2"
	echo ""
	echo "Command:"
	echo "audio       Compares the raw audio (PCM) data."
	echo "metadata    Compares the metadata within the file."
	echo "Options:"
	echo "  --normalize    Normalize both files to same format before comparison"
	echo "                 (44.1kHz, 16-bit, stereo)"
	echo "  --help         Show this help message"
	echo ""
	echo "Without --normalize, compares files in their native formats"
	echo "and shows technical differences."
}

compare_normalized() {
    local file1="$1"
    local file2="$2"

    echo "=== NORMALIZED COMPARISON ==="
    echo "Converting both files to: 44.1kHz, 16-bit, stereo PCM"
    echo ""

    # Generate normalized PCM hashes
    echo "Generating PCM hash for file 1..."
    hash1=$(ffmpeg -i "$file1" -f s16le -acodec pcm_s16le -ar 44100 -ac 2 - 2>/dev/null | sha256sum | cut -d' ' -f1)

    echo "Generating PCM hash for file 2..."
    hash2=$(ffmpeg -i "$file2" -f s16le -acodec pcm_s16le -ar 44100 -ac 2 - 2>/dev/null | sha256sum | cut -d' ' -f1)

    echo ""
    echo "File 1 PCM Hash: $hash1"
    echo "File 2 PCM Hash: $hash2"
    echo ""

    if [ "$hash1" = "$hash2" ]; then
        echo "✓ RESULT: Files contain the SAME audio content (when normalized)"
    else
        echo "✗ RESULT: Files contain DIFFERENT audio content"
    fi
}

compare_native() {
    local file1="$1"
    local file2="$2"

    echo "=== NATIVE FORMAT COMPARISON ==="
    echo "Comparing files in their original formats..."
    echo ""

    # Get native format info
    info1=$(ffprobe -v quiet -select_streams a:0 -show_entries stream=sample_rate,channels,bits_per_sample -of csv=p=0 "$file1" 2>/dev/null)
    info2=$(ffprobe -v quiet -select_streams a:0 -show_entries stream=sample_rate,channels,bits_per_sample -of csv=p=0 "$file2" 2>/dev/null)

    IFS=',' read -r sr1 ch1 bits1 <<< "$info1"
    IFS=',' read -r sr2 ch2 bits2 <<< "$info2"

    # Extract to native PCM format for each file
    echo "Extracting PCM from file 1 (${sr1}Hz, ${ch1}ch, ${bits1}bit)..."
    temp1=$(mktemp --suffix=.pcm)
    ffmpeg -i "$file1" -f s${bits1:-16}le -acodec pcm_s${bits1:-16}le -ar "$sr1" -ac "$ch1" "$temp1" 2>/dev/null

    echo "Extracting PCM from file 2 (${sr2}Hz, ${ch2}ch, ${bits2}bit)..."
    temp2=$(mktemp --suffix=.pcm)
    ffmpeg -i "$file2" -f s${bits2:-16}le -acodec pcm_s${bits2:-16}le -ar "$sr2" -ac "$ch2" "$temp2" 2>/dev/null

    # Compare file sizes and hashes
    size1=$(stat -f%z "$temp1" 2>/dev/null || stat -c%s "$temp1" 2>/dev/null)
    size2=$(stat -f%z "$temp2" 2>/dev/null || stat -c%s "$temp2" 2>/dev/null)
    hash1=$(sha256sum "$temp1" | cut -d' ' -f1)
    hash2=$(sha256sum "$temp2" | cut -d' ' -f1)

    echo ""
    echo "File 1 PCM: $size1 bytes, Hash: $hash1"
    echo "File 2 PCM: $size2 bytes, Hash: $hash2"
    echo ""

    # Analysis
    if [ "$hash1" = "$hash2" ]; then
        echo "✓ RESULT: Files are IDENTICAL at the PCM level"
    else
        echo "✗ RESULT: Files are DIFFERENT"
        echo ""
        echo "TECHNICAL DIFFERENCES:"

        if [ "$sr1" != "$sr2" ]; then
            echo "  • Sample Rate: $sr1 Hz vs $sr2 Hz"
            if [ "$sr1" -gt "$sr2" ]; then
                echo "    → File 1 has higher quality (keep this one)"
            else
                echo "    → File 2 has higher quality (keep this one)"
            fi
        fi

        if [ "$ch1" != "$ch2" ]; then
            echo "  • Channels: $ch1 vs $ch2"
        fi

        if [ "$bits1" != "$bits2" ]; then
            echo "  • Bit Depth: ${bits1:-16} vs ${bits2:-16}"
            if [ "${bits1:-16}" -gt "${bits2:-16}" ]; then
                echo "    → File 1 has higher quality (keep this one)"
            else
                echo "    → File 2 has higher quality (keep this one)"
            fi
        fi

        if [ "$size1" != "$size2" ]; then
            echo "  • File Size: $size1 bytes vs $size2 bytes"
        fi

        # If formats are the same but hashes differ, it's content difference
        if [ "$sr1" = "$sr2" ] && [ "$ch1" = "$ch2" ] && [ "${bits1:-16}" = "${bits2:-16}" ]; then
            echo "  • Same technical specs but different audio content"
        fi
    fi

    # Cleanup
    rm -f "$temp1" "$temp2"
}

# Main script logic
if [ $# -eq 0 ] || [ "$1" = "--help" ]; then
    showHelp
    exit 0
fi

normalize=false
if [ "$2" = "--normalize" ]; then
    normalize=true
    shift
fi

if [ $# -ne 3 ]; then
    echo "Error: Please provide exactly 2 audio files to compare."
    echo "Use --help for usage information."
    exit 1
fi

file1="$2"
file2="$3"

# Check if files exist
if [ ! -f "$file1" ]; then
    echo "Error: File '$file1' not found."
    exit 1
fi

if [ ! -f "$file2" ]; then
    echo "Error: File '$file2' not found."
    exit 1
fi

# Check if ffmpeg and ffprobe are available
if ! command -v ffmpeg >/dev/null 2>&1; then
    echo "Error: ffmpeg is not installed or not in PATH."
    exit 1
fi

if ! command -v ffprobe >/dev/null 2>&1; then
    echo "Error: ffprobe is not installed or not in PATH."
    exit 1
fi

echo "Comparing audio files:"
echo "  File 1: $file1"
echo "  File 2: $file2"
echo ""

# Show technical information for both files
getAudioInfo "$file1"
getAudioInfo "$file2"

# Perform comparison based on mode
if [ "$normalize" = true ]; then
    compare_normalized "$file1" "$file2"
else
    compare_native "$file1" "$file2"
fi

echo ""
echo "Comparison complete."
