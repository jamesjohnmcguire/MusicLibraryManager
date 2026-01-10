/////////////////////////////////////////////////////////////////////////////
// <copyright file="IMediaFileFormat.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace MediaFileToolkit;

/// <summary>
/// Defines methods for determining the audio type of various media file
/// formats.
/// </summary>
/// <remarks>Implementations of this interface provide format-specific logic to
/// identify the audio type of files with different extensions, such as M4A,
/// MKA, OGG, WavPack, and WMA. This interface is intended for internal use and
/// is not designed for direct consumption by application code.</remarks>
public interface IMediaFileFormat
{
	/// <summary>
	/// Determines the audio type of the specified M4A file based on its
	/// content or file extension.
	/// </summary>
	/// <param name="filePath">The full path to the M4A audio file to analyze.
	/// Cannot be null or empty.</param>
	/// <returns>An AudioType value that represents the detected type of the
	/// specified M4A file.</returns>
	AudioType GetAudioTypeM4a(string filePath);

	/// <summary>
	/// Determines the audio type of a Matroska audio (MKA) file based on its
	/// content.
	/// </summary>
	/// <param name="filePath">The full path to the MKA file to analyze.
	/// Cannot be null or empty.</param>
	/// <returns>An AudioType value indicating the detected audio type of the
	/// specified MKA file.</returns>
	AudioType GetAudioTypeMka(string filePath);

	/// <summary>
	/// Determines the audio type of the specified Ogg file.
	/// </summary>
	/// <param name="filePath">The path to the Ogg audio file to analyze.
	/// Cannot be null or empty.</param>
	/// <returns>An AudioType value that represents the detected type of the
	/// Ogg audio file.</returns>
	AudioType GetAudioTypeOgg(string filePath);

	/// <summary>
	/// Determines the audio type of a WavPack file based on the specified
	/// file path.
	/// </summary>
	/// <param name="filePath">The path to the WavPack audio file to analyze.
	/// Cannot be null or empty.</param>
	/// <returns>An AudioType value representing the detected audio type of the
	/// specified WavPack file.</returns>
	AudioType GetAudioTypeWavPack(string filePath);

	/// <summary>
	/// Determines the audio type of a Windows Media Audio (WMA) file at the
	/// specified path.
	/// </summary>
	/// <param name="filePath">The full path to the WMA file to analyze.
	/// Cannot be null or empty.</param>
	/// <returns>An AudioType value that represents the type of the specified
	/// WMA file.</returns>
	AudioType GetAudioTypeWma(string filePath);
}
