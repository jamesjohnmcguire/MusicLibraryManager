/////////////////////////////////////////////////////////////////////////////
// <copyright file="AudioFileFormatTests.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MusicToolKit.Tests;

using MediaFileToolkit;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

/// <summary>
/// Audio file format tests class.
/// </summary>
[TestFixture]
internal sealed class AudioFileFormatTests : BaseTestsSupport
{
	private readonly AudioSettings audioSettings = new();
	private MediaFileFormat mediaFileFormat;
	private Mock<IMediaFileFormat> mockMediaFileFormat;

	// Note: APE format generation is not currently supported.
	private static readonly Collection<string> fileTypes = new()
	{
		"aac", "aiff", "flac", "m4a", "mka", "mp3", "ogg",
		"opus", "tta", "wav", "wma", "wv"
	};

	private Dictionary<string, string> testFiles;

	/// <summary>
	/// The one time setup method.
	/// </summary>
	[OneTimeSetUp]
	public void OneTimeSetUp()
	{
		testFiles = new Dictionary<string, string>();

		bool exists = FfmpegBase.CheckFfmpeg();

		if (exists == false)
		{
			Assert.Fail("FFmpeg is not available in PATH.");
		}

		foreach (string fileType in fileTypes)
		{
			string filePath = GenerateAudioFile(fileType);

			if (filePath != null)
			{
				testFiles[fileType] = filePath;
			}
		}

		Assert.That(testFiles.Count, Is.GreaterThan(0));
	}

	[SetUp]
	public void SetUp()
	{
		mockMediaFileFormat = new Mock<IMediaFileFormat>();
		mediaFileFormat = new MediaFileFormat(mockMediaFileFormat.Object);
	}

	[Test]
	public void GetAudioType_NullFilePath_ThrowsFileNotFoundException()
	{
		Assert.Throws<FileNotFoundException>(() =>
			mediaFileFormat.GetAudioType(null));
	}

	/// <summary>
	/// The files exist test.
	/// </summary>
	[Test]
	public void TestFilesExist()
	{
		foreach (KeyValuePair<string, string> audioFile in testFiles)
		{
			Assert.That(File.Exists(audioFile.Value), Is.True);
		}
	}

	/// <summary>
	/// The files have content test.
	/// </summary>
	[Test]
	public void TestFilesHaveContent()
	{
		foreach (KeyValuePair<string, string> audioFile in testFiles)
		{
			FileInfo fileInfo = new FileInfo(audioFile.Value);
			Assert.That(fileInfo.Length, Is.GreaterThan(0));
		}
	}

	/// <summary>
	/// The file can be read test.
	/// </summary>
	/// <param name="format">The audio file format.</param>
	[Test]
	[TestCaseSource(nameof(fileTypes))]
	public void TestFileCanBeRead(string format)
	{
		if (!testFiles.ContainsKey(format))
		{
			Assert.Ignore($"Test file for {format} was not generated");
		}

		string filePath = testFiles[format];

		Assert.DoesNotThrow(
			() =>
			{
				using FileStream stream = File.OpenRead(filePath);
				Assert.That(stream.CanRead, Is.True);
			},
			$"Could not read {format} file");
	}

	/// <summary>
	/// The file has correct extension test.
	/// </summary>
	/// <param name="format">The audio file format.</param>
	[Test]
	[TestCaseSource(nameof(fileTypes))]
	public void TestFileHasCorrectExtension(string format)
	{
		if (!testFiles.ContainsKey(format))
		{
			Assert.Ignore($"Test file for {format} was not generated");
		}

		string filePath = testFiles[format];
		string extension = Path.GetExtension(filePath);
		extension = extension.TrimStart('.');

		Assert.That(extension, Is.EqualTo(format));
	}

	/// <summary>
	/// Test all expected formats generated.
	/// </summary>
	[Test]
	public void TestAllExpectedFormatsGenerated()
	{
		Dictionary<string, string>.KeyCollection keys = testFiles.Keys;
		List<string> generatedFormats = keys.ToList<string>();

		foreach (string format in fileTypes)
		{
			Assert.That(generatedFormats, Does.Contain(format));
		}
	}

	/// <summary>
	/// The get audio properties test.
	/// </summary>
	/// <param name="format">The audio file format.</param>
	[Test]
	[TestCaseSource(nameof(fileTypes))]
	public void TestGetAudioProperties(string format)
	{
		if (!testFiles.ContainsKey(format))
		{
			Assert.Ignore($"Test file for {format} was not generated");
		}

		string filePath = testFiles[format];

		MediaStream? mediaStream = MediaFileFormatFfmpeg.ProcessFile(filePath);
		Assert.That(mediaStream, Is.Not.Null);

		Assert.That(mediaStream.Duration, Is.Not.Null.And.Not.Empty);

		bool result =
			double.TryParse(mediaStream.Duration, out double duration);
		Assert.That(result, Is.True);

		if (result == true)
		{
			Assert.That(duration, Is.InRange(0.9, 1.1));
		}
	}

	private string GetArguments(string format, string outputPath)
	{
		string baseArguments = "-f lavfi -i " +
			$"\"sine=frequency={audioSettings.Frequency}:" +
			$"duration={audioSettings.Duration}\"";

		string codec = format switch
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

	private string GenerateAudioFile(string format)
	{
		string audioFile = null;
		string outputPath = Path.Combine(TemporaryPath, $"test.{format}");

		string arguments = GetArguments(format, outputPath);

		ExternalProcess process = new();

		bool result = process.Execute("ffmpeg", arguments);
		bool exists = File.Exists(outputPath);

		Assert.That(result, Is.True);
		Assert.That(exists, Is.True);

		if (result == true && exists == true)
		{
			audioFile = outputPath;
		}

		return audioFile;
	}
}
