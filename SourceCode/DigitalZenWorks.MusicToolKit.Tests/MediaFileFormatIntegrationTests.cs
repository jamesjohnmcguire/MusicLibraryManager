/////////////////////////////////////////////////////////////////////////////
// <copyright file="MediaFileFormatIntegrationTests.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MusicToolKit.Tests;

using MediaFileToolkit;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

internal class MediaFileFormatIntegrationTests : BaseTestsSupport
{
	private MediaFileFormat _mediaFileFormatWithFfmpeg;
	private MediaFileFormat _mediaFileFormatWithNaudio;
	private MediaFileFormat _mediaFileFormatWithTagLib;

	[OneTimeSetUp]
	public void OneTimeSetUp()
	{
		// Initialize with real implementations
		// Note: You'll need to provide actual implementations
		_mediaFileFormatWithFfmpeg = new MediaFileFormat(new MediaFileFormatFfmpeg());
		_mediaFileFormatWithNaudio = new MediaFileFormat(new MediaFileFormatNaudio());
		_mediaFileFormatWithTagLib = new MediaFileFormat(new MediaFileFormatTagLib());
	}

	[OneTimeTearDown]
	public void OneTimeTearDown()
	{
	}

	[Test]
	[TestCase("mp3", CompressionType.Lossy)]
	[TestCase("aac", CompressionType.Lossy)]
	[TestCase("opus", CompressionType.Lossy)]
	[TestCase("wav", CompressionType.Lossless)]
	[TestCase("flac", CompressionType.Lossless)]
	public void GetAudioType_RealFiles_FFmpeg_ReturnsCorrectType(string format, CompressionType expectedType)
	{
		string filePath = TestFiles[format];
		if (filePath == null)
			Assert.Ignore($"Test file for {format} not available");

		var result = _mediaFileFormatWithFfmpeg.GetCompressionType(filePath);

		Assert.That(result, Is.EqualTo(expectedType));
	}

	[Test]
	[TestCase("mp3", CompressionType.Lossy)]
	[TestCase("wav", CompressionType.Lossless)]
	[TestCase("flac", CompressionType.Lossless)]
	public void GetAudioType_RealFiles_NAudio_ReturnsCorrectType(string format, CompressionType expectedType)
	{
		string filePath = TestFiles[format];
		if (filePath == null)
			Assert.Ignore($"Test file for {format} not available");

		var result = _mediaFileFormatWithNaudio.GetCompressionType(filePath);

		Assert.That(result, Is.EqualTo(expectedType));
	}

	[Test]
	[TestCase("mp3", CompressionType.Lossy)]
	[TestCase("flac", CompressionType.Lossless)]
	[TestCase("ogg", CompressionType.Lossy)] // Vorbis
	public void GetAudioType_RealFiles_TagLib_ReturnsCorrectType(string format, CompressionType expectedType)
	{
		string filePath = TestFiles[format];
		if (filePath == null)
			Assert.Ignore($"Test file for {format} not available");

		var result = _mediaFileFormatWithTagLib.GetCompressionType(filePath);

		Assert.That(result, Is.EqualTo(expectedType));
	}

	[Test]
	public void GetAudioType_M4aAlac_AllImplementations_ReturnLossless()
	{
		string filePath = TestFiles["alac"];
		if (filePath == null)
			Assert.Ignore("M4A test file not available");

		var resultFfmpeg = _mediaFileFormatWithFfmpeg.GetCompressionType(filePath);
		var resultNaudio = _mediaFileFormatWithNaudio.GetCompressionType(filePath);
		var resultTagLib = _mediaFileFormatWithTagLib.GetCompressionType(filePath);

		Assert.That(resultFfmpeg, Is.EqualTo(CompressionType.Lossless));
		Assert.That(resultNaudio, Is.EqualTo(CompressionType.Unknown));
		Assert.That(resultTagLib, Is.EqualTo(CompressionType.Lossless));
	}

	[Test]
	public void GetAudioType_M4aAac_AllImplementations_ReturnLossy()
	{
		string filePath = TestFiles["m4a"];
		if (filePath == null)
			Assert.Ignore("M4A test file not available");

		var resultFfmpeg = _mediaFileFormatWithFfmpeg.GetCompressionType(filePath);
		var resultNaudio = _mediaFileFormatWithNaudio.GetCompressionType(filePath);
		var resultTagLib = _mediaFileFormatWithTagLib.GetCompressionType(filePath);

		Assert.That(resultFfmpeg, Is.EqualTo(CompressionType.Lossy));
		Assert.That(resultNaudio, Is.EqualTo(CompressionType.Unknown));
		Assert.That(resultTagLib, Is.EqualTo(CompressionType.Lossy));
	}

	[Test]
	public void GetAudioType_OggVorbis_AllImplementations_ReturnLossy()
	{
		string filePath = TestFiles["ogg"];
		if (filePath == null)
			Assert.Ignore("OGG test file not available");

		var resultFfmpeg = _mediaFileFormatWithFfmpeg.GetCompressionType(filePath);
		var resultTagLib = _mediaFileFormatWithTagLib.GetCompressionType(filePath);

		Assert.That(resultFfmpeg, Is.EqualTo(CompressionType.Lossy));
		Assert.That(resultTagLib, Is.EqualTo(CompressionType.Lossy));
	}

	[Test]
	public void GetAudioType_WmaLossless_ReturnsLossless()
	{
		// This would require generating a WMA Lossless file
		Assert.Inconclusive("Requires WMA Lossless test file generation");
	}

	[Test]
	public void GetAudioType_WmaLossy_ReturnsLossy()
	{
		string filePath = TestFiles["wma"];
		if (filePath == null)
			Assert.Ignore("WMA test file not available");

		var result = _mediaFileFormatWithFfmpeg.GetCompressionType(filePath);

		Assert.That(result, Is.EqualTo(CompressionType.Lossy));
	}

	[Test]
	public void GetAudioType_AllImplementations_AgreeOnSimpleFormats()
	{
		var testCases = new[] { "mp3", "wav", "flac" };

		foreach (var format in testCases)
		{
			string filePath = TestFiles[format];
			if (filePath == null)
				continue;

			var resultFfmpeg = _mediaFileFormatWithFfmpeg.GetCompressionType(filePath);
			var resultNaudio = _mediaFileFormatWithNaudio.GetCompressionType(filePath);
			var resultTagLib = _mediaFileFormatWithTagLib.GetCompressionType(filePath);

			Assert.That(resultFfmpeg, Is.EqualTo(resultNaudio),
				$"FFmpeg and NAudio disagree on {format}");
			Assert.That(resultNaudio, Is.EqualTo(resultTagLib),
				$"NAudio and TagLib disagree on {format}");
		}
	}
}
