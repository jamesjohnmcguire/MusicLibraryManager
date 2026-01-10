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
/// <remarks>
/// Initializes a new instance of the <see cref="MediaFileFormat"/> class.
/// </remarks>
/// <param name="mediaFileFormat">The media file format provider.</param>
public class MediaFileFormat(IMediaFileFormat mediaFileFormat)
{
	private readonly IMediaFileFormat mediaFileFormat = mediaFileFormat;

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
	public AudioType GetAudioType(string filePath)
	{
		if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
		{
			throw new FileNotFoundException("File not found: " + filePath);
		}

		string extension = Path.GetExtension(filePath);
		extension = extension.ToUpperInvariant();
		var audioType = extension switch
		{
			// Only lossy formats
			".AAC" or ".MP3" or ".OPUS" => AudioType.Lossy,

			// Only lossless formats
			// Rare edge case: AIFF-C can contain lossy
			".AIFF" or ".APE" or ".FLAC" or ".TTA" or ".WAV" => AudioType.Lossless,

			// Both lossy and lossless formats
			".M4A" => mediaFileFormat.GetAudioTypeM4a(filePath),
			".MKA" => mediaFileFormat.GetAudioTypeMka(filePath),
			".OGG" => mediaFileFormat.GetAudioTypeOgg(filePath),
			".WMA" => mediaFileFormat.GetAudioTypeWma(filePath),

			// WavPack
			".WV" => mediaFileFormat.GetAudioTypeWavPack(filePath),
			_ => AudioType.Unknown,
		};
		return audioType;
	}
}
