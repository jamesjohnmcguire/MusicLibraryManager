/////////////////////////////////////////////////////////////////////////////
// <copyright file="MediaFileFormatFfmpeg.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace MediaFileToolkit;

using System;
using System.Threading.Tasks;
using global::Common.Logging;
using Newtonsoft.Json;

/// <summary>
/// Provides methods for determining the audio type of various media file
/// formats using FFmpeg.
/// </summary>
/// <remarks>This class implements the <see cref="IMediaFileFormat"/> interface
/// to analyze audio files and identify their encoding type.  This class is
/// intended to assist in identifying the encoding type of various file
/// formats. All methods require a valid file path to an existing media file.
/// The class methods return an AudioType value indicating whether the
/// audio is lossless, lossy, or unknown. For formats that are not currently
/// supported, the methods return AudioType.Unknown. The results depend on the
/// accuracy of the analysis provided by FFMpeg. This class does not modify
/// files.
/// </remarks>
internal class MediaFileFormatFfmpeg : IMediaFileFormat
{
	private static readonly Type LogType = typeof(AudioConveterFfmpeg);
	private static readonly ILog Log = LogManager.GetLogger(LogType);

	/// <summary>
	/// Gets the media stream information obtained from FFprobe.
	/// </summary>
	public MediaStream? MediaStream { get; private set; }

	/// <summary>
	/// Determines the audio type of an M4A file based on its encoding format.
	/// </summary>
	/// <param name="filePath">The path to the M4A audio file to analyze. Must
	/// refer to a valid, accessible file.</param>
	/// <returns>An AudioType value indicating whether the file uses a lossless
	/// or lossy encoding.</returns>
	public AudioType GetAudioTypeM4a(string filePath)
	{
		AudioType audioType = AudioType.Unknown;

		audioType = ProcessFile(filePath);

		if (MediaStream != null)
		{
			string codecName = MediaStream.CodecName ?? string.Empty;
			codecName = codecName.ToUpperInvariant();

			if (codecName.Contains("ALAC", StringComparison.Ordinal) ||
				codecName.Contains("LOSSLESS", StringComparison.Ordinal))
			{
				// WMA Lossless
				audioType = AudioType.Lossless;
			}
			else if (codecName.Contains("AAC", StringComparison.Ordinal))
			{
				// Regular WMA is lossy
				audioType = AudioType.Lossy;
			}
		}

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
	public AudioType GetAudioTypeMka(string filePath)
	{
		AudioType audioType = AudioType.Unknown;

		audioType = ProcessFile(filePath);

		if (MediaStream != null)
		{
			string codecName = MediaStream.CodecName ?? string.Empty;
			codecName = codecName.ToUpperInvariant();

			if (codecName.Contains("LOSSLESS", StringComparison.Ordinal) ||
				codecName.Contains("ALAC", StringComparison.Ordinal) ||
				codecName.Contains("APE", StringComparison.Ordinal) ||
				codecName.Contains("FLAC", StringComparison.Ordinal) ||
				codecName.Contains("PCM", StringComparison.Ordinal) ||
				codecName.Contains("WAV", StringComparison.Ordinal) ||
				codecName.Contains("WAVPACK", StringComparison.Ordinal))
			{
				audioType = AudioType.Lossless;
			}
			else if (codecName.Contains("AAC", StringComparison.Ordinal) ||
				codecName.Contains("AC3", StringComparison.Ordinal) ||
				codecName.Contains("DTS", StringComparison.Ordinal) ||
				codecName.Contains("MP3", StringComparison.Ordinal) ||
				codecName.Contains("OPUS", StringComparison.Ordinal) ||
				codecName.Contains("VORBIS", StringComparison.Ordinal))
			{
				audioType = AudioType.Lossy;
			}
		}

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
	public AudioType GetAudioTypeOgg(string filePath)
	{
		AudioType audioType = AudioType.Unknown;

		audioType = ProcessFile(filePath);

		if (MediaStream != null)
		{
			string codecName = MediaStream.CodecName ?? string.Empty;
			codecName = codecName.ToUpperInvariant();

			if (codecName.Contains("LOSSLESS", StringComparison.Ordinal))
			{
				audioType = AudioType.Lossless;
			}
			else if (codecName.Contains("OPUS", StringComparison.Ordinal) ||
				codecName.Contains("VORBIS", StringComparison.Ordinal))
			{
				audioType = AudioType.Lossy;
			}
		}

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
	public AudioType GetAudioTypeWavPack(string filePath)
	{
		AudioType audioType = AudioType.Unknown;

		audioType = ProcessFile(filePath);

		if (MediaStream != null)
		{
			// WavPack files report bits per sample. Hybrid mode typically
			// shows as lossy compression.  Pure lossless will have full bit
			// depth.
			if (MediaStream.BitsPerSample > 0)
			{
				// If we can read the file, assume lossless unless we detect
				// hybrid mode. This is a simplification - hybrid detection is
				// complex/
				audioType = AudioType.Lossless;
			}
		}

		return audioType;
	}

	/// <summary>
	/// Determines the audio type of a Windows Media Audio (WMA) file based on
	/// its encoding format.
	/// </summary>
	/// <param name="filePath">The path to the WMA file to analyze. Must refer
	/// to a valid WMA file; otherwise, the result may be inaccurate.</param>
	/// <returns>An AudioType value indicating whether the file is lossless,
	/// lossy, or unknown based on its encoding.</returns>
	public AudioType GetAudioTypeWma(string filePath)
	{
		AudioType audioType = AudioType.Unknown;

		audioType = ProcessFile(filePath);

		if (MediaStream != null)
		{
			string codecName = MediaStream.CodecName ?? string.Empty;
			codecName = codecName.ToUpperInvariant();

			if (codecName.Contains("LOSSLESS", StringComparison.Ordinal))
			{
				// WMA Lossless
				audioType = AudioType.Lossless;
			}
			else if (codecName.Contains("WMA", StringComparison.Ordinal))
			{
				// Regular WMA is lossy
				audioType = AudioType.Lossy;
			}
		}

		return audioType;
	}

	private AudioType ProcessFile(string filePath)
	{
		AudioType audioType = AudioType.Unknown;

		string arguments =
			"-of json" +
			"-v error " +
			"-select_streams a:0 " +
			"-show_entries stream=bit_rate,bits_per_sample,codec_long_name," +
			"codec_name,codec_tag_string,profile,sample_rate ";

		Log.Info($"Converting: {filePath}");
		ExternalProcess process = new();

		bool result = process.Execute("ffprobe", arguments);

		if (result == false)
		{
			string error = process.Output;
			string message = $"Error analyzing {filePath}: {error}";
			Log.Error(message);
		}
		else
		{
			audioType = GetAudioTypeFromFfprobe(process.Output);
		}

		return audioType;
	}

	private async Task<AudioType> ProcessFileAsync(string filePath)
	{
		AudioType audioType = AudioType.Unknown;

		string arguments =
			"-v error -show_entries stream=codec_name,bit_rate,sample_rate";

		Log.Info($"Converting: {filePath}");
		ExternalProcess process = new();

		bool result = await process.ExecuteAsync("ffprobe", arguments).
			ConfigureAwait(false);

		if (result == false)
		{
			string error = process.Output;
			string message = $"Error analyzing {filePath}: {error}";
			Log.Error(message);
		}
		else
		{
			audioType = GetAudioTypeFromFfprobe(process.Output);
		}

		return audioType;
	}

	private static MediaStream? GetMediaStremFromFfprobe(string ffprobeJson)
	{
		MediaStream? mediaStream =
			JsonConvert.DeserializeObject<MediaStream>(ffprobeJson);

		return mediaStream;
	}

	private AudioType GetAudioTypeFromFfprobe(string ffprobeJson)
	{
		AudioType audioType = AudioType.Unknown;

		MediaStream = GetMediaStremFromFfprobe(ffprobeJson);

		return audioType;
	}
}
