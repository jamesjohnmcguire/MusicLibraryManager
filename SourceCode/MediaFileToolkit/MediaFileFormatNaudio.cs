/////////////////////////////////////////////////////////////////////////////
// <copyright file="MediaFileFormatNaudio.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace MediaFileToolkit;

using NAudio.Wave;

/// <summary>
/// Provides methods for determining the audio type of various media file
/// formats using NAudio.
/// </summary>
/// <remarks>This class implements the <see cref="IMediaFileFormat"/> interface
/// to analyze audio files and identify their encoding type.  This class is
/// intended to assist in identifying the encoding type of various file
/// formats. All methods require a valid file path to an existing media file.
/// The class methods return a CompressionType value indicating whether the
/// audio is lossless, lossy, or unknown. For formats that are not currently
/// supported, the methods return CompressionType.Unknown. The results depend
/// on the accuracy of the analysis provided by NAudio. This class does not
/// modify files.</remarks>
public class MediaFileFormatNaudio : IMediaFileFormat
{
	/// <summary>
	/// Determines the audio type of an M4A file based on its encoding format.
	/// </summary>
	/// <remarks>This method inspects the encoding of the specified M4A file
	/// to classify it as lossless or lossy.
	/// The result depends on the file's internal format and may not reflect
	/// the original source quality.</remarks>
	/// <param name="filePath">The path to the M4A audio file to analyze. Must
	/// refer to a valid, accessible file.</param>
	/// <returns>A CompressionType value indicating whether the file uses a
	/// lossless or lossy encoding.</returns>
	public CompressionType GetCompressionTypeM4a(string filePath)
	{
		using MediaFoundationReader reader = new(filePath);

		WaveFormat format = reader.WaveFormat;

		CompressionType compressionType = format.Encoding switch
		{
			WaveFormatEncoding.Pcm => CompressionType.Lossless,
			WaveFormatEncoding.Adpcm => CompressionType.Lossless,
			WaveFormatEncoding.IeeeFloat => CompressionType.Lossless,
			_ => CompressionType.Lossy,
		};

		return compressionType;
	}

	/// <summary>
	/// Determines the audio type of a Matroska audio file (.mka) based on the
	/// specified file path.
	/// </summary>
	/// <param name="filePath">The path to the Matroska audio file (.mka) to
	/// analyze. Cannot be null or empty.</param>
	/// <returns>A CompressionType value representing the detected audio type
	/// of the specified file. Returns CompressionType.Unknown if the type
	/// cannot be determined.</returns>
	public CompressionType GetCompressionTypeMka(string filePath)
	{
		CompressionType compressionType = CompressionType.Unknown;

		return compressionType;
	}

	/// <summary>
	/// Determines the audio type of the specified Ogg file.
	/// </summary>
	/// <param name="filePath">The path to the Ogg audio file to analyze.
	/// Cannot be null or empty.</param>
	/// <returns>A CompressionType value representing the type of the Ogg audio
	/// file. Returns CompressionType.Unknown if the type cannot be determined.
	/// </returns>
	public CompressionType GetCompressionTypeOgg(string filePath)
	{
		CompressionType compressionType = CompressionType.Unknown;

		return compressionType;
	}

	/// <summary>
	/// Determines the audio type of the specified WavPack file.
	/// </summary>
	/// <param name="filePath">The path to the WavPack audio file to analyze.
	/// Cannot be null or empty.</param>
	/// <returns>A CompressionType value representing the type of the specified
	/// audio file. Returns CompressionType.Unknown if the type cannot be
	/// determined.</returns>
	public CompressionType GetCompressionTypeWavPack(string filePath)
	{
		CompressionType compressionType = CompressionType.Unknown;

		return compressionType;
	}

	/// <summary>
	/// Determines the audio type of a Windows Media Audio (WMA) file based on
	/// its encoding format.
	/// </summary>
	/// <remarks>This method inspects the encoding of the specified WMA file to
	/// classify it as lossless or lossy. If the encoding is not recognized as
	/// a standard WMA format, the method returns CompressionType.Unknown.
	/// </remarks>
	/// <param name="filePath">The path to the WMA file to analyze. Must refer
	/// to a valid WMA file; otherwise, the result may be inaccurate.</param>
	/// <returns>A CompressionType value indicating whether the file is
	/// lossless, lossy, or unknown based on its encoding.</returns>
	public CompressionType GetCompressionTypeWma(string filePath)
	{
		using MediaFoundationReader reader = new(filePath);

		WaveFormat format = reader.WaveFormat;

		CompressionType compressionType = format.Encoding switch
		{
			WaveFormatEncoding.WindowsMediaAudioLosseless =>
				CompressionType.Lossless,

			// WMA Standard
			WaveFormatEncoding.WindowsMediaAudio => CompressionType.Lossy,
			WaveFormatEncoding.WindowsMediaAudioProfessional =>
				CompressionType.Lossy,
			WaveFormatEncoding.WindowsMediaAudioSpdif => CompressionType.Lossy,
			_ => CompressionType.Unknown,
		};

		return compressionType;
	}
}
