/////////////////////////////////////////////////////////////////////////////
// <copyright file="MediaFileFormat.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace MediaFileToolkit;

using System;
using System.IO;
using NAudio.Wave;

/// <summary>
/// Provides methods for determining the audio type (lossy, lossless, or
/// unknown) of a media file based on its file path and format.
/// </summary>
/// <remarks>This class supports a variety of common audio file formats and
/// attempts to classify them as lossy or lossless using file extension and,
/// where necessary, file content analysis. The classification may depend on
/// the platform and available libraries. Not all formats can be definitively
/// classified; in such cases, the result may be AudioType.Unknown.</remarks>
public static class MediaFileFormat
{
	/// <summary>
	/// Determines the audio type of the specified file based on its file
	/// extension and, for certain formats, by inspecting the file contents.
	/// </summary>
	/// <remarks>For some formats, such as .m4a and .wma, the method may
	/// inspect the file contents to determine the audio type. Formats that
	/// can be either lossy or lossless may return AudioType.Unknown if the
	/// type cannot be determined from the extension alone.</remarks>
	/// <param name="filePath">The full path to the audio file to analyze.
	/// Cannot be null or empty.</param>
	/// <returns>An AudioType value indicating whether the file is a lossy,
	/// lossless, or unknown audio type. Returns AudioType.Unknown if the type
	/// cannot be determined.</returns>
	/// <exception cref="FileNotFoundException">Thrown if the file specified by
	/// filePath does not exist.</exception>
	public static AudioType GetAudioType(string filePath)
	{
		AudioType audioType = AudioType.Unknown;

		if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
		{
			throw new FileNotFoundException("File not found: " + filePath);
		}

		string extension = Path.GetExtension(filePath);
		extension = filePath.ToUpperInvariant();

		switch (extension)
		{
			// Only lossy formats
			case ".AAC":
			case ".MP3":
			case ".OPUS":
				audioType = AudioType.Lossy;
				break;

			// Only lossless formats
			case ".AIFF": // Rare edge case: AIFF-C can contain lossy
			case ".APE":
			case ".FLAC":
			case ".TTA":
			case ".WAV":
				audioType = AudioType.Lossless;
				break;

			// Both lossy and lossless formats
			case ".MKA":
			case ".OGG":
			case ".WV": // WavPack
				// TODO
				break;

			case ".M4A":
#if WINDOWS
				audioType = GetAudioTypeM4aNaudio(filePath);
#else
				audioType = GetAudioTypeM4aTagLib(filePath);
#endif
				break;

			case ".WMA":
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
		using MediaFoundationReader reader = new(filePath);

		WaveFormat format = reader.WaveFormat;

		AudioType audioType = format.Encoding switch
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
						description =
							description?.ToUpperInvariant() ?? string.Empty;

						// ALAC sometimes reports as mp4a with ALAC description
						if (description.Contains(
							"ALAC", StringComparison.Ordinal) ||
							description.Contains(
								"APPLE LOSSLESS", StringComparison.Ordinal))
						{
							audioType = AudioType.Lossless;
						}
						else if (description.Contains(
							"AAC", StringComparison.Ordinal) ||
							description.Contains(
								"MPEG-4", StringComparison.Ordinal))
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
					}
				}
			}
		}

		return audioType;
	}

	private static AudioType GetAudioTypeWmaNaudio(string filePath)
	{
		using MediaFoundationReader reader = new(filePath);

		WaveFormat format = reader.WaveFormat;

		AudioType audioType = format.Encoding switch
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
				description = description?.ToUpperInvariant() ?? string.Empty;

				if (description.Contains("LOSSLESS", StringComparison.Ordinal))
				{
					// WMA Lossless
					audioType = AudioType.Lossless;
				}
				else if (description.Contains("WMA", StringComparison.Ordinal))
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
				description = description?.ToUpperInvariant() ?? string.Empty;

				if (description.Contains("ALAC", StringComparison.Ordinal) ||
					description.Contains("APE", StringComparison.Ordinal) ||
					description.Contains("FLAC", StringComparison.Ordinal) ||
					description.Contains("PCM", StringComparison.Ordinal) ||
					description.Contains("WAV", StringComparison.Ordinal) ||
					description.Contains("WAVPACK", StringComparison.Ordinal))
				{
					// Lossless codecs
					audioType = AudioType.Lossless;
				}

				if (description.Contains("AAC", StringComparison.Ordinal) ||
					description.Contains("AC3", StringComparison.Ordinal) ||
					description.Contains("DTS", StringComparison.Ordinal) ||
					description.Contains("MP3", StringComparison.Ordinal) ||
					description.Contains("OPUS", StringComparison.Ordinal) ||
					description.Contains("VORBIS", StringComparison.Ordinal))
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
					description =
						description?.ToUpperInvariant() ?? string.Empty;

					if (description.Contains(
						"LOSSLESS", StringComparison.Ordinal))
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
