/////////////////////////////////////////////////////////////////////////////
// <copyright file="AudioConveterFfmpeg.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace MediaFileToolkit;

using System.IO;
using System.Threading.Tasks;

/// <summary>
/// Provides functionality to convert audio files using FFmpeg.
/// </summary>
/// <remarks>This class implements the <see cref="IAudioConverter"/> interface
/// to perform audio file conversions. It is intended for internal use and may
/// rely on FFmpeg being available in the execution environment.</remarks>
internal class AudioConveterFfmpeg : IAudioConverter
{
	/// <summary>
	/// Asynchronously converts the specified input file to the target format.
	/// </summary>
	/// <param name="inputFile">The file to be converted. Must be a valid,
	/// existing file.</param>
	/// <returns>A task that represents the asynchronous conversion operation.
	/// </returns>
	public async Task ConvertFileAsync(FileInfo inputFile)
	{
	}
}
