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
/// <remarks>This class implements the IMediaFileFormat interface to analyze
/// audio files and identify their encoding type. All methods are static and
/// are intended for internal use within the application. The class supports
/// formats such as M4A and WMA, and returns an AudioType value indicating
/// whether the audio is lossless, lossy, or unknown. For formats that are not
/// currently supported, the methods return AudioType.Unknown.</remarks>
internal class MediaFileFormatNaudio : IMediaFileFormat
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
	/// <returns>An AudioType value indicating whether the file uses a lossless
	/// or lossy encoding.</returns>
	public static AudioType GetAudioTypeM4a(string filePath)
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

	/// <summary>
	/// Determines the audio type of a Matroska audio file (.mka) based on the
	/// specified file path.
	/// </summary>
	/// <param name="filePath">The path to the Matroska audio file (.mka) to
	/// analyze. Cannot be null or empty.</param>
	/// <returns>An AudioType value representing the detected audio type of the
	/// specified file. Returns AudioType.Unknown if the type cannot be
	/// determined.</returns>
	public static AudioType GetAudioTypeMka(string filePath)
	{
		AudioType audioType = AudioType.Unknown;

		return audioType;
	}

	/// <summary>
	/// Determines the audio type of the specified Ogg file.
	/// </summary>
	/// <param name="filePath">The path to the Ogg audio file to analyze.
	/// Cannot be null or empty.</param>
	/// <returns>An AudioType value representing the type of the Ogg audio
	/// file. Returns AudioType.Unknown if the type cannot be determined.
	/// </returns>
	public static AudioType GetAudioTypeOgg(string filePath)
	{
		AudioType audioType = AudioType.Unknown;

		return audioType;
	}

	/// <summary>
	/// Determines the audio type of the specified WavPack file.
	/// </summary>
	/// <param name="filePath">The path to the WavPack audio file to analyze.
	/// Cannot be null or empty.</param>
	/// <returns>An AudioType value representing the type of the specified
	/// audio file. Returns AudioType.Unknown if the type cannot be determined.
	/// </returns>
	public static AudioType GetAudioTypeWavPack(string filePath)
	{
		AudioType audioType = AudioType.Unknown;

		return audioType;
	}

	/// <summary>
	/// Determines the audio type of a Windows Media Audio (WMA) file based on
	/// its encoding format.
	/// </summary>
	/// <remarks>This method inspects the encoding of the specified WMA file to
	/// classify it as lossless or lossy. If the encoding is not recognized as
	/// a standard WMA format, the method returns AudioType.Unknown.</remarks>
	/// <param name="filePath">The path to the WMA file to analyze. Must refer
	/// to a valid WMA file; otherwise, the result may be inaccurate.</param>
	/// <returns>An AudioType value indicating whether the file is lossless,
	/// lossy, or unknown based on its encoding.</returns>
	public static AudioType GetAudioTypeWma(string filePath)
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
}
