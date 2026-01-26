/////////////////////////////////////////////////////////////////////////////
// <copyright file="AudioFileFormatTests.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MusicToolKit.Tests;

using System;
using System.Collections.Generic;
using System.IO;
using MediaFileToolkit;
using Moq;
using NUnit.Framework;

/// <summary>
/// Contains unit tests for verifying audio file format detection and
/// properties using the MediaFileFormat class.
/// </summary>
/// <remarks>This test fixture covers scenarios for identifying audio
/// compression types, handling invalid or missing files, and validating file
/// properties across a variety of audio formats. It ensures that the
/// MediaFileFormat implementation correctly distinguishes between lossy,
/// lossless, and unknown formats, and that test audio files are present and
/// readable. The tests also verify that the correct interface methods are
/// called for specific formats and that results are not cached between calls.
/// </remarks>
[TestFixture]
internal sealed class AudioFileFormatTests : BaseTestsSupport
{
	private MediaFileFormat mediaFileFormatMocked;
	private Mock<IMediaFileFormat> mockMediaFileFormat;

	/// <summary>
	/// The one time setup method.
	/// </summary>
	[OneTimeSetUp]
	public void OneTimeSetUp()
	{
		CheckFfmpegExists();
	}

	/// <summary>
	/// The setup before every test method.
	/// </summary>
	[SetUp]
	public void SetUp()
	{
		mockMediaFileFormat = new Mock<IMediaFileFormat>();
		mediaFileFormatMocked =
			new MediaFileFormat(mockMediaFileFormat.Object);
	}

	/// <summary>
	/// Verifies that the GetCompressionType method throws an
	/// ArgumentNullException when passed a null argument.
	/// </summary>
	/// <remarks>This test ensures that the method enforces its null argument
	/// precondition by throwing the expected exception. Use this test to
	/// validate input checking behavior for the GetCompressionType method.
	/// </remarks>
	[Test]
	public void GetAudioTypeThrowsArgumentExceptionOnNull()
	{
		Assert.Throws<ArgumentNullException>(() =>
			mediaFileFormatMocked.GetCompressionType(null));
	}

	/// <summary>
	/// Verifies that GetCompressionType throws an ArgumentException when the
	/// specified file name is empty or consists only of whitespace.
	/// </summary>
	/// <remarks>This test ensures that GetCompressionType enforces input
	/// validation by throwing an ArgumentException for invalid file name
	/// values.</remarks>
	/// <param name="fileName">The file name to test. Must be either an empty
	/// string or contain only whitespace characters.</param>
	[TestCase("")]
	[TestCase("    ")]
	public void GetAudioTypeThrowsArgumentExceptionOnEmpty(string fileName)
	{
		Assert.Throws<ArgumentException>(() =>
			mediaFileFormatMocked.GetCompressionType(fileName));
	}

	/// <summary>
	/// Verifies that calling GetCompressionType with a non-existent file path
	/// throws a FileNotFoundException.
	/// </summary>
	/// <remarks>Use this test to ensure that the GetCompressionType method
	/// correctly handles missing files by throwing the expected exception.
	/// </remarks>
	/// <param name="fileName">The path to the audio file to test. Must refer
	/// to a file that does not exist.</param>
	[TestCase("bad/file/path")]
	[Test]
	public void GetAudioTypeThrowsFileNotFound(string fileName)
	{
		Assert.Throws<FileNotFoundException>(() =>
			mediaFileFormatMocked.GetCompressionType(fileName));
	}

	/// <summary>
	/// Verifies that the GetCompressionType method throws a
	/// FileNotFoundException when called with a path to a non-existent audio
	/// file.
	/// </summary>
	/// <remarks>This test ensures that the method correctly handles invalid
	/// file paths by throwing the expected exception, helping to validate
	/// error handling behavior.</remarks>
	[Test]
	public void GetAudioTypeThrowsFileNotFoundNonExistentFile()
	{
		string nonExistentPath =
			Path.Combine(TemporaryPath, "nonexistent.mp3");
		Assert.Throws<FileNotFoundException>(() =>
			mediaFileFormatMocked.GetCompressionType(nonExistentPath));
	}

	/// <summary>
	/// Verifies that GetCompressionType throws a FileNotFoundException
	/// containing the file path when the specified audio file does not exist.
	/// </summary>
	/// <remarks>This test ensures that the exception message includes the
	/// missing file's path, which helps callers identify which file could not
	/// be found.</remarks>
	[Test]
	public void GetAudioTypeExceptionContainsFilePathOnFileNotFound()
	{
		string path = "missing.mp3";
		FileNotFoundException? exception =
			Assert.Throws<FileNotFoundException>(() =>
				mediaFileFormatMocked.GetCompressionType(path));
		Assert.That(exception.Message, Does.Contain(path));
	}

	/// <summary>
	/// Verifies that files with AAC, MP3, or OPUS extensions are identified as
	/// using lossy compression.
	/// </summary>
	/// <remarks>This test ensures that the compression type detection logic
	/// correctly classifies common audio formats as lossy, regardless of the
	/// casing of the extension.</remarks>
	/// <param name="extension">The file extension to test. Supported values
	/// are "AAC", "MP3", and "OPUS" (case-insensitive).</param>
	[Test]
	[TestCase("AAC")]
	[TestCase("Aac")]
	[TestCase("aac")]
	[TestCase("MP3")]
	[TestCase("Mp3")]
	[TestCase("mp3")]
	[TestCase("OPUS")]
	[TestCase("Opus")]
	[TestCase("opus")]
	public void GetAudioTypeAacReturnsLossy(string extension)
	{
		string filePath = TestFiles[extension];

		CheckCompressionType(
			mediaFileFormatMocked,
			filePath,
			CompressionType.Lossy);
	}

	/// <summary>
	/// Verifies that files with lossy audio formats return the expected lossy
	/// compression type.
	/// </summary>
	/// <remarks>This test checks multiple common lossy audio file extensions
	/// to ensure that the compression type detection logic correctly
	/// identifies them as lossy formats. The test is intended to validate the
	/// behavior of the compression type detection for AAC, MP3, and Opus
	/// files.</remarks>
	[Test]
	public void GetAudioTypeAllLossyFormatsReturnLossy()
	{
		string[] lossyExtensions = new[] { "aac", "mp3", "opus" };

		foreach (string? extension in lossyExtensions)
		{
			string filePath = TestFiles[extension];

			CheckCompressionType(
				mediaFileFormatMocked,
				filePath,
				CompressionType.Lossy);
		}
	}

	/// <summary>
	/// Verifies that the audio type determined by file extension is classified
	/// as lossless.
	/// </summary>
	/// <remarks>This test ensures that files with the specified extension are
	/// recognized as using a lossless compression type. Supported extensions
	/// include AIFF, APE, FLAC, TTA, and WAV in any casing.</remarks>
	/// <param name="extension">The file extension to test. The value is
	/// case-insensitive and should correspond to a supported lossless audio
	/// format.</param>
	[Test]
	[TestCase("AIFF")]
	[TestCase("Aiff")]
	[TestCase("aiff")]
	[TestCase("APE")]
	[TestCase("Ape")]
	[TestCase("ape")]
	[TestCase("FLAC")]
	[TestCase("Flac")]
	[TestCase("flac")]
	[TestCase("TTA")]
	[TestCase("Tta")]
	[TestCase("tta")]
	[TestCase("WAV")]
	[TestCase("Wav")]
	[TestCase("wav")]
	public void GetAudioTypeReturnsLossless(string extension)
	{
		string filePath = TestFiles[extension];

		CheckCompressionType(
			mediaFileFormatMocked,
			filePath,
			CompressionType.Lossless);
	}

	/// <summary>
	/// Verifies that all supported lossless audio file formats are
	/// correctly identified as lossless compression types.
	/// </summary>
	/// <remarks>This test iterates through a set of known lossless
	/// audio file extensions and asserts that each is recognized as having a
	/// lossless compression type. Use this test to ensure that the audio type
	/// detection logic remains accurate when new formats are added or existing
	/// formats are modified.</remarks>
	[Test]
	public void GetAudioTypeAllLosslessFormatsReturnLossless()
	{
		var losslessExtensions = new[] { "aiff", "ape", "flac", "tta", "wav" };

		foreach (var extension in losslessExtensions)
		{
			string filePath = TestFiles[extension];

			CheckCompressionType(
				mediaFileFormatMocked,
				filePath,
				CompressionType.Lossless);
		}
	}

	/// <summary>
	/// Verifies that calling GetCompressionType with an M4A file path invokes
	/// GetCompressionTypeM4a on the mocked IMediaFileFormat instance.
	/// </summary>
	/// <remarks>This test ensures that the GetCompressionType method correctly
	/// delegates to the M4A-specific compression type method when provided
	/// with an M4A file. The verification checks that GetCompressionTypeM4a is
	/// called exactly once with the expected file path.</remarks>
	[Test]
	public void GetAudioTypeMockCallsGetAudioTypeM4a()
	{
		string filePath = TestFiles["m4a"];

		mockMediaFileFormat.Setup(
			m => m.GetCompressionTypeM4a(It.IsAny<string>()))
			.Returns(CompressionType.Lossy);

		mediaFileFormatMocked.GetCompressionType(filePath);

		mockMediaFileFormat.Verify(
			m => m.GetCompressionTypeM4a(filePath), Times.Once);
	}

	/// <summary>
	/// Verifies that the audio type for an M4A file is correctly identified as
	/// lossy when the interface returns a lossy compression type.
	/// </summary>
	/// <remarks>This test sets up the mock interface to return a lossy
	/// compression type for an M4A file and asserts that the system under test
	/// recognizes the file as lossy. Use this test to ensure correct behavior
	/// when handling M4A files with lossy compression.</remarks>
	[Test]
	public void GetAudioTypeM4aFileWhenInterfaceReturnsLossy()
	{
		string filePath = TestFiles["m4a"];

		mockMediaFileFormat.Setup(
			m => m.GetCompressionTypeM4a(It.IsAny<string>()))
			.Returns(CompressionType.Lossy);

		CheckCompressionType(
			mediaFileFormatMocked,
			filePath,
			CompressionType.Lossy);
	}

	/// <summary>
	/// Verifies that the audio type for an M4A file is correctly identified as
	/// lossless when the interface returns a lossless compression type.
	/// </summary>
	/// <remarks>This test sets up the mock interface to return a lossless
	/// compression type for M4A files and asserts that the system under test
	/// recognizes the file accordingly. Use this test to ensure correct
	/// handling of lossless M4A files in scenarios where compression type
	/// detection is critical.</remarks>
	[Test]
	public void GetAudioTypeM4aFileWhenInterfaceReturnsLossless()
	{
		string filePath = TestFiles["m4a"];

		mockMediaFileFormat.Setup(
			m => m.GetCompressionTypeM4a(It.IsAny<string>()))
			.Returns(CompressionType.Lossless);

		CheckCompressionType(
			mediaFileFormatMocked,
			filePath,
			CompressionType.Lossless);
	}

	/// <summary>
	/// Verifies that the GetCompressionType method passes the correct file
	/// path to the underlying M4A compression type handler.
	/// </summary>
	/// <remarks>This test ensures that when an M4A file path is provided, the
	/// method under test forwards the exact path to the dependency responsible
	/// for determining the compression type. Use this test to confirm correct
	/// parameter forwarding in scenarios involving M4A audio files.</remarks>
	[Test]
	public void GetAudioTypeM4aFilePassesCorrectFilePath()
	{
		string filePath = TestFiles["m4a"];
		string? capturedPath = null;

		mockMediaFileFormat.Setup(
			m => m.GetCompressionTypeM4a(It.IsAny<string>()))
			.Callback<string>(path => capturedPath = path)
			.Returns(CompressionType.Lossy);

		mediaFileFormatMocked.GetCompressionType(filePath);

		Assert.That(capturedPath, Is.EqualTo(filePath));
	}

	/// <summary>
	/// Verifies that the GetCompressionType method calls GetCompressionTypeMka
	/// on the mock object with the expected file path when processing an MKA
	/// audio file.
	/// </summary>
	/// <remarks>This test ensures that the GetCompressionType method delegates
	/// to the correct mock method for MKA files and that the method is invoked
	/// exactly once. Use this test to confirm integration between the media
	/// file format abstraction and its mock implementation.</remarks>
	[Test]
	public void GetAudioTypeMockCallsGetAudioTypeMka()
	{
		string filePath = TestFiles["mka"];

		mockMediaFileFormat.Setup(
			m => m.GetCompressionTypeMka(It.IsAny<string>()))
			.Returns(CompressionType.Lossy);

		mediaFileFormatMocked.GetCompressionType(filePath);

		mockMediaFileFormat.Verify(
			m => m.GetCompressionTypeMka(filePath), Times.Once);
	}

	/// <summary>
	/// Verifies that the audio type for an MKA file is identified as lossy
	/// when the interface returns a lossy compression type.
	/// </summary>
	/// <remarks>This test configures the mock interface to return a lossy
	/// compression type for MKA files and asserts that the system under test
	/// correctly recognizes the file as lossy. Use this test to ensure correct
	/// handling of lossy audio formats in scenarios involving MKA files.
	/// </remarks>
	[Test]
	public void GetAudioTypeMkaFileWhenInterfaceReturnsLossy()
	{
		string filePath = TestFiles["mka"];

		mockMediaFileFormat.Setup(
			m => m.GetCompressionTypeMka(It.IsAny<string>()))
			.Returns(CompressionType.Lossy);

		CheckCompressionType(
			mediaFileFormatMocked,
			filePath,
			CompressionType.Lossy);
	}

	/// <summary>
	/// Verifies that the audio type for an MKA file is identified as lossless
	/// when the interface returns a lossless compression type.
	/// </summary>
	/// <remarks>This test ensures that the system correctly interprets the
	/// compression type returned by the interface for MKA files. It is
	/// intended to validate integration between the file format detection
	/// logic and the compression type interface.</remarks>
	[Test]
	public void GetAudioTypeMkaFileWhenInterfaceReturnsLossless()
	{
		string filePath = TestFiles["mka"];

		mockMediaFileFormat.Setup(
			m => m.GetCompressionTypeMka(It.IsAny<string>()))
			.Returns(CompressionType.Lossless);

		CheckCompressionType(
			mediaFileFormatMocked,
			filePath,
			CompressionType.Lossless);
	}

	/// <summary>
	/// Verifies that the GetCompressionType method calls GetCompressionTypeOgg
	/// with the correct file path when processing an Ogg audio file.
	/// </summary>
	/// <remarks>This test ensures that the mocked media file format
	/// implementation correctly delegates to the Ogg-specific compression type
	/// method when an Ogg file is provided. Use this test to validate
	/// integration between the general compression type logic and the Ogg
	/// -specific handler.</remarks>
	[Test]
	public void GetAudioTypeMockCallsGetAudioTypeOgg()
	{
		string filePath = TestFiles["ogg"];

		mockMediaFileFormat.Setup(
			m => m.GetCompressionTypeOgg(It.IsAny<string>()))
			.Returns(CompressionType.Lossy);

		mediaFileFormatMocked.GetCompressionType(filePath);

		mockMediaFileFormat.Verify(
			m => m.GetCompressionTypeOgg(filePath), Times.Once);
	}

	/// <summary>
	/// Verifies that the audio type for an Ogg file is identified as lossy
	/// when the interface returns a lossy compression type.
	/// </summary>
	/// <remarks>This test configures the mock interface to return a lossy
	/// compression type for Ogg files and asserts that the system correctly
	/// recognizes the file as lossy. Use this test to ensure that Ogg file
	/// handling aligns with expected compression type detection behavior.
	/// </remarks>
	[Test]
	public void GetAudioTypeOggFileWhenInterfaceReturnsLossy()
	{
		string filePath = TestFiles["ogg"];

		mockMediaFileFormat.Setup(
			m => m.GetCompressionTypeOgg(It.IsAny<string>()))
			.Returns(CompressionType.Lossy);

		CheckCompressionType(
			mediaFileFormatMocked,
			filePath,
			CompressionType.Lossy);
	}

	/// <summary>
	/// Verifies that the audio type for an Ogg file is identified as lossless
	/// when the interface returns a lossless compression type.
	/// </summary>
	/// <remarks>This test ensures that the system correctly interprets the
	/// compression type returned by the interface for Ogg files. It is
	/// intended to validate the mapping between the interface's compression
	/// type and the expected audio type result.</remarks>
	[Test]
	public void GetAudioTypeOggFileWhenInterfaceReturnsLossless()
	{
		string filePath = TestFiles["ogg"];

		mockMediaFileFormat.Setup(
			m => m.GetCompressionTypeOgg(It.IsAny<string>()))
			.Returns(CompressionType.Lossless);

		CheckCompressionType(
			mediaFileFormatMocked,
			filePath,
			CompressionType.Lossless);
	}

	/// <summary>
	/// Verifies that the GetCompressionType method calls GetCompressionTypeWma
	/// when provided with a WMA file path.
	/// </summary>
	/// <remarks>This test ensures that the media file format mock correctly
	/// delegates WMA file compression type requests to the
	/// GetCompressionTypeWma method. It uses a mock setup and verification to
	/// confirm the expected interaction occurs exactly once.</remarks>
	[Test]
	public void GetAudioTypeMockCallsGetAudioTypeWma()
	{
		string filePath = TestFiles["wma"];

		mockMediaFileFormat.Setup(
			m => m.GetCompressionTypeWma(It.IsAny<string>()))
			.Returns(CompressionType.Lossy);

		mediaFileFormatMocked.GetCompressionType(TestFiles["wma"]);

		mockMediaFileFormat.Verify(
			m => m.GetCompressionTypeWma(TestFiles["wma"]), Times.Once);
	}

	/// <summary>
	/// Verifies that the audio type for a WMA file is correctly identified as
	/// lossy when the interface returns a lossy compression type.
	/// </summary>
	/// <remarks>This test sets up the mock interface to return a lossy
	/// compression type for a WMA file and asserts that the system under test
	/// recognizes the file as lossy. Use this test to ensure correct behavior
	/// when handling WMA files with lossy compression.</remarks>
	[Test]
	public void GetAudioTypeWmaFileWhenInterfaceReturnsLossy()
	{
		string filePath = TestFiles["wma"];

		mockMediaFileFormat.Setup(m => m.GetCompressionTypeWma(
			It.IsAny<string>()))
			.Returns(CompressionType.Lossy);

		CheckCompressionType(
			mediaFileFormatMocked,
			filePath,
			CompressionType.Lossy);
	}

	/// <summary>
	/// Verifies that the audio type for a WMA file is correctly identified as
	/// lossless when the interface returns a lossless compression type.
	/// </summary>
	/// <remarks>This test ensures that the system correctly interprets the
	/// compression type returned by the interface for WMA files. It is
	/// intended to validate the behavior when the compression type is set to
	/// lossless, helping to prevent regressions in audio type detection logic.
	/// </remarks>
	[Test]
	public void GetAudioTypeWmaFileWhenInterfaceReturnsLossless()
	{
		string filePath = TestFiles["wma"];

		mockMediaFileFormat.Setup(
			m => m.GetCompressionTypeWma(It.IsAny<string>()))
			.Returns(CompressionType.Lossless);

		CheckCompressionType(
			mediaFileFormatMocked,
			filePath,
			CompressionType.Lossless);
	}

	/// <summary>
	/// Verifies that the GetCompressionType method calls
	/// GetCompressionTypeWavPack with the correct file path when processing a
	/// WavPack audio file.
	/// </summary>
	/// <remarks>This test sets up a mock for GetCompressionTypeWavPack to
	/// return CompressionType.Lossless and ensures that the method is invoked
	/// exactly once with the expected file path. Use this test to confirm
	/// correct delegation for WavPack file type handling.</remarks>
	[Test]
	public void GetAudioTypeMockCallsGetAudioTypeWavPack()
	{
		string filePath = TestFiles["wv"];

		mockMediaFileFormat.Setup(
			m => m.GetCompressionTypeWavPack(It.IsAny<string>()))
			.Returns(CompressionType.Lossless);

		mediaFileFormatMocked.GetCompressionType(filePath);

		mockMediaFileFormat.Verify(
			m => m.GetCompressionTypeWavPack(
				filePath), Times.Once);
	}

	/// <summary>
	/// Get audio type wv file when interface returns lossy.
	/// </summary>
	[Test]
	public void GetAudioTypeWvFileWhenInterfaceReturnsLossy()
	{
		string filePath = TestFiles["wv"];

		mockMediaFileFormat.Setup(
			m => m.GetCompressionTypeWavPack(It.IsAny<string>()))
			.Returns(CompressionType.Lossy);

		CheckCompressionType(
			mediaFileFormatMocked,
			filePath,
			CompressionType.Lossy);
	}

	/// <summary>
	/// Get audio type wv file when interface returns lossless.
	/// </summary>
	[Test]
	public void GetAudioTypeWvFileWhenInterfaceReturnsLossless()
	{
		string filePath = TestFiles["wv"];

		mockMediaFileFormat.Setup(
			m => m.GetCompressionTypeWavPack(It.IsAny<string>()))
			.Returns(CompressionType.Lossless);

		CheckCompressionType(
			mediaFileFormatMocked,
			filePath,
			CompressionType.Lossless);
	}

	/// <summary>
	/// Get audio type when unknown extenstion returns unknown.
	/// </summary>
	[Test]
	public void GetAudioTypeUnknownExtensionReturnsUnknown()
	{
		string unknownFile = Path.Combine(TemporaryPath, "test.xyz");
		File.WriteAllText(unknownFile, "dummy");

		CheckCompressionType(
			mediaFileFormatMocked,
			unknownFile,
			CompressionType.Unknown);
	}

	/// <summary>
	/// Get audio type when no extenstion returns unknown.
	/// </summary>
	[Test]
	public void GetAudioTypeNoExtensionReturnsUnknown()
	{
		string noExtentionFile = Path.Combine(TemporaryPath, "testfile");
		File.WriteAllText(noExtentionFile, "dummy");

		CheckCompressionType(
			mediaFileFormatMocked,
			noExtentionFile,
			CompressionType.Unknown);
	}

	/// <summary>
	/// Verifies that GetAudioType returns CompressionType.Unknown for file
	/// extensions that are not associated with audio formats.
	/// </summary>
	/// <remarks>This test ensures that the method under test does not
	/// incorrectly identify non-audio file types as audio, and instead returns
	/// CompressionType.Unknown as expected.</remarks>
	/// <param name="extension">The file extension to test. This should be a
	/// non-audio extension such as "mp4", "avi", "mkv", "txt", or "doc".
	/// </param>
	[Test]
	[TestCase("mp4")]
	[TestCase("avi")]
	[TestCase("mkv")]
	[TestCase("txt")]
	[TestCase("doc")]
	public void GetAudioTypeNonAudioExtensionsReturnsUnknown(string extension)
	{
		string file = Path.Combine(TemporaryPath, $"test{extension}");
		File.WriteAllText(file, "dummy");

		CheckCompressionType(
			mediaFileFormatMocked,
			file,
			CompressionType.Unknown);
	}

	/// <summary>
	/// Verifies that audio file extensions are correctly mapped to their
	/// corresponding compression types, regardless of case sensitivity.
	/// </summary>
	/// <remarks>This test ensures that the method for determining audio
	/// compression type from file extensions functions correctly for various
	/// casing scenarios, such as 'mp3', 'MP3', 'Mp3', and 'mP3'.</remarks>
	/// <param name="extension">The file extension to test, representing an
	/// audio format. The comparison is performed in a case-insensitive manner.
	/// </param>
	/// <param name="expectedType">The expected compression type associated
	/// with the specified audio file extension.</param>
	[Test]
	[TestCase("mp3", CompressionType.Lossy)]
	[TestCase("MP3", CompressionType.Lossy)]
	[TestCase("Mp3", CompressionType.Lossy)]
	[TestCase("mP3", CompressionType.Lossy)]
	[TestCase("flac", CompressionType.Lossless)]
	[TestCase("FLAC", CompressionType.Lossless)]
	[TestCase("Flac", CompressionType.Lossless)]
	[TestCase("fLaC", CompressionType.Lossless)]
	public void GetAudioTypeCaseInsensitiveExtensions(
		string extension, CompressionType expectedType)
	{
		string file = Path.Combine(TemporaryPath, $"casetest.{extension}");
		File.WriteAllText(file, "dummy");

		CheckCompressionType(
			mediaFileFormatMocked,
			file,
			expectedType);
	}

	/// <summary>
	/// Verifies that repeated calls to GetCompressionType for the same M4A
	/// file do not return cached results and instead invoke the underlying
	/// method each time.
	/// </summary>
	/// <remarks>This test ensures that the GetCompressionType method does not
	/// cache its output for M4A files, and that each call reflects the current
	/// state of the underlying GetCompressionTypeM4a implementation. This
	/// behavior is important when the compression type may change between
	/// invocations or when accurate, up-to-date results are required.
	/// </remarks>
	[Test]
	public void GetAudioTypeDoesNotCacheResults()
	{
		mockMediaFileFormat.SetupSequence(
			m => m.GetCompressionTypeM4a(It.IsAny<string>()))
			.Returns(CompressionType.Lossy)
			.Returns(CompressionType.Lossless);

		CompressionType result1 =
			mediaFileFormatMocked.GetCompressionType(TestFiles["M4A"]);
		CompressionType result2 =
			mediaFileFormatMocked.GetCompressionType(TestFiles["M4A"]);

		Assert.That(result1, Is.EqualTo(CompressionType.Lossy));
		Assert.That(result2, Is.EqualTo(CompressionType.Lossless));

		mockMediaFileFormat.Verify(
			m => m.GetCompressionTypeM4a(
				It.IsAny<string>()), Times.Exactly(2));
	}

	/// <summary>
	/// Verifies that calling GetCompressionType for a lossy audio file does
	/// not invoke interface methods for other audio formats.
	/// </summary>
	/// <remarks>This test ensures that when processing an MP3 file, only the
	/// relevant compression type method is called, and methods for other
	/// formats such as M4A, MKA, OGG, WMA, and WavPack are not invoked. This
	/// helps confirm correct dispatching behavior and prevents unintended
	/// side effects.</remarks>
	[Test]
	public void GetAudioTypeLossyDoesNotCallInterfaceMethods()
	{
		string filePath = TestFiles["MP3"];

		mediaFileFormatMocked.GetCompressionType(filePath);

		mockMediaFileFormat.Verify(
			m => m.GetCompressionTypeM4a(It.IsAny<string>()), Times.Never);
		mockMediaFileFormat.Verify(
			m => m.GetCompressionTypeMka(It.IsAny<string>()), Times.Never);
		mockMediaFileFormat.Verify(
			m => m.GetCompressionTypeOgg(It.IsAny<string>()), Times.Never);
		mockMediaFileFormat.Verify(
			m => m.GetCompressionTypeWma(It.IsAny<string>()), Times.Never);
		mockMediaFileFormat.Verify(
			m => m.GetCompressionTypeWavPack(It.IsAny<string>()), Times.Never);
	}

	/// <summary>
	/// Verifies that retrieving the compression type for a FLAC file does not
	/// invoke interface methods for other audio formats.
	/// </summary>
	/// <remarks>This test ensures that when the compression type is requested
	/// for a lossless FLAC file, only the relevant method is called and
	/// methods for M4A, MKA, OGG, WMA, and WavPack formats are not invoked.
	/// This helps confirm correct dispatching behavior in the media file
	/// format implementation.</remarks>
	[Test]
	public void GetAudioTypeLosslessDoesNotCallInterfaceMethods()
	{
		string filePath = TestFiles["FLAC"];

		mediaFileFormatMocked.GetCompressionType(filePath);

		mockMediaFileFormat.Verify(
			m => m.GetCompressionTypeM4a(It.IsAny<string>()), Times.Never);
		mockMediaFileFormat.Verify(
			m => m.GetCompressionTypeMka(It.IsAny<string>()), Times.Never);
		mockMediaFileFormat.Verify(
			m => m.GetCompressionTypeOgg(It.IsAny<string>()), Times.Never);
		mockMediaFileFormat.Verify(
			m => m.GetCompressionTypeWma(It.IsAny<string>()), Times.Never);
		mockMediaFileFormat.Verify(
			m => m.GetCompressionTypeWavPack(It.IsAny<string>()), Times.Never);
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
		bool exists = TestFiles.ContainsKey(format);
		Assert.That(exists, Is.True);

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
		bool exists = TestFiles.ContainsKey(format);
		Assert.That(exists, Is.True);

		string filePath = TestFiles[format];
		string extension = Path.GetExtension(filePath);

		if (format.Equals("alac", StringComparison.OrdinalIgnoreCase))
		{
			extension = GetSecondToLastExtension(filePath);
		}

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
		bool exists = TestFiles.ContainsKey(format);
		Assert.That(exists, Is.True);

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
			double expectedMaxDuration = 1.1;

			if (format.Equals("ape", StringComparison.OrdinalIgnoreCase))
			{
				// APE file is slightly longer due to padding.
				expectedMaxDuration = 1.6;
			}

			Assert.That(duration, Is.InRange(0.9, expectedMaxDuration));
		}
	}
}
