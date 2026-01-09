/////////////////////////////////////////////////////////////////////////////
// <copyright file="MediaFileFormat.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace MediaFileToolkit;

using System.IO;
using NAudio.Wave;

public class MediaFileFormat
{

	public static AudioType GetAudioType(string filePath)
	{
		AudioType audioType = AudioType.Unknown;

		if (!File.Exists(filePath))
		{
			throw new FileNotFoundException("File not found: " + filePath);
		}

		string extension = Path.GetExtension(filePath).ToLower();

		switch (extension)
		{
			// Only lossy formats
			case ".aac":
			case ".mp3":
			case ".opus":
				audioType = AudioType.Lossy;
				break;

			// Only lossless formats
			case ".aiff": // Rare edge case: AIFF-C can contain lossy
			case ".ape":
			case ".flac":
			case ".tta":
			case ".wav":
				audioType = AudioType.Lossless;
				break;

			// Both lossy and lossless formats
			case ".mka":
			case ".ogg":
			case ".wv": // WavPack
				// TODO
				break;

			case ".m4a":
#if WINDOWS
				audioType = GetAudioTypeM4aNaudio(filePath);
#else
				audioType = GetAudioTypeM4aTagLib(filePath);
#endif
				break;

			case ".wma":
#if WINDOWS
				audioType = GetAudioTypeWmaNaudio(filePath);
#else
				audioType = GetAudioTypeM4aTagLib(filePath);
#endif
				break;

			default:
				audioType = AudioType.Unknown;
				break;
		}

		return audioType;
	}

	private static AudioType GetAudioTypeM4aNaudio(string filePath)
	{
		AudioType audioType = AudioType.Unknown;

		using MediaFoundationReader reader = new(filePath);

		WaveFormat format = reader.WaveFormat;

		audioType = format.Encoding switch
		{
			WaveFormatEncoding.Pcm => AudioType.Lossless,
			WaveFormatEncoding.Adpcm => AudioType.Lossless,
			WaveFormatEncoding.IeeeFloat => AudioType.Lossless,
			_ => AudioType.Lossy,
		};

		return audioType;
	}

	private static AudioType GetAudioTypeM4aTagLib(string filePath)
	{
		AudioType audioType = AudioType.Unknown;

		using TagLib.File file = TagLib.File.Create(filePath);

		if (file.Properties.Codecs != null)
		{
			foreach (TagLib.ICodec? codec in file.Properties.Codecs)
			{
				if (codec is TagLib.Mpeg4.IsoAudioSampleEntry audioEntry)
				{
					if (audioEntry.BoxType == "alac")
					{
						audioType = AudioType.Lossless;
					}

					if (audioEntry.BoxType == "mp4a")
					{
						string description = codec.Description;
						description = description?.ToLower() ?? string.Empty;

						// ALAC sometimes reports as mp4a with ALAC description
						if (description.Contains("alac") ||
							description.Contains("apple lossless"))
						{
							audioType = AudioType.Lossless;
						}
						else if (description.Contains("aac") ||
							description.Contains("mpeg-4"))
						{
							// Most mp4a entries are AAC variants (lossy)
							// AAC-LC, AAC-HE, AAC-HEv2, etc.
							audioType = AudioType.Lossy;
						}
						else
						{
							// Add additional checks here for other codecs.
							// If we can't determine specifically,
							// mp4a is typically lossy
							audioType = AudioType.Lossy;
						}

						if (audioType != AudioType.Unknown)
						{
							break;
						}
					}
				}
			}
		}

		return audioType;
	}

	private static AudioType GetAudioTypeWmaNaudio(string filePath)
	{
		AudioType audioType = AudioType.Unknown;

		using MediaFoundationReader reader = new(filePath);

		WaveFormat format = reader.WaveFormat;

		audioType = format.Encoding switch
		{
			WaveFormatEncoding.WindowsMediaAudioLosseless => AudioType.Lossless,
			// WMA Standard
			WaveFormatEncoding.WindowsMediaAudio => AudioType.Lossy,
			WaveFormatEncoding.WindowsMediaAudioProfessional => AudioType.Lossy,
			WaveFormatEncoding.WindowsMediaAudioSpdif => AudioType.Lossy,
			_ => AudioType.Unknown,
		};

		return audioType;
	}

	private static AudioType GetAudioTypeWmaTagLib(string filePath)
	{
		AudioType audioType = AudioType.Unknown;

		using TagLib.File file = TagLib.File.Create(filePath);

		if (file.Properties.Codecs != null)
		{
			foreach (TagLib.ICodec? codec in file.Properties.Codecs)
			{
				string description = codec.Description;
				description = description?.ToLower() ?? string.Empty;

				if (description.Contains("lossless"))
				{
					// WMA Lossless
					audioType = AudioType.Lossless;
				}
				else if (description.Contains("wma"))
				{
					// Regular WMA is lossy
					audioType = AudioType.Lossy;
				}

				if (audioType != AudioType.Unknown)
				{
					break;
				}
			}
		}

		return audioType;
	}

	private static AudioType GetAudioTypeMka(string filePath)
	{
		AudioType audioType = AudioType.Unknown;

		using TagLib.File file = TagLib.File.Create(filePath);

		if (file.Properties.Codecs != null)
		{
			foreach (TagLib.ICodec? codec in file.Properties.Codecs)
			{
				string description = codec.Description;
				description = description?.ToLower() ?? string.Empty;

				if (description.Contains("alac") ||
					description.Contains("ape") ||
					description.Contains("flac") ||
					description.Contains("pcm") ||
					description.Contains("wav") ||
					description.Contains("wavpack"))
				{
					// Lossless codecs
					audioType = AudioType.Lossless;
				}

				if (description.Contains("aac") ||
					description.Contains("ac3") ||
					description.Contains("dts") ||
					description.Contains("mp3") ||
					description.Contains("opus") ||
					description.Contains("vorbis"))
				{
					// Lossy codecs
					audioType = AudioType.Lossy;
				}

				if (audioType != AudioType.Unknown)
				{
					break;
				}
			}
		}

		return audioType;
	}

	private static AudioType GetAudioTypeOgg(string filePath)
	{
		AudioType audioType = AudioType.Unknown;

		using TagLib.File file = TagLib.File.Create(filePath);

		if (file.Properties.Codecs != null)
		{
			foreach (TagLib.ICodec? codec in file.Properties.Codecs)
			{
				if (codec is TagLib.Ogg.Codecs.Opus ||
					codec is TagLib.Ogg.Codecs.Vorbis)
				{
					// Seems Not Supported: codec is TagLib.Ogg.Codecs.Speex
					audioType = AudioType.Lossy;
				}
				else
				{
					// Seems Not Supported: codec is TagLib.Ogg.Codecs.Flac

					string description = codec.Description;
					description = description?.ToLower() ?? string.Empty;

					if (description.Contains("lossless"))
					{
						audioType = AudioType.Lossless;
					}
				}

				if (audioType != AudioType.Unknown)
				{
					break;
				}
			}
		}

		return audioType;
	}

	private static AudioType GetAudioTypeWavPack(string filePath)
	{
		AudioType audioType = AudioType.Unknown;

		using TagLib.File file = TagLib.File.Create(filePath);

		// WavPack files report bits per sample. Hybrid mode typically shows as
		// lossy compression.Pure lossless will have full bit depth.
		if (file.Properties.BitsPerSample > 0)
		{
			// If we can read the file, assume lossless unless we detect
			// hybrid mode. This is a simplification - hybrid detection is
			// complex/
			audioType = AudioType.Lossless;
		}

		return audioType;
	}
}
