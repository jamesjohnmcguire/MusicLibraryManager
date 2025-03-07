using System;
using System.IO;
using NAudio.Wave;

public class AudioFormatChecker
{
    public enum AudioType
    {
        Lossy,
        Lossless,
        Unknown
    }

    public static AudioType GetAudioType(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found: " + filePath);
        }

        string extension = Path.GetExtension(filePath).ToLower();
        switch (extension)
        {
            case ".mp3":
                return AudioType.Lossy;
            case ".aac":
            case ".ogg":
                return AudioType.Lossy;

            case ".flac":
            case ".alac":
            case ".wav":
            case ".aiff":
            case ".ape":
                return AudioType.Lossless;

            case ".wma":
                return CheckWmaAudioType(filePath);

            case ".m4a": 
                return CheckM4aAudioType(filePath);

            default:
                return AudioType.Unknown;
        }
    }

    private static AudioType CheckWmaAudioType(string filePath)
    {
        try
        {
            using (var reader = new MediaFoundationReader(filePath))
            {
                var format = reader.WaveFormat;
                return format.Encoding switch
                {
                    WaveFormatEncoding.WmaLossless => AudioType.Lossless,
                    WaveFormatEncoding.Wmaudio2 => AudioType.Lossy, // WMA Standard
                    WaveFormatEncoding.Wmaudio3 => AudioType.Lossy, // WMA Professional
                    _ => AudioType.Unknown,
                };
            }
        }
        catch (Exception)
        {
            return AudioType.Unknown;
        }
    }

    private static AudioType CheckM4aAudioType(string filePath)
    {
        try
        {
            using (var reader = new AudioFileReader(filePath))
            {
                var format = reader.WaveFormat;
                return format.Encoding switch
                {
                    WaveFormatEncoding.Pcm => AudioType.Lossless,
                    WaveFormatEncoding.Adpcm => AudioType.Lossless,
                    WaveFormatEncoding.IeeeFloat => AudioType.Lossless,
                    _ => AudioType.Lossy,
                };
            }
        }
        catch (Exception)
        {
            return AudioType.Unknown;
        }
    }
}