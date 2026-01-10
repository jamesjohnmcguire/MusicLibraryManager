/////////////////////////////////////////////////////////////////////////////
// <copyright file="IAudioConverter.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace MediaFileToolkit;

using System.IO;
using System.Threading.Tasks;

/// <summary>
/// Defines a contract for converting audio files asynchronously.
/// </summary>
/// <remarks>Implementations of this interface should provide logic to convert
/// the specified input file to a desired audio format. The conversion process
/// is performed asynchronously and may involve I/O operations. Thread safety
/// and supported formats depend on the specific implementation.</remarks>
internal interface IAudioConverter
{
	/// <summary>
	/// Asynchronously converts the specified input file to the target format.
	/// </summary>
	/// <param name="inputFile">The file to be converted. Must not be null and
	/// should reference an existing file.</param>
	/// <returns>A task that represents the asynchronous conversion operation.
	/// </returns>
	Task ConvertFileAsync(FileInfo inputFile);

	/// <summary>
	/// Asynchronously converts all files of the specified types in the given
	/// directory.
	/// </summary>
	/// <param name="inputDirectory">The directory containing the files to
	/// convert. Must not be null.</param>
	/// <param name="fileTypes">A comma-separated list of file extensions
	/// (without dots) specifying which file types to convert. For example,
	/// "mp3, m4a".</param>
	/// <returns>A task that represents the asynchronous conversion operation.
	/// </returns>
	Task ConvertFilesAsync(DirectoryInfo inputDirectory, string fileTypes);
}
