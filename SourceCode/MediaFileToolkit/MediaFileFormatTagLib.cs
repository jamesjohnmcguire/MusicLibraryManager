/////////////////////////////////////////////////////////////////////////////
// <copyright file="MediaFileFormatTagLib.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace MediaFileToolkit;

using System;

/// <summary>
/// Provides static methods for determining the audio type (lossless, lossy, or
/// unknown) of various media file formats using TagLib metadata analysis.
/// </summary>
/// <remarks>This class is intended for internal use to assist in identifying
/// the encoding type of audio streams within supported file formats such as
/// M4A, MKA, OGG, WavPack, and WMA. All methods are static and require a valid
/// file path to an existing media file. The results depend on the accuracy and
/// completeness of the metadata provided by TagLib and the codecs present in
/// the file. This class does not modify files or their metadata.</remarks>
internal class MediaFileFormatTagLib : IMediaFileFormat
{
	/// <summary>
	/// Determines the audio type of an M4A file based on its codec
	/// information.
	/// </summary>
	/// <remarks>This method inspects the codecs present in the specified M4A
	/// file to distinguish between lossless (such as ALAC) and lossy (such as
	/// AAC) audio formats. If the codec cannot be determined,
	/// AudioType.Unknown is returned.</remarks>
	/// <param name="filePath">The path to the M4A file to analyze.
	/// Cannot be null or empty.</param>
	/// <returns>An AudioType value indicating whether the file is lossless,
	/// lossy, or unknown.</returns>
	public static AudioType GetAudioTypeM4a(string filePath)
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

	/// <summary>
	/// Determines the audio type of a Matroska audio (MKA) file based on its
	/// codec information.
	/// </summary>
	/// <remarks>This method inspects the codecs present in the specified MKA
	/// file to classify the audio as lossless, lossy, or unknown. If the file
	/// contains multiple codecs, only the first recognized codec determines
	/// the result. The method does not validate the file's existence or format
	/// beyond what TagLib supports.</remarks>
	/// <param name="filePath">The path to the MKA file to analyze.
	/// Cannot be null or an empty string.</param>
	/// <returns>An AudioType value indicating whether the file uses a
	/// lossless, lossy, or unknown codec.</returns>
	public static AudioType GetAudioTypeMka(string filePath)
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

	/// <summary>
	/// Determines the audio type of an Ogg file based on its codec
	/// information.
	/// </summary>
	/// <remarks>This method inspects the codecs present in the specified Ogg
	/// file to classify its audio type. If the codec is recognized as Opus or
	/// Vorbis, the file is considered lossy. If the codec description contains
	/// 'LOSSLESS', the file is considered lossless. If the codec cannot be
	/// determined, AudioType.Unknown is returned.</remarks>
	/// <param name="filePath">The path to the Ogg audio file to analyze.
	/// Cannot be null or empty.</param>
	/// <returns>An AudioType value indicating whether the file is lossy,
	/// lossless, or unknown based on the detected codec.</returns>
	public static AudioType GetAudioTypeOgg(string filePath)
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

	/// <summary>
	/// Determines the audio type of the specified WavPack file based on its
	/// codec information.
	/// </summary>
	/// <remarks>This method checks if the file properties BitsPerSample
	/// property is greater then zero, if so, the file is considered lossless,
	/// otherwise, tentatively, the file is lossy.</remarks>
	/// <param name="filePath">The path to the WavPack audio file to analyze.
	/// Cannot be null or empty.</param>
	/// <returns>An AudioType value indicating whether the file is lossy,
	/// lossless, or unknown based on the detected codec.</returns>
	public static AudioType GetAudioTypeWavPack(string filePath)
	{
		AudioType audioType = AudioType.Unknown;

		using TagLib.File file = TagLib.File.Create(filePath);

		// WavPack files report bits per sample. Hybrid mode typically shows as
		// lossy compression.  Pure lossless will have full bit depth.
		if (file.Properties.BitsPerSample > 0)
		{
			// If we can read the file, assume lossless unless we detect
			// hybrid mode. This is a simplification - hybrid detection is
			// complex/
			audioType = AudioType.Lossless;
		}

		return audioType;
	}

	/// <summary>
	/// Determines the audio type of a Windows Media Audio (WMA) file based on
	/// its codec information.
	/// </summary>
	/// <remarks>This method inspects the codecs present in the specified
	/// Windows Media Audio (WMA) file to classify its audio type. If the codec
	/// description contains 'LOSSLESS', the file is considered lossless. If
	/// the codec description contains 'WMA', the file is considered lossy. If
	/// the codec cannot be determined, AudioType.Unknown is returned.
	/// </remarks>
	/// <param name="filePath">The path to the WMA file to analyze. Must refer
	/// to a valid WMA file; otherwise, the result may be inaccurate.</param>
	/// <returns>An AudioType value indicating whether the file is lossy,
	/// lossless, or unknown based on the detected codec.</returns>
	public static AudioType GetAudioTypeWma(string filePath)
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
}
