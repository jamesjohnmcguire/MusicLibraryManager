#!/usr/bin/env python3
"""
Audio Conversion Script
Created: 2025-02-06
Author: James John McGuire
Description: Batch converts WMA files to modern formats while preserving metadata
"""

import argparse
import logging
import os
import subprocess
import sys
from datetime import datetime
from pathlib import Path

def setup_logging(log_file="audio_conversion.log"):
    """Setup logging configuration"""
    logging.basicConfig(
        level=logging.INFO,
        format='%(asctime)s - %(levelname)s - %(message)s',
        handlers=[
            logging.FileHandler(log_file),
            logging.StreamHandler(sys.stdout)
        ]
    )
    return logging.getLogger(__name__)

def check_ffmpeg():
    """Check if FFmpeg is installed"""
    try:
        subprocess.run(['ffmpeg', '-version'], capture_output=True, check=True)
        return True
    except (subprocess.SubprocessError, FileNotFoundError):
        return False

def get_audio_info(file_path):
    """Get audio file information using FFmpeg"""
    try:
        result = subprocess.run(
            ['ffprobe', '-v', 'error', '-show_entries', 
             'stream=codec_name,bit_rate,sample_rate', '-of', 'default=noprint_wrappers=1',
             file_path],
            capture_output=True,
            text=True,
            check=True
        )
        return result.stdout
    except subprocess.SubprocessError as e:
        logging.error(f"Error getting audio info for {file_path}: {e}")
        return None

def convert_audio(input_file, output_format='m4a', quality='high'):
    """Convert audio file to specified format"""
    input_path = Path(input_file)
    output_path = input_path.with_suffix(f'.{output_format}')
    
    # Skip if output file exists
    if output_path.exists():
        logging.warning(f"Skipping {input_file} - output file already exists")
        return False
    
    # Quality settings
    quality_settings = {
        'm4a': {
            'high': ['-c:a', 'aac', '-b:a', '256k'],
            'medium': ['-c:a', 'aac', '-b:a', '192k'],
            'low': ['-c:a', 'aac', '-b:a', '128k']
        },
        'flac': {
            'high': ['-c:a', 'flac', '-compression_level', '8'],
            'medium': ['-c:a', 'flac', '-compression_level', '5'],
            'low': ['-c:a', 'flac', '-compression_level', '3']
        },
        'mp3': {
            'high': ['-c:a', 'libmp3lame', '-b:a', '320k'],
            'medium': ['-c:a', 'libmp3lame', '-b:a', '192k'],
            'low': ['-c:a', 'libmp3lame', '-b:a', '128k']
        }
    }

    try:
        cmd = ['ffmpeg', '-i', str(input_path), '-map_metadata', '0']
        cmd.extend(quality_settings[output_format][quality])
        cmd.append(str(output_path))
        
        logging.info(f"Converting: {input_file}")
        subprocess.run(cmd, check=True, capture_output=True)
        return True
    except subprocess.SubprocessError as e:
        logging.error(f"Error converting {input_file}: {e}")
        return False

def main():
    parser = argparse.ArgumentParser(description='Batch convert audio files to modern formats')
    parser.add_argument('input_dir', help='Input directory containing WMA files')
    parser.add_argument('--format', choices=['m4a', 'flac', 'mp3'], default='m4a',
                        help='Output format (default: m4a)')
    parser.add_argument('--quality', choices=['high', 'medium', 'low'], default='high',
                        help='Output quality (default: high)')
    parser.add_argument('--recursive', action='store_true',
                        help='Search for files recursively')
    args = parser.parse_args()

    # Setup logging
    logger = setup_logging()
    logger.info(f"Starting conversion process at {datetime.utcnow()}")

    # Check FFmpeg installation
    if not check_ffmpeg():
        logger.error("FFmpeg is not installed. Please install FFmpeg to use this script.")
        sys.exit(1)

    # Get list of WMA files
    input_path = Path(args.input_dir)
    if args.recursive:
        wma_files = list(input_path.rglob("*.wma"))
    else:
        wma_files = list(input_path.glob("*.wma"))

    if not wma_files:
        logger.error(f"No WMA files found in {args.input_dir}")
        sys.exit(1)

    # Process files
    success_count = 0
    total_files = len(wma_files)
    logger.info(f"Found {total_files} WMA files to process")

    for file in wma_files:
        if convert_audio(file, args.format, args.quality):
            success_count += 1
        logger.info(f"Progress: {success_count}/{total_files}")

    # Summary
    logger.info(f"Conversion completed: {success_count}/{total_files} files converted successfully")
    logger.info(f"Process completed at {datetime.utcnow()}")

if __name__ == "__main__":
    main()
