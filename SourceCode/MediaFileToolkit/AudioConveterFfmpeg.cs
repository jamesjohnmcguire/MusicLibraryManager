/////////////////////////////////////////////////////////////////////////////
// <copyright file="AudioConveterFfmpeg.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace MediaFileToolkit;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using global::Common.Logging;

/// <summary>
/// Provides functionality to convert audio files using FFmpeg.
/// </summary>
/// <remarks>This class implements the <see cref="IAudioConverter"/> interface
/// to perform audio file conversions. It is intended for internal use and may
/// rely on FFmpeg being available in the execution environment.</remarks>
public class AudioConveterFfmpeg : FfmpegBase, IAudioConverter
{
	private static readonly Type LogType = typeof(AudioConveterFfmpeg);
	private static readonly ILog Log = LogManager.GetLogger(LogType);
	private readonly string format;
	private readonly string quality;
	private readonly bool recursive;
	private readonly HashSet<string> qualityRanges =
		new(StringComparer.OrdinalIgnoreCase)
	{ "high", "medium", "low" };

	private readonly HashSet<string> supportedFormats =
		new (StringComparer.OrdinalIgnoreCase)
	{ "flac", "m4a", "mp3", "wma" };

	private bool batchMode;
	private int successCount;
	private int totalFiles;

	/// <summary>
	/// Initializes a new instance of the <see cref="AudioConveterFfmpeg"/>
	/// class to convert. audio files in a specified directory to a given
	/// format and quality.
	/// </summary>
	/// <param name="format">The target audio format for conversion. Supported
	/// values are "m4a", "flac", and "mp3".</param>
	/// <param name="quality">The desired quality level for the output files.
	/// Supported values are "high", "medium", and "low".</param>
	/// <param name="recursive">true to include files in all subdirectories of
	/// the input directory; otherwise, false.</param>
	/// <exception cref="ArgumentException">Thrown if format is not one of the
	/// supported values ("m4a", "flac", "mp3"), or if quality is not one of
	/// the supported values ("high", "medium", "low").</exception>
	public AudioConveterFfmpeg(
		string format,
		string quality,
		bool recursive)
	{
		this.format = format;
		this.quality = quality;
		this.recursive = recursive;

		if (!supportedFormats.Contains(format))
		{
			string message =
				"Invalid format. Supported formats: m4a, flac, mp3";
			throw new ArgumentException(message);
		}

		if (!qualityRanges.Contains(quality))
		{
			string message =
				"Invalid quality. Supported qualities: high, medium, low";
			throw new ArgumentException(message);
		}
	}

	/// <summary>
	/// Asynchronously converts the specified input file to the target format.
	/// </summary>
	/// <param name="inputFile">The file to be converted. Must be a valid,
	/// existing file.</param>
	/// <returns>A task that represents the asynchronous conversion operation.
	/// </returns>
	public async Task ConvertFileAsync(FileInfo inputFile)
	{
		if (inputFile != null)
		{
			string outputPath =
				Path.ChangeExtension(inputFile.FullName, format);

			if (File.Exists(outputPath))
			{
				string message =
					$"Skipping {inputFile.Name} - output file already exists";
				Log.Warn(message);
			}
			else
			{
				string arguments =
					GetFfmpegArguments(inputFile.FullName, outputPath);

				Log.Info($"Converting: {inputFile.Name}");
				ExternalProcess process = new();

				bool result = await process.ExecuteAsync("ffmpeg", arguments).
					ConfigureAwait(false);

				if (result == true)
				{
					if (batchMode == true)
					{
						successCount++;
					}
				}
				else
				{
					string error = process.Output;
					string message =
						$"Error converting {inputFile.Name}: {error}";
					Log.Error(message);
				}
			}
		}
	}

	/// <summary>
	/// Asynchronously converts all WMA audio files in the specified directory
	/// to the target format.
	/// </summary>
	/// <remarks>FFmpeg must be installed and available on the system for the
	/// conversion to proceed. If no WMA files are found in the specified
	/// directory, the method completes without performing any conversions. The
	/// search includes subdirectories if recursive processing is enabled.
	/// </remarks>
	/// <param name="inputDirectory">The directory to search for WMA files to
	/// convert. All files with a .wma extension in this directory (and
	/// subdirectories, if enabled) will be processed.</param>
	/// <param name="fileTypes">A comma-separated list of file extensions
	/// (without dots) specifying which file types to convert. For example,
	/// "mp3, m4a".</param>
	/// <returns>A task that represents the asynchronous conversion operation.
	/// </returns>
	public async Task ConvertFilesAsync(
		DirectoryInfo inputDirectory, string fileTypes)
	{
		if (inputDirectory != null && fileTypes != null)
		{
			bool exists = CheckFfmpeg();

			if (exists == false)
			{
				string message =
					"FFmpeg is not installed. " +
					"Please install FFmpeg to use this application.";
				Log.Error(message);
			}
			else
			{
				IEnumerable<string> extensions = fileTypes.Split(
					',', StringSplitOptions.RemoveEmptyEntries)
					.Select(extension => extension.Trim());
				List<string> extensionsList = [.. extensions];

				try
				{
					ValidateFileExtensions(extensionsList);
				}
				catch (ArgumentException ex)
				{
					Log.Error(ex.Message);

					throw;
				}

				this.batchMode = true;
				string message = "Starting conversion process at "
					+ $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}";
				Log.Info(message);

				SearchOption searchOption = SearchOption.TopDirectoryOnly;

				if (recursive == true)
				{
					searchOption = SearchOption.AllDirectories;
				}

				IEnumerable<FileInfo> files = extensionsList
					.SelectMany(extension => inputDirectory.GetFiles(
						$"*.{extension}", searchOption))
					.Distinct();
				FileInfo[] convertFiles = [.. files];
				totalFiles = convertFiles.Length;

				if (totalFiles == 0)
				{
					Log.Error(
						$"No valid files found in {inputDirectory.FullName}");
				}
				else
				{
					Log.Info($"Found {totalFiles} WMA files to process");

					foreach (FileInfo file in convertFiles)
					{
						await ConvertFileAsync(file).ConfigureAwait(false);
						Log.Info($"Progress: {successCount} / {totalFiles}");
					}

					message = "Conversion completed: " +
						$"{successCount} / {totalFiles} " +
						"files converted successfully";
					Log.Info(message);
					string now = DateTime.UtcNow.ToString(
						"yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
					message = $"Process completed at {now}";
					Log.Info(message);
				}
			}
		}
	}

	private string GetFfmpegArguments(
		string inputPath, string outputPath)
	{
		Dictionary<string, string> flacSettings = new()
		{
			["high"] = "-c:a flac -compression_level 8",
			["medium"] = "-c:a flac -compression_level 5",
			["low"] = "-c:a flac -compression_level 3"
		};

		Dictionary<string, string> m4aSettings = new()
		{
			["high"] = "-c:a aac -b:a 256k",
			["medium"] = "-c:a aac -b:a 192k",
			["low"] = "-c:a aac -b:a 128k"
		};

		Dictionary<string, string> mp3Settings = new()
		{
			["high"] = "-c:a libmp3lame -b:a 320k",
			["medium"] = "-c:a libmp3lame -b:a 192k",
			["low"] = "-c:a libmp3lame -b:a 128k"
		};

		Dictionary<string, Dictionary<string, string>> qualitySettings =
			new()
			{
				["flac"] = flacSettings,
				["m4a"] = m4aSettings,
				["mp3"] = mp3Settings
			};

		string resultBegin = $"-i \"{inputPath}\" -map_metadata 0";
		string resultMiddle = $" {qualitySettings[format][quality]}";
		string resultEnd = $" \"{outputPath}\"";

		string result = resultBegin + resultMiddle + resultEnd;
		return result;
	}

	/// <summary>
	/// Validates that all specified file extensions are supported.
	/// </summary>
	/// <param name="extensions">A list of file extensions (without dots)
	/// specifying which file types to convert.</param>
	/// <returns>True if all extensions are valid; otherwise, false.</returns>
	/// <exception cref="ArgumentException">Thrown when fileTypes contains
	/// unsupported extensions.</exception>
	private bool ValidateFileExtensions(List<string> extensions)
	{
		IEnumerable<string> unsupported =
			extensions.Where(
				extension => !supportedFormats.Contains(extension));
		List<string> unsupportedList = [.. unsupported];

		if (unsupportedList.Count > 0)
		{
			string csvListUnsupported = string.Join(", ", unsupported);
			string csvListSupported = string.Join(", ", supportedFormats);
			string message =
				$"Unsupported file type(s): {csvListUnsupported}. " +
				$"Supported formats are: {csvListSupported}";

			throw new ArgumentException(message, nameof(extensions));
		}

		return true;
	}
}
