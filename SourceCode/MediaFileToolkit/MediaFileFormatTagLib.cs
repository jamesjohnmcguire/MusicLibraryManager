/////////////////////////////////////////////////////////////////////////////
// <copyright file="MediaFileFormatTagLib.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace MediaFileToolkit;

using System;

/// <summary>
/// Represents a method that analyzes a codec.
/// </summary>
/// <param name="codec">The codec to analyze.</param>
/// <param name="description">The codec's description.</param>
/// <returns>A CompressionType value indicating whether the file is lossless,
/// lossy, or unknown.</returns>
public delegate CompressionType AnalyzeCodec(
	TagLib.ICodec? codec, string description);

/// <summary>
/// Provides methods for determining the audio type of various media file
/// formats using TagLib metadata analysis.
/// </summary>
/// <remarks>This class implements the <see cref="IMediaFileFormat"/> interface
/// to analyze audio files and identify their encoding type.  This class is
/// intended to assist in identifying the encoding type of various file
/// formats. All methods require a valid file path to an existing media file.
/// The class methods returns a CompressionType value indicating whether the
/// audio is lossless, lossy, or unknown. For formats that are not currently
/// supported, the methods return CompressionType.Unknown. The results depend
/// on the accuracy and completeness of the metadata provided by TagLib and the
/// codecs present in the file. This class does not modify files or their
/// metadata.</remarks>
/// <remarks>This class implements the <see cref="IMediaFileFormat"/> interface
/// to analyze audio files and identify their encoding type. This class is
/// intended to assist in identifying
/// the encoding type of various file formats. All methods are static and
/// require a valid file path to an existing media file. The results depend on
/// the accuracy and completeness of the metadata provided by TagLib and the
/// codecs present in the file. This class does not modify files or their
/// metadata.</remarks>
public class MediaFileFormatTagLib : IMediaFileFormat
{
	/// <summary>
	/// Determines the audio type of an M4A file based on its codec
	/// information.
	/// </summary>
	/// <remarks>This method inspects the codecs present in the specified M4A
	/// file to distinguish between lossless (such as ALAC) and lossy (such as
	/// AAC) audio formats. If the codec cannot be determined,
	/// CompressionType.Unknown is returned.</remarks>
	/// <param name="filePath">The path to the M4A file to analyze.
	/// Cannot be null or empty.</param>
	/// <returns>A CompressionType value indicating whether the file is
	/// lossless, lossy, or unknown.</returns>
	public CompressionType GetCompressionTypeM4a(string filePath)
	{
		CompressionType compressionType =
			IterateTagLibCodecs(filePath, AnalyzeCodecM4a);

		return compressionType;
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
	/// <returns>A CompressionType value indicating whether the file uses a
	/// lossless, lossy, or unknown codec.</returns>
	public CompressionType GetCompressionTypeMka(string filePath)
	{
		CompressionType compressionType =
			IterateTagLibCodecs(filePath, AnalyzeCodecMka);

		return compressionType;
	}

	/// <summary>
	/// Determines the audio type of an Ogg file based on its codec
	/// information.
	/// </summary>
	/// <remarks>This method inspects the codecs present in the specified Ogg
	/// file to classify its audio type. If the codec is recognized as Opus or
	/// Vorbis, the file is considered lossy. If the codec description contains
	/// 'LOSSLESS', the file is considered lossless. If the codec cannot be
	/// determined, CompressionType.Unknown is returned.</remarks>
	/// <param name="filePath">The path to the Ogg audio file to analyze.
	/// Cannot be null or empty.</param>
	/// <returns>A CompressionType value indicating whether the file is lossy,
	/// lossless, or unknown based on the detected codec.</returns>
	public CompressionType GetCompressionTypeOgg(string filePath)
	{
		CompressionType compressionType =
			IterateTagLibCodecs(filePath, AnalyzeCodecOgg);

		return compressionType;
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
	/// <returns>A CompressionType value indicating whether the file is lossy,
	/// lossless, or unknown based on the detected codec.</returns>
	public CompressionType GetCompressionTypeWavPack(string filePath)
	{
		CompressionType compressionType = CompressionType.Unknown;

		using TagLib.File file = TagLib.File.Create(filePath);

		// WavPack files report bits per sample. Hybrid mode typically shows as
		// lossy compression.  Pure lossless will have full bit depth.
		if (file.Properties.BitsPerSample > 0)
		{
			// If we can read the file, assume lossless unless we detect
			// hybrid mode. This is a simplification - hybrid detection is
			// complex/
			compressionType = CompressionType.Lossless;
		}

		return compressionType;
	}

	/// <summary>
	/// Determines the audio type of a Windows Media Audio (WMA) file based on
	/// its codec information.
	/// </summary>
	/// <remarks>This method inspects the codecs present in the specified
	/// Windows Media Audio (WMA) file to classify its audio type. If the codec
	/// description contains 'LOSSLESS', the file is considered lossless. If
	/// the codec description contains 'WMA', the file is considered lossy. If
	/// the codec cannot be determined, CompressionType.Unknown is returned.
	/// </remarks>
	/// <param name="filePath">The path to the WMA file to analyze. Must refer
	/// to a valid WMA file; otherwise, the result may be inaccurate.</param>
	/// <returns>A CompressionType value indicating whether the file is lossy,
	/// lossless, or unknown based on the detected codec.</returns>
	public CompressionType GetCompressionTypeWma(string filePath)
	{
		CompressionType compressionType =
			IterateTagLibCodecs(filePath, AnalyzeCodecWma);

		return compressionType;
	}

	private static CompressionType IterateTagLibCodecs(
		string filePath, AnalyzeCodec analyzer)
	{
		CompressionType compressionType = CompressionType.Unknown;

		using TagLib.File file = TagLib.File.Create(filePath);

		if (file.Properties.Codecs != null)
		{
			foreach (TagLib.ICodec? codec in file.Properties.Codecs)
			{
				string description = codec.Description;
				description =
					description?.ToUpperInvariant() ?? string.Empty;

				compressionType = analyzer(codec, description);

				if (compressionType != CompressionType.Unknown)
				{
					break;
				}
			}
		}

		return compressionType;
	}

	private static CompressionType AnalyzeCodecM4a(
		TagLib.ICodec? codec, string description)
	{
		CompressionType compressionType = CompressionType.Unknown;

		if (codec != null)
		{
			if (codec is TagLib.Mpeg4.IsoAudioSampleEntry audioEntry)
			{
				if (audioEntry.BoxType == "alac")
				{
					compressionType = CompressionType.Lossless;
				}

				if (audioEntry.BoxType == "mp4a")
				{
					// ALAC sometimes reports as mp4a with ALAC description
					if (description.Contains(
						"ALAC", StringComparison.Ordinal) ||
						description.Contains(
							"APPLE LOSSLESS", StringComparison.Ordinal))
					{
						compressionType = CompressionType.Lossless;
					}
					else if (description.Contains(
						"AAC", StringComparison.Ordinal) ||
						description.Contains(
							"MPEG-4", StringComparison.Ordinal))
					{
						// Most mp4a entries are AAC variants (lossy)
						// AAC-LC, AAC-HE, AAC-HEv2, etc.
						compressionType = CompressionType.Lossy;
					}
					else
					{
						// Add additional checks here for other codecs.
						// If we can't determine specifically,
						// mp4a is typically lossy
						compressionType = CompressionType.Lossy;
					}
				}
			}
		}

		return compressionType;
	}

	private static CompressionType AnalyzeCodecMka(
		TagLib.ICodec? codec, string description)
	{
		CompressionType compressionType = CompressionType.Unknown;

		if (codec != null)
		{
			if (description.Contains("ALAC", StringComparison.Ordinal) ||
				description.Contains("APE", StringComparison.Ordinal) ||
				description.Contains("FLAC", StringComparison.Ordinal) ||
				description.Contains("PCM", StringComparison.Ordinal) ||
				description.Contains("WAV", StringComparison.Ordinal) ||
				description.Contains("WAVPACK", StringComparison.Ordinal))
			{
				// Lossless codecs
				compressionType = CompressionType.Lossless;
			}

			if (description.Contains("AAC", StringComparison.Ordinal) ||
				description.Contains("AC3", StringComparison.Ordinal) ||
				description.Contains("DTS", StringComparison.Ordinal) ||
				description.Contains("MP3", StringComparison.Ordinal) ||
				description.Contains("OPUS", StringComparison.Ordinal) ||
				description.Contains("VORBIS", StringComparison.Ordinal))
			{
				// Lossy codecs
				compressionType = CompressionType.Lossy;
			}
		}

		return compressionType;
	}

	private static CompressionType AnalyzeCodecOgg(
		TagLib.ICodec? codec, string description)
	{
		CompressionType compressionType = CompressionType.Unknown;

		if (codec != null)
		{
			if (codec is TagLib.Ogg.Codecs.Opus ||
				codec is TagLib.Ogg.Codecs.Vorbis)
			{
				// Seems Not Supported: codec is TagLib.Ogg.Codecs.Speex
				compressionType = CompressionType.Lossy;
			}
			else
			{
				// Seems Not Supported: codec is TagLib.Ogg.Codecs.Flac
				if (description.Contains(
					"LOSSLESS", StringComparison.Ordinal))
				{
					compressionType = CompressionType.Lossless;
				}
			}
		}

		return compressionType;
	}

	private static CompressionType AnalyzeCodecWma(
		TagLib.ICodec? codec, string description)
	{
		CompressionType compressionType = CompressionType.Unknown;

		if (codec != null)
		{
			if (description.Contains("LOSSLESS", StringComparison.Ordinal))
			{
				// WMA Lossless
				compressionType = CompressionType.Lossless;
			}
			else if (description.Contains("WMA", StringComparison.Ordinal))
			{
				// Regular WMA is lossy
				compressionType = CompressionType.Lossy;
			}
		}

		return compressionType;
	}
}
