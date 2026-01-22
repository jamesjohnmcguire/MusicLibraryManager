/////////////////////////////////////////////////////////////////////////////
// <copyright file="BaseTestsSupport.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MusicToolKit.Tests;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using DigitalZenWorks.Common.Utilities;
using DigitalZenWorks.RulesLibrary;
using MediaFileToolkit;
using NUnit.Framework;

/// <summary>
/// Base test support class.
/// </summary>
internal class BaseTestsSupport
{
	// Note: APE format generation is not currently supported.
	public static readonly Collection<string> FileTypes =
	[
		"aac", "aiff", "flac", "m4a", "mka", "mp3", "ogg",
		"opus", "tta", "wav", "wma", "wv"
	];

	private readonly AudioSettings audioSettings = new();
	private Rules rules;
	private string temporaryPath;
	private string testFile;

	/// <summary>
	/// Gets or sets the rules object.
	/// </summary>
	/// <value>The rules object.</value>
	public Rules Rules { get => rules; set => rules = value; }

	/// <summary>
	/// Gets the temporary path.
	/// </summary>
	/// <value>The temporary path.</value>
	public string TemporaryPath { get => temporaryPath; }

	/// <summary>
	/// Gets the test file.
	/// </summary>
	/// <value>The test file.</value>
	public string TestFile { get => testFile; }

	public Dictionary<string, string> TestFiles;

	/// <summary>
	/// The one time setup method.
	/// </summary>
	[OneTimeSetUp]
	public void BaseOneTimeSetUp()
	{
		temporaryPath = CreateUniqueDirectory();

		testFile = temporaryPath + @"\Music\Artist\Album\Sakura.mp4";

		FileUtils.CreateFileFromEmbeddedResource(
			"DigitalZenWorks.MusicToolKit.Tests.Sakura.mp4", testFile);

		rules = MusicManager.GetDefaultRules();

		TestFiles = GenerateAudioFiles();
	}

	/// <summary>
	/// One time tear down method.
	/// </summary>
	[OneTimeTearDown]
	public void BaseOneTimeTearDown()
	{
		bool result = Directory.Exists(temporaryPath);

		if (result == true)
		{
			Directory.Delete(temporaryPath, true);
		}
	}

	protected string? GenerateAudioFile(string format)
	{
		string? audioFile = null;
		string outputPath = Path.Combine(TemporaryPath, $"test.{format}");

		string arguments = GetArguments(format, outputPath);

		ExternalProcess process = new();

		bool result = process.Execute("ffmpeg", arguments);
		bool exists = File.Exists(outputPath);

		Assert.Multiple(() =>
		{
			Assert.That(result, Is.True);
			Assert.That(exists, Is.True);
		});

		if (result == true && exists == true)
		{
			audioFile = outputPath;
		}

		return audioFile;
	}

	/// <summary>
	/// Make test file copy.
	/// </summary>
	/// <param name="directory">The directory to create.</param>
	/// <param name="fileName">The file name to create.</param>
	/// <returns>The file path of the copy file.</returns>
	protected string MakeTestFileCopy(string directory, string fileName)
	{
		string newPath = temporaryPath + directory;
		Directory.CreateDirectory(newPath);

		string newFileName = newPath + @"\" + fileName;

		if (File.Exists(newFileName))
		{
			File.Delete(newFileName);
		}

		File.Copy(testFile, newFileName);

		return newFileName;
	}

	private static string CreateUniqueDirectory()
	{
		string guid = Guid.NewGuid().ToString("N");
		string uniqueDirectoryName = "MusicManTests-" + guid;
		string temporaryPath = Path.GetTempPath();
		string uniqueDirectoryPath =
			Path.Combine(temporaryPath, uniqueDirectoryName);
		Directory.CreateDirectory(uniqueDirectoryPath);

		return uniqueDirectoryPath;
	}

	private Dictionary<string, string> GenerateAudioFiles()
	{
		Dictionary<string, string> testFiles =
			new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

		foreach (string fileType in FileTypes)
		{
			string? filePath = GenerateAudioFile(fileType);

			if (filePath != null)
			{
				testFiles[fileType] = filePath;
			}
		}

		return testFiles;
	}

	private string GetArguments(string format, string outputPath)
	{
		string baseArguments = "-f lavfi -i " +
			$"\"sine=frequency={audioSettings.Frequency}:" +
			$"duration={audioSettings.Duration}\"";

		string? codec = format switch
		{
			"aac" => "-codec:a aac -b:a 32k",
			"aiff" => "-codec:a pcm_s16be",

			// APE not available, not supported yet
			// "ape" => "-codec:a ape -compression_level 2000",
			// Use FLAC as lossless alternative
			// "ape" => "-codec:a flac -compression_level 5",
			"flac" => "-codec:a flac",
			"m4a" => "-codec:a aac -b:a 32k",
			"mka" => "-codec:a libvorbis -b:a 32k",
			"mp3" => "-codec:a libmp3lame -b:a 32k",
			"ogg" => "-codec:a libvorbis -b:a 32k",
			"opus" => "-codec:a libopus -b:a 32k",
			"tta" => "-codec:a tta",
			"wav" => "-codec:a pcm_s16le",
			"wma" => "-codec:a wmav2 -b:a 32k",
			"wv" => "-codec:a wavpack",
			_ => null
		};

		string arguments = $"{baseArguments} {codec} \"{outputPath}\"";

		return arguments;
	}
}
