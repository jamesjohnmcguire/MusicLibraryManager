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
	private MediaFileFormat mediaFileFormat;
	private Mock<IMediaFileFormat> mockMediaFileFormat;

	/// <summary>
	/// The one time setup method.
	/// </summary>
	[OneTimeSetUp]
	public void OneTimeSetUp()
	{
		bool exists = FfmpegBase.CheckFfmpeg();

		if (exists == false)
		{
			Assert.Fail("FFmpeg is not available in PATH.");
		}

		// Created in base class.
		Assert.That(TestFiles.Count, Is.GreaterThan(0));
	}

	/// <summary>
	/// The setup before every test method.
	/// </summary>
	[SetUp]
	public void SetUp()
	{
		mockMediaFileFormat = new Mock<IMediaFileFormat>();
		mediaFileFormat = new MediaFileFormat(mockMediaFileFormat.Object);
	}

	/// <summary>
	/// The get audio type throws file not found exception on null test.
	/// </summary>
	[Test]
	public void GetAudioTypeThrowsFileNotFoundExceptionOnNull()
	{
		Assert.Throws<FileNotFoundException>(() =>
			mediaFileFormat.GetCompressionType("bad/file/path"));
	}

	[Test]
	public void GetAudioType_EmptyFilePath_ThrowsFileNotFoundException()
	{
		Assert.Throws<FileNotFoundException>(() => mediaFileFormat.GetCompressionType(""));
	}

	[Test]
	public void GetAudioType_WhitespaceFilePath_ThrowsFileNotFoundException()
	{
		Assert.Throws<FileNotFoundException>(() => mediaFileFormat.GetCompressionType("   "));
	}

	[Test]
	public void GetAudioType_NonExistentFile_ThrowsFileNotFoundException()
	{
		string nonExistentPath = Path.Combine(TemporaryPath, "nonexistent.mp3");
		Assert.Throws<FileNotFoundException>(() => mediaFileFormat.GetCompressionType(nonExistentPath));
	}

	[Test]
	public void GetAudioType_FileNotFound_ExceptionContainsFilePath()
	{
		string path = "missing.mp3";
		var ex = Assert.Throws<FileNotFoundException>(() => mediaFileFormat.GetCompressionType(path));
		Assert.That(ex.Message, Does.Contain(path));
	}

	[Test]
	[TestCase("AAC")]
	[TestCase("aac")]
	[TestCase("Aac")]
	public void GetAudioType_AacFile_ReturnsLossy(string extension)
	{
		var result = mediaFileFormat.GetCompressionType(TestFiles[extension]);
		Assert.That(result, Is.EqualTo(CompressionType.Lossy));
	}

	[Test]
	[TestCase("MP3")]
	[TestCase("mp3")]
	[TestCase("Mp3")]
	public void GetAudioType_Mp3File_ReturnsLossy(string extension)
	{
		var result = mediaFileFormat.GetCompressionType(TestFiles[extension]);
		Assert.That(result, Is.EqualTo(CompressionType.Lossy));
	}

	[Test]
	[TestCase("OPUS")]
	[TestCase("opus")]
	[TestCase("Opus")]
	public void GetAudioType_OpusFile_ReturnsLossy(string extension)
	{
		var result = mediaFileFormat.GetCompressionType(TestFiles[extension]);
		Assert.That(result, Is.EqualTo(CompressionType.Lossy));
	}

	[Test]
	public void GetAudioType_AllLossyFormats_ReturnLossy()
	{
		var lossyExtensions = new[] { "aac", "mp3", "opus" };

		foreach (var ext in lossyExtensions)
		{
			var result = mediaFileFormat.GetCompressionType(TestFiles[ext]);
			Assert.That(result, Is.EqualTo(CompressionType.Lossy), $"Expected {ext} to be Lossy");
		}
	}

	[Test]
	[TestCase("AIFF")]
	[TestCase("aiff")]
	[TestCase("Aiff")]
	public void GetAudioType_AiffFile_ReturnsLossless(string extension)
	{
		var result = mediaFileFormat.GetCompressionType(TestFiles[extension]);
		Assert.That(result, Is.EqualTo(CompressionType.Lossless));
	}

	[Test]
	[TestCase("APE")]
	[TestCase("ape")]
	[TestCase("Ape")]
	public void GetAudioType_ApeFile_ReturnsLossless(string extension)
	{
		var result = mediaFileFormat.GetCompressionType(TestFiles[extension]);
		Assert.That(result, Is.EqualTo(CompressionType.Lossless));
	}

	[Test]
	[TestCase("FLAC")]
	[TestCase("flac")]
	[TestCase("Flac")]
	public void GetAudioType_FlacFile_ReturnsLossless(string extension)
	{
		var result = mediaFileFormat.GetCompressionType(TestFiles[extension]);
		Assert.That(result, Is.EqualTo(CompressionType.Lossless));
	}

	[Test]
	[TestCase("TTA")]
	[TestCase("tta")]
	[TestCase("Tta")]
	public void GetAudioType_TtaFile_ReturnsLossless(string extension)
	{
		var result = mediaFileFormat.GetCompressionType(TestFiles[extension]);
		Assert.That(result, Is.EqualTo(CompressionType.Lossless));
	}

	[Test]
	[TestCase("WAV")]
	[TestCase("wav")]
	[TestCase("Wav")]
	public void GetAudioType_WavFile_ReturnsLossless(string extension)
	{
		var result = mediaFileFormat.GetCompressionType(TestFiles[extension]);
		Assert.That(result, Is.EqualTo(CompressionType.Lossless));
	}

	[Test]
	public void GetAudioType_AllLosslessFormats_ReturnLossless()
	{
		var losslessExtensions = new[] { "aiff", "ape", "flac", "tta", "wav" };

		foreach (var ext in losslessExtensions)
		{
			var result = mediaFileFormat.GetCompressionType(TestFiles[ext]);
			Assert.That(result, Is.EqualTo(CompressionType.Lossless), $"Expected {ext} to be Lossless");
		}
	}

	[Test]
	public void GetAudioType_M4aFile_CallsGetAudioTypeM4a()
	{
		mockMediaFileFormat.Setup(m => m.GetCompressionTypeM4a(It.IsAny<string>()))
			.Returns(CompressionType.Lossy);

		mediaFileFormat.GetCompressionType(TestFiles["m4a"]);

		mockMediaFileFormat.Verify(m => m.GetCompressionTypeM4a(TestFiles["m4a"]), Times.Once);
	}

	[Test]
	public void GetAudioType_M4aFile_ReturnsLossy_WhenInterfaceReturnsLossy()
	{
		mockMediaFileFormat.Setup(m => m.GetCompressionTypeM4a(It.IsAny<string>()))
			.Returns(CompressionType.Lossy);

		var result = mediaFileFormat.GetCompressionType(TestFiles["m4a"]);

		Assert.That(result, Is.EqualTo(CompressionType.Lossy));
	}

	[Test]
	public void GetAudioType_M4aFile_ReturnsLossless_WhenInterfaceReturnsLossless()
	{
		mockMediaFileFormat.Setup(m => m.GetCompressionTypeM4a(It.IsAny<string>()))
			.Returns(CompressionType.Lossless);

		var result = mediaFileFormat.GetCompressionType(TestFiles["m4a"]);

		Assert.That(result, Is.EqualTo(CompressionType.Lossless));
	}

	[Test]
	public void GetAudioType_M4aFile_PassesCorrectFilePath()
	{
		string capturedPath = null;
		mockMediaFileFormat.Setup(m => m.GetCompressionTypeM4a(It.IsAny<string>()))
			.Callback<string>(path => capturedPath = path)
			.Returns(CompressionType.Lossy);

		mediaFileFormat.GetCompressionType(TestFiles["m4a"]);

		Assert.That(capturedPath, Is.EqualTo(TestFiles["m4a"]));
	}

	[Test]
	public void GetAudioType_MkaFile_CallsGetAudioTypeMka()
	{
		mockMediaFileFormat.Setup(m => m.GetCompressionTypeMka(It.IsAny<string>()))
			.Returns(CompressionType.Lossy);

		mediaFileFormat.GetCompressionType(TestFiles["mka"]);

		mockMediaFileFormat.Verify(m => m.GetCompressionTypeMka(TestFiles["mka"]), Times.Once);
	}

	[Test]
	public void GetAudioType_MkaFile_ReturnsLossy_WhenInterfaceReturnsLossy()
	{
		mockMediaFileFormat.Setup(m => m.GetCompressionTypeMka(It.IsAny<string>()))
			.Returns(CompressionType.Lossy);

		var result = mediaFileFormat.GetCompressionType(TestFiles["mka"]);

		Assert.That(result, Is.EqualTo(CompressionType.Lossy));
	}

	[Test]
	public void GetAudioType_MkaFile_ReturnsLossless_WhenInterfaceReturnsLossless()
	{
		mockMediaFileFormat.Setup(m => m.GetCompressionTypeMka(It.IsAny<string>()))
			.Returns(CompressionType.Lossless);

		var result = mediaFileFormat.GetCompressionType(TestFiles["mka"]);

		Assert.That(result, Is.EqualTo(CompressionType.Lossless));
	}

	[Test]
	public void GetAudioType_OggFile_CallsGetAudioTypeOgg()
	{
		mockMediaFileFormat.Setup(m => m.GetCompressionTypeOgg(It.IsAny<string>()))
			.Returns(CompressionType.Lossy);

		mediaFileFormat.GetCompressionType(TestFiles["ogg"]);

		mockMediaFileFormat.Verify(m => m.GetCompressionTypeOgg(TestFiles["ogg"]), Times.Once);
	}

	[Test]
	public void GetAudioType_OggFile_ReturnsLossy_WhenInterfaceReturnsLossy()
	{
		mockMediaFileFormat.Setup(m => m.GetCompressionTypeOgg(It.IsAny<string>()))
			.Returns(CompressionType.Lossy);

		var result = mediaFileFormat.GetCompressionType(TestFiles["ogg"]);

		Assert.That(result, Is.EqualTo(CompressionType.Lossy));
	}

	[Test]
	public void GetAudioType_OggFile_ReturnsLossless_WhenInterfaceReturnsLossless()
	{
		mockMediaFileFormat.Setup(m => m.GetCompressionTypeOgg(It.IsAny<string>()))
			.Returns(CompressionType.Lossless);

		var result = mediaFileFormat.GetCompressionType(TestFiles["ogg"]);

		Assert.That(result, Is.EqualTo(CompressionType.Lossless));
	}

	[Test]
	public void GetAudioType_WmaFile_CallsGetAudioTypeWma()
	{
		mockMediaFileFormat.Setup(m => m.GetCompressionTypeWma(It.IsAny<string>()))
			.Returns(CompressionType.Lossy);

		mediaFileFormat.GetCompressionType(TestFiles["wma"]);

		mockMediaFileFormat.Verify(m => m.GetCompressionTypeWma(TestFiles["wma"]), Times.Once);
	}

	[Test]
	public void GetAudioType_WmaFile_ReturnsLossy_WhenInterfaceReturnsLossy()
	{
		mockMediaFileFormat.Setup(m => m.GetCompressionTypeWma(It.IsAny<string>()))
			.Returns(CompressionType.Lossy);

		var result = mediaFileFormat.GetCompressionType(TestFiles["wma"]);

		Assert.That(result, Is.EqualTo(CompressionType.Lossy));
	}

	[Test]
	public void GetAudioType_WmaFile_ReturnsLossless_WhenInterfaceReturnsLossless()
	{
		mockMediaFileFormat.Setup(m => m.GetCompressionTypeWma(It.IsAny<string>()))
			.Returns(CompressionType.Lossless);

		var result = mediaFileFormat.GetCompressionType(TestFiles["wma"]);

		Assert.That(result, Is.EqualTo(CompressionType.Lossless));
	}

	[Test]
	public void GetAudioType_WvFile_CallsGetAudioTypeWavPack()
	{
		mockMediaFileFormat.Setup(m => m.GetCompressionTypeWavPack(It.IsAny<string>()))
			.Returns(CompressionType.Lossless);

		mediaFileFormat.GetCompressionType(TestFiles["wv"]);

		mockMediaFileFormat.Verify(m => m.GetCompressionTypeWavPack(TestFiles["wv"]), Times.Once);
	}

	[Test]
	public void GetAudioType_WvFile_ReturnsLossy_WhenInterfaceReturnsLossy()
	{
		mockMediaFileFormat.Setup(m => m.GetCompressionTypeWavPack(It.IsAny<string>()))
			.Returns(CompressionType.Lossy);

		var result = mediaFileFormat.GetCompressionType(TestFiles["wv"]);

		Assert.That(result, Is.EqualTo(CompressionType.Lossy));
	}

	[Test]
	public void GetAudioType_WvFile_ReturnsLossless_WhenInterfaceReturnsLossless()
	{
		mockMediaFileFormat.Setup(m => m.GetCompressionTypeWavPack(It.IsAny<string>()))
			.Returns(CompressionType.Lossless);

		var result = mediaFileFormat.GetCompressionType(TestFiles["wv"]);

		Assert.That(result, Is.EqualTo(CompressionType.Lossless));
	}

	[Test]
	public void GetAudioType_UnknownExtension_ReturnsUnknown()
	{
		string unknownFile = Path.Combine(TemporaryPath, "test.xyz");
		File.WriteAllText(unknownFile, "dummy");

		var result = mediaFileFormat.GetCompressionType(unknownFile);

		Assert.That(result, Is.EqualTo(CompressionType.Unknown));
	}

	[Test]
	public void GetAudioType_NoExtension_ReturnsUnknown()
	{
		string noExtFile = Path.Combine(TemporaryPath, "testfile");
		File.WriteAllText(noExtFile, "dummy");

		var result = mediaFileFormat.GetCompressionType(noExtFile);

		Assert.That(result, Is.EqualTo(CompressionType.Unknown));
	}

	[Test]
	[TestCase("mp4")]
	[TestCase("avi")]
	[TestCase("mkv")]
	[TestCase("txt")]
	[TestCase("doc")]
	public void GetAudioType_NonAudioExtensions_ReturnsUnknown(string extension)
	{
		string file = Path.Combine(TemporaryPath, $"test{extension}");
		File.WriteAllText(file, "dummy");

		var result = mediaFileFormat.GetCompressionType(file);

		Assert.That(result, Is.EqualTo(CompressionType.Unknown));
	}

	[Test]
	[TestCase("mp3", CompressionType.Lossy)]
	[TestCase("MP3", CompressionType.Lossy)]
	[TestCase("Mp3", CompressionType.Lossy)]
	[TestCase("mP3", CompressionType.Lossy)]
	[TestCase("flac", CompressionType.Lossless)]
	[TestCase("FLAC", CompressionType.Lossless)]
	[TestCase("Flac", CompressionType.Lossless)]
	[TestCase("fLaC", CompressionType.Lossless)]
	public void GetAudioType_CaseInsensitiveExtensions_ReturnsCorrectType(string extension, CompressionType expectedType)
	{
		string file = Path.Combine(TemporaryPath, $"casetest.{extension}");
		File.WriteAllText(file, "dummy");

		var result = mediaFileFormat.GetCompressionType(file);

		Assert.That(result, Is.EqualTo(expectedType));
	}

	[Test]
	public void GetAudioType_CalledMultipleTimes_DoesNotCacheResults()
	{
		mockMediaFileFormat.SetupSequence(m => m.GetCompressionTypeM4a(It.IsAny<string>()))
			.Returns(CompressionType.Lossy)
			.Returns(CompressionType.Lossless);

		var result1 = mediaFileFormat.GetCompressionType(TestFiles["M4A"]);
		var result2 = mediaFileFormat.GetCompressionType(TestFiles["M4A"]);

		Assert.That(result1, Is.EqualTo(CompressionType.Lossy));
		Assert.That(result2, Is.EqualTo(CompressionType.Lossless));
		mockMediaFileFormat.Verify(m => m.GetCompressionTypeM4a(It.IsAny<string>()), Times.Exactly(2));
	}

	[Test]
	public void GetAudioType_LossyFormat_DoesNotCallInterfaceMethods()
	{
		mediaFileFormat.GetCompressionType(TestFiles["MP3"]);

		mockMediaFileFormat.Verify(m => m.GetCompressionTypeM4a(It.IsAny<string>()), Times.Never);
		mockMediaFileFormat.Verify(m => m.GetCompressionTypeMka(It.IsAny<string>()), Times.Never);
		mockMediaFileFormat.Verify(m => m.GetCompressionTypeOgg(It.IsAny<string>()), Times.Never);
		mockMediaFileFormat.Verify(m => m.GetCompressionTypeWma(It.IsAny<string>()), Times.Never);
		mockMediaFileFormat.Verify(m => m.GetCompressionTypeWavPack(It.IsAny<string>()), Times.Never);
	}

	[Test]
	public void GetAudioType_LosslessFormat_DoesNotCallInterfaceMethods()
	{
		mediaFileFormat.GetCompressionType(TestFiles["FLAC"]);

		mockMediaFileFormat.Verify(m => m.GetCompressionTypeM4a(It.IsAny<string>()), Times.Never);
		mockMediaFileFormat.Verify(m => m.GetCompressionTypeMka(It.IsAny<string>()), Times.Never);
		mockMediaFileFormat.Verify(m => m.GetCompressionTypeOgg(It.IsAny<string>()), Times.Never);
		mockMediaFileFormat.Verify(m => m.GetCompressionTypeWma(It.IsAny<string>()), Times.Never);
		mockMediaFileFormat.Verify(m => m.GetCompressionTypeWavPack(It.IsAny<string>()), Times.Never);
	}

	/// <summary>
	/// The files exist test.
	/// </summary>
	[Test]
	public void TestFilesExist()
	{
		foreach (KeyValuePair<string, string> audioFile in TestFiles)
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
		foreach (KeyValuePair<string, string> audioFile in TestFiles)
		{
			FileInfo fileInfo = new(audioFile.Value);
			Assert.That(fileInfo.Length, Is.GreaterThan(0));
		}
	}

	/// <summary>
	/// The file can be read test.
	/// </summary>
	/// <param name="format">The audio file format.</param>
	[Test]
	[TestCaseSource(nameof(FileTypes))]
	public void TestFileCanBeRead(string format)
	{
		if (!TestFiles.ContainsKey(format))
		{
			Assert.Ignore($"Test file for {format} was not generated");
		}

		string filePath = TestFiles[format];

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
	[TestCaseSource(nameof(FileTypes))]
	public void TestFileHasCorrectExtension(string format)
	{
		if (!TestFiles.ContainsKey(format))
		{
			Assert.Ignore($"Test file for {format} was not generated");
		}

		string filePath = TestFiles[format];
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
		Dictionary<string, string>.KeyCollection keys = TestFiles.Keys;
		List<string> generatedFormats = [.. keys];

		foreach (string format in FileTypes)
		{
			Assert.That(generatedFormats, Does.Contain(format));
		}
	}

	/// <summary>
	/// The get audio properties test.
	/// </summary>
	/// <param name="format">The audio file format.</param>
	[Test]
	[TestCaseSource(nameof(FileTypes))]
	public void TestGetAudioProperties(string format)
	{
		if (!TestFiles.ContainsKey(format))
		{
			Assert.Ignore($"Test file for {format} was not generated");
		}

		string filePath = TestFiles[format];

		MediaStreamProperties? mediaStream =
			MediaFileFormatFfmpeg.ProcessFile(filePath);
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
}
