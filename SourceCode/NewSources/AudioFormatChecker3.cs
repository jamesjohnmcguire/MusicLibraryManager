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
            case ".aac":
            case ".ogg":
            case ".wma": // Standard WMA codec
                return AudioType.Lossy;

            case ".flac":
            case ".alac":
            case ".wav":
            case ".aiff":
            case ".ape":
            case ".m4a": // ALAC in M4A container
                return CheckLosslessInContainer(filePath);

            default:
                return AudioType.Unknown;
        }
    }

    private static AudioType CheckLosslessInContainer(string filePath)
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
                    WaveFormatEncoding.WmaLossless => AudioType.Lossless,
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