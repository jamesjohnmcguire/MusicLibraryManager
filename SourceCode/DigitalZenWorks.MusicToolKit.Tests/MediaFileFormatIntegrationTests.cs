/////////////////////////////////////////////////////////////////////////////
// <copyright file="MediaFileFormatIntegrationTests.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MusicToolKit.Tests;

using MediaFileToolkit;
using NUnit.Framework;
using System.Reflection.Metadata;

/// <summary>
/// Provides integration tests for verifying audio file format detection and
/// compression type classification across multiple media file format
/// implementations.
/// </summary>
/// <remarks>These tests validate that different implementations (such as
/// FFmpeg, NAudio, and TagLib) correctly identify the compression type (lossy,
/// lossless, or unknown) for a variety of real audio files. The tests require
/// access to specific test files for each format; if a required file is
/// missing, the corresponding test is skipped. This class is intended for use
/// in end-to-end or integration testing scenarios to ensure consistency and
/// correctness of audio format handling.</remarks>
internal class MediaFileFormatIntegrationTests : BaseTestsSupport
{
	private MediaFileFormat mediaFileFormatWithFfmpeg;
	private MediaFileFormat mediaFileFormatWithNaudio;
	private MediaFileFormat mediaFileFormatWithTagLib;

	/// <summary>
	/// Performs one-time setup operations before any tests in the test fixture
	/// are run.
	/// </summary>
	/// <remarks>Use this method to initialize resources or dependencies that
	/// are shared across all tests in the fixture. This method is called once
	/// before any test methods are executed.</remarks>
	[OneTimeSetUp]
	public void OneTimeSetUp()
	{
		// Initialize with real implementations
		MediaFileFormatFfmpeg mediaFileFormatFfmpeg =
			new MediaFileFormatFfmpeg();
		mediaFileFormatWithFfmpeg = new MediaFileFormat(mediaFileFormatFfmpeg);

		MediaFileFormatNaudio mediaFileFormatNaudio =
			new MediaFileFormatNaudio();
		mediaFileFormatWithNaudio = new MediaFileFormat(mediaFileFormatNaudio);

		MediaFileFormatTagLib mediaFileFormatTagLib =
			new MediaFileFormatTagLib();
		mediaFileFormatWithTagLib = new MediaFileFormat(mediaFileFormatTagLib);
	}

	/// <summary>
	/// Verifies that all audio format detection implementations return the
	/// same compression type for common audio file formats.
	/// </summary>
	/// <remarks>This test ensures consistency between different libraries when
	/// identifying the compression type of simple audio formats such as MP3,
	/// WAV, and FLAC. A failure indicates a disagreement between
	/// implementations, which may affect audio processing reliability.
	/// </remarks>
	[Test]
	public void GetAudioTypeAllImplementationsAgreeOnSimpleFormats()
	{
		string[] testCases = new[] { "mp3", "wav", "flac" };

		foreach (var format in testCases)
		{
			string filePath = TestFiles[format];

			CheckFileExists(filePath);

			CompressionType resultFfmpeg =
				mediaFileFormatWithFfmpeg.GetCompressionType(filePath);
			CompressionType resultNaudio =
				mediaFileFormatWithNaudio.GetCompressionType(filePath);
			CompressionType resultTagLib =
				mediaFileFormatWithTagLib.GetCompressionType(filePath);

			Assert.That(
				resultFfmpeg,
				Is.EqualTo(resultNaudio),
				$"FFmpeg and NAudio disagree on {format}");
			Assert.That(
				resultNaudio,
				Is.EqualTo(resultTagLib),
				$"NAudio and TagLib disagree on {format}");
		}
	}

	/// <summary>
	/// Verifies that the compression type returned by FFmpeg for a given audio
	/// format matches the expected compression
	/// type.
	/// </summary>
	/// <remarks>This test ensures that the GetCompressionType method correctly
	/// identifies the compression type for various audio formats when using
	/// FFmpeg. It checks both lossy and lossless formats.</remarks>
	/// <param name="format">The audio file format to test, specified as a file
	/// extension (for example, "mp3" or "flac").</param>
	/// <param name="expectedType">The expected compression type for the
	/// specified audio format.</param>
	[Test]
	[TestCase("mp3", CompressionType.Lossy)]
	[TestCase("aac", CompressionType.Lossy)]
	[TestCase("opus", CompressionType.Lossy)]
	[TestCase("wav", CompressionType.Lossless)]
	[TestCase("flac", CompressionType.Lossless)]
	public void GetAudioTypeFFmpegCorrectType(
		string format, CompressionType expectedType)
	{
		string filePath = TestFiles[format];

		CheckFileExists(filePath);

		CheckCompressionType(
			mediaFileFormatWithFfmpeg,
			filePath,
			expectedType);
	}

	/// <summary>
	/// Verifies that the compression type detection for an M4A AAC lossy audio
	/// file returns the expected results for different media file format
	/// analyzers.
	/// </summary>
	/// <remarks>This test checks that the FFMpeg and TagLib analyzers
	/// correctly identify the file as lossy, while the NAudio analyzer returns
	/// an unknown compression type. Use this test to ensure consistency and
	/// correctness in audio type detection across supported analyzers.
	/// </remarks>
	[Test]
	public void GetAudioTypeM4aAacLossyReturnExpected()
	{
		string filePath = TestFiles["m4a"];

		CheckFileExists(filePath);

		CheckCompressionType(
			mediaFileFormatWithFfmpeg,
			filePath,
			CompressionType.Lossy);

		CheckCompressionType(
			mediaFileFormatWithNaudio,
			filePath,
			CompressionType.Unknown);

		CheckCompressionType(
			mediaFileFormatWithTagLib,
			filePath,
			CompressionType.Lossy);
	}

	/// <summary>
	/// Verifies that the GetCompressionType method returns the expected
	/// CompressionType values for an ALAC-encoded M4A file using different
	/// media file format analyzers.
	/// </summary>
	/// <remarks>This test checks that the FFMpeg and TagLib analyzers
	/// correctly identify the ALAC M4A file as lossless, while the NAudio
	/// analyzer returns an unknown compression type. The test requires the
	/// ALAC test file to be present at the specified path.</remarks>
	[Test]
	public void GetAudioTypeM4aAlacLosslessReturnExpected()
	{
		string filePath = TestFiles["alac"];

		CheckFileExists(filePath);

		CheckCompressionType(
			mediaFileFormatWithFfmpeg,
			filePath,
			CompressionType.Lossless);

		CheckCompressionType(
			mediaFileFormatWithNaudio,
			filePath,
			CompressionType.Unknown);

		CheckCompressionType(
			mediaFileFormatWithTagLib,
			filePath,
			CompressionType.Lossless);
	}

	/// <summary>
	/// Verifies that the GetCompressionType method of
	/// mediaFileFormatWithNaudio returns the expected CompressionType for a
	/// given audio file format.
	/// </summary>
	/// <param name="format">The file format extension to test, such as "mp3",
	/// "wav", or "flac".</param>
	/// <param name="expectedType">The expected CompressionType value that
	/// should be returned for the specified format.</param>
	[Test]
	[TestCase("mp3", CompressionType.Lossy)]
	[TestCase("wav", CompressionType.Lossless)]
	[TestCase("flac", CompressionType.Lossless)]
	public void GetAudioTypeNAudioReturnsCorrectType(
		string format, CompressionType expectedType)
	{
		string filePath = TestFiles[format];

		CheckFileExists(filePath);

		CheckCompressionType(
			mediaFileFormatWithNaudio,
			filePath,
			expectedType);
	}

	/// <summary>
	/// Verifies that Ogg Vorbis audio files are correctly identified as using
	/// lossy compression by both the FFmpeg and TagLib backends.
	/// </summary>
	/// <remarks>This test ensures that the GetCompressionType method returns
	/// CompressionType.Lossy for Ogg Vorbis files, validating consistent
	/// behavior across different media file format implementations.</remarks>
	[Test]
	public void GetAudioTypeOggVorbisReturnLossy()
	{
		string filePath = TestFiles["ogg"];

		CheckFileExists(filePath);

		CheckCompressionType(
			mediaFileFormatWithFfmpeg,
			filePath,
			CompressionType.Lossy);

		CheckCompressionType(
			mediaFileFormatWithNaudio,
			filePath,
			CompressionType.Unknown);

		CheckCompressionType(
			mediaFileFormatWithTagLib,
			filePath,
			CompressionType.Lossy);
	}

	/// <summary>
	/// Verifies that the GetCompressionType method of
	/// mediaFileFormatWithTagLib returns the correct CompressionType for a
	/// given audio file format.
	/// </summary>
	/// <remarks>This test ensures that the audio type detection logic
	/// correctly identifies the compression type for various supported formats
	/// using TagLib. Each test case provides a format and its corresponding
	/// expected compression
	/// type.</remarks>
	/// <param name="format">The file format extension (such as "mp3", "flac",
	/// or "ogg") used to select the test audio file.</param>
	/// <param name="expectedType">The expected CompressionType that should be
	/// returned for the specified format.</param>
	[Test]
	[TestCase("mp3", CompressionType.Lossy)]
	[TestCase("flac", CompressionType.Lossless)]
	[TestCase("ogg", CompressionType.Lossy)] // Vorbis
	public void GetAudioTypeTagLibReturnsCorrectType(
		string format, CompressionType expectedType)
	{
		string filePath = TestFiles[format];

		CheckFileExists(filePath);

		CheckCompressionType(
			mediaFileFormatWithTagLib,
			filePath,
			expectedType);
	}

	/// <summary>
	/// Verifies that the audio type detection correctly identifies WMA
	/// Lossless files as lossless.
	/// </summary>
	/// <remarks>This test is marked as inconclusive because it requires the
	/// generation of a WMA Lossless test file, which is not currently
	/// available.</remarks>
	[Test]
	public void GetAudioTypeWmaLosslessReturnsLossless()
	{
		// This would require generating a WMA Lossless file
		Assert.Inconclusive("Requires WMA Lossless test file generation");
	}

	/// <summary>
	/// Verifies that the GetCompressionType method returns
	/// CompressionType.Lossy for a WMA file, indicating that WMA is detected
	/// as a lossy audio format.
	/// </summary>
	/// <remarks>This test ensures that the mediaFileFormatWithFfmpeg
	/// implementation correctly identifies WMA files as using lossy
	/// compression. The test depends on the presence of a valid WMA test file.
	/// </remarks>
	[Test]
	public void GetAudioTypeWmaLossyReturnsLossy()
	{
		string filePath = TestFiles["wma"];

		CheckFileExists(filePath);

		CheckCompressionType(
			mediaFileFormatWithFfmpeg,
			filePath,
			CompressionType.Lossy);

		CheckCompressionType(
			mediaFileFormatWithNaudio,
			filePath,
			CompressionType.Unknown);

		CheckCompressionType(
			mediaFileFormatWithTagLib,
			filePath,
			CompressionType.Lossy);
	}
}
