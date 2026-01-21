/////////////////////////////////////////////////////////////////////////////
// <copyright file="MusicTests.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

[assembly: System.CLSCompliant(false)]

namespace DigitalZenWorks.MusicToolKit.Tests;

using System;
using System.IO;
using DigitalZenWorks.MusicToolKit.Decoders;
using DigitalZenWorks.RulesLibrary;
using NUnit.Framework;
using NUnit.Framework.Internal;

/// <summary>
/// General music manager tests class.
/// </summary>
[TestFixture]
internal sealed class MusicTests : BaseTestsSupport, IDisposable
{
	private MusicManager? musicManager;

	/// <summary>
	/// The one time setup method.
	/// </summary>
	[OneTimeSetUp]
	public void OneTimeSetUp()
	{
		musicManager = new MusicManager(false);
	}

	/// <summary>
	/// The setup before every test method.
	/// </summary>
	[SetUp]
	public void SetUp()
	{
		if (musicManager is null)
		{
			throw new ArgumentNullException(nameof(musicManager));
		}

		Rules = musicManager.Rules;
	}

	/// <summary>
	/// One time tear down method.
	/// </summary>
	[OneTimeTearDown]
	public void OneTimeTearDown()
	{
		Dispose();
	}

	/// <summary>
	/// Dispose method.
	/// </summary>
	public void Dispose()
	{
		Dispose(true);
		GC.SuppressFinalize(this);
	}

	/// <summary>
	/// Album name get from path method test.
	/// </summary>
	[Test]
	public void AlbumNameGetFromPath()
	{
		string fileName = @"Music\10cc\The Very Best Of 10cc\" +
			"The Things We Do For Love.mp3";

		string album = Paths.GetAlbumFromPath(fileName);

		Assert.That(album, Is.Not.Null);
		Assert.That(album, Is.Not.Empty);

		string expected = "The Very Best Of 10cc";
		Assert.That(album, Is.EqualTo(expected));
	}

	/// <summary>
	/// Album remove artist method test.
	/// </summary>
	[Test]
	public void AlbumRemoveArtist()
	{
		string album = "America - Ventura Highway";
		string artist = "America";

		album = AlbumRules.RemoveArtist(album, artist);

		Assert.That(album, Is.Not.Null);
		Assert.That(album, Is.Not.Empty);

		string expected = "Ventura Highway";
		Assert.That(album, Is.EqualTo(expected));
	}

	/// <summary>
	/// Album remove cd method test.
	/// </summary>
	[Test]
	public void AlbumRemoveCd()
	{
		string album = "Den Bosh cd 1";

		album = AlbumRules.RemoveCd(album);

		Assert.That(album, Is.Not.Null);
		Assert.That(album, Is.Not.Empty);

		string expected = "Den Bosh";
		Assert.That(album, Is.EqualTo(expected));
	}

	/// <summary>
	/// Album remove cd no change test.
	/// </summary>
	[Test]
	public void AlbumRemoveCdNoChange()
	{
		string original = "Den Bosh";
		string album = original;

		album = AlbumRules.RemoveCd(album);

		Assert.That(album, Is.Not.Null);
		Assert.That(album, Is.Not.Empty);

		Assert.That(album, Is.EqualTo(original));
	}

	/// <summary>
	/// Album remove copy amount test.
	/// </summary>
	[Test]
	public void AlbumRemoveCopyAmount()
	{
		string album = "Den Bosh (1)";

		album = AlbumRules.RemoveCopyAmount(album);

		Assert.That(album, Is.Not.Null);
		Assert.That(album, Is.Not.Empty);

		string expected = "Den Bosh";
		Assert.That(album, Is.EqualTo(expected));
	}

	/// <summary>
	/// Album remove copy amount same test.
	/// </summary>
	[Test]
	public void AlbumRemoveCopyAmountSame()
	{
		string album = "Den Bosh (Nice Day)";

		album = AlbumRules.RemoveCopyAmount(album);

		Assert.That(album, Is.Not.Null);
		Assert.That(album, Is.Not.Empty);

		string expected = "Den Bosh (Nice Day)";
		Assert.That(album, Is.EqualTo(expected));
	}

	/// <summary>
	/// Album remove disc method test.
	/// </summary>
	[Test]
	public void AlbumRemoveDisc()
	{
		string album = "What It Is! Funky Soul And Rare Grooves (Disk 2)";

		album = AlbumRules.RemoveDisc(album);

		Assert.That(album, Is.Not.Null);
		Assert.That(album, Is.Not.Empty);

		string expected = "What It Is! Funky Soul And Rare Grooves";
		Assert.That(album, Is.EqualTo(expected));
	}

	/// <summary>
	/// Album remove cd no change test.
	/// </summary>
	[Test]
	public void AlbumRemoveDiscNoChange()
	{
		string original = "Den Bosh";
		string album = original;

		album = AlbumRules.RemoveDisc(album);

		Assert.That(album, Is.Not.Null);
		Assert.That(album, Is.Not.Empty);

		Assert.That(album, Is.EqualTo(original));
	}

	/// <summary>
	/// Regex remove cd method test.
	/// </summary>
	[Test]
	public void AlbumRemoveFlac()
	{
		string album = "Talking Heads - Brick(2005)[FLAC]";

		album = AlbumRules.RemoveFlac(album);

		Assert.That(album, Is.Not.Null);
		Assert.That(album, Is.Not.Empty);

		string expected = "Talking Heads - Brick(2005)";
		Assert.That(album, Is.EqualTo(expected));
	}

	/// <summary>
	/// Album replace curly braces method test.
	/// </summary>
	[Test]
	public void AlbumReplaceCurlyBraces()
	{
		string album = "Something {In Heaven}";

		album = AlbumRules.ReplaceCurlyBraces(album);

		string expected = "Something [In Heaven]";
		Assert.That(album, Is.EqualTo(expected));
	}

	/// <summary>
	/// Album replace curly braces method test.
	/// </summary>
	[Test]
	public void AlbumReplaceCurlyBracesNoChange()
	{
		string original = "Something In Heaven";
		string album = original;

		album = AlbumRules.ReplaceCurlyBraces(album);

		Assert.That(album, Is.EqualTo(original));
	}

	/// <summary>
	/// The artist name get from path method test.
	/// </summary>
	[Test]
	public void ArtistNameGetFromPath()
	{
		string fileName = @"Music\10cc\The Very Best Of 10cc\" +
			"The Things We Do For Love.mp3";

		string artist = Paths.GetArtistFromPath(fileName);

		Assert.That(artist, Is.Not.Null);
		Assert.That(artist, Is.Not.Empty);

		string expected = "10cc";
		Assert.That(artist, Is.EqualTo(expected));
	}

	/// <summary>
	/// The artist remove album method test.
	/// </summary>
	[Test]
	public void ArtistRemoveAlbum()
	{
		string artist = "America - Ventura Highway";
		string album = "Ventura Highway";

		artist = ArtistRules.RemoveAlbum(artist, album);

		Assert.That(artist, Is.Not.Null);
		Assert.That(artist, Is.Not.Empty);

		string expected = "America";
		Assert.That(artist, Is.EqualTo(expected));
	}

	/// <summary>
	/// The artist replace various artists method test.
	/// </summary>
	[Test]
	public void ArtistReplaceVariousArtists()
	{
		string artist = "Various Artists";
		string replacement = "The Spinners";

		artist = ArtistRules.ReplaceVariousArtists(artist, replacement);

		Assert.That(artist, Is.Not.Null);
		Assert.That(artist, Is.Not.Empty);

		Assert.That(artist, Is.EqualTo(replacement));
	}

	/// <summary>
	/// The artist replace various artists method test.
	/// </summary>
	[Test]
	public void ArtistReplaceVariousArtistsNotNull()
	{
		string original = "Various Artists";
		string? replacement = null;

		string artist =
			ArtistRules.ReplaceVariousArtists(original, replacement);

		Assert.That(artist, Is.Not.Null);
		Assert.That(artist, Is.Not.Empty);

		Assert.That(artist, Is.EqualTo(original));
	}

	/// <summary>
	/// The clean album success test.
	/// </summary>
	[Test]
	public void CapitalizeFirstCharacter()
	{
		string title = "sakura";

		title = GeneralRules.CapitalizeFirstCharacter(title);

		string expected = "Sakura";
		Assert.That(title, Is.EqualTo(expected));
	}

	/// <summary>
	/// The clean album success test.
	/// </summary>
	[Test]
	public void CleanAlbumSuccess()
	{
		string album = "America - Somewhere  in *Heaven.";
		string artist = "America";

		album = AlbumRules.CleanAlbumFilePath(album, artist);

		string expected = "Somewhere in Heaven";
		Assert.That(album, Is.EqualTo(expected));
	}

	/// <summary>
	/// The clean artist success test.
	/// </summary>
	[Test]
	public void CleanArtistSuccess()
	{
		string artist = ArtistRules.CleanArtistFilePath(
			"?Jefferson  Airplane..", null, null);

		Assert.That(artist, Is.Not.Null);
		Assert.That(artist, Is.Not.Empty);

		string expected = "Jefferson Airplane";
		Assert.That(artist, Is.EqualTo(expected));
	}

	/// <summary>
	/// The decode file test.
	/// </summary>
	[Test]
	public void DecodeFile()
	{
		using NAudioDecoder decoder = new (TestFile);
		AudioConsumer consumer = new ();

		bool result = decoder.Decode(consumer, 240);

		Assert.That(result, Is.True);
	}

	/// <summary>
	/// The get ChromaPrint audio signature test.
	/// </summary>
	[Test]
	public void AudioSignatureTest()
	{
		string intended = "AQAAfFGiSAmTHVRyHg-FQ_yQH7WWHJ_m4NIx7nCSD2cR" +
			"_Tp-8BC_F6-QJwSj8HDoFD2LHjk8KodWsiueHLNuaDoatUH08bj4BPQs" +
			"KbiiY2d05IGOPD-8l3ieCueLBz7xC8mRG3ZyfFqD6xWO68iXB7qSNsgv" +
			"NEcvZcdjzEye4NkX_HjzgJ-RT0G3PqCWHtexH-JNeHoVTAwrxMV3MMkT9" +
			"ApyQj9y8mi6pUlQ57jw_PCTDHl-6MjyJ_CDP0W9gVEIh0T4hLgM_Ud-" +
			"9LmCpsmDZ0e8Clo-hFfwnvivBHWOD2fQH_FRLg-YLMsFfcfRfEck73jy4_" +
			"iNdhmadwzOhMeHM0cYOcmhHceZA0EQZdAJQZAAhCEgnMFEEEGEdAAhZRAB" +
			"hCCBGBvICIQAAIhRQKIBAAJJEDAGCCMFIcgAYxAxwCHDgEDCCIGEQAAQRR" +
			"gAgBBMBGJCKCkEMUADQwAAgiCmlFAECVIEQsJAAgAQgiGHQRMA";

		string audioSignature = AudioSignature.GetAudioSignature(TestFile);

		Assert.That(audioSignature, Is.EqualTo(intended));
	}

	/// <summary>
	/// The get duplicate location test.
	/// </summary>
	[Test]
	public void GetDuplicateLocation()
	{
		string location = musicManager!.LibraryLocation;

		string fileName = @"Music\10cc\The Very Best Of 10cc\" +
			"The Things We Do For Love.mp3";
		string fullPath = Path.Combine(location, fileName);

		string duplicateLocation =
			MusicManager.GetDuplicateLocation(fullPath);

		bool contains = duplicateLocation.Contains(
			"Music2", StringComparison.Ordinal);

		Assert.That(contains, Is.True);
	}

	/// <summary>
	/// The MusicManager check for rules test.
	/// </summary>
	[Test]
	public void MusicManagerCheckForSameRules()
	{
		string pattern = @"\s*\(Dis(c|k).*?\)";

		Rule rule = new (
			"DigitalZenWorks.MusicToolKit.Tags.Album",
			Condition.ContainsRegex,
			pattern,
			Operation.Remove);

		Rules.RulesList.Add(rule);

		using MusicManager musicManager2 = new (Rules, false);

		Rules rules2 = musicManager2.Rules;

		int count1 = Rules.RulesList.Count;
		int count2 = rules2.RulesList.Count;
		Assert.That(count2, Is.EqualTo(count1));
	}

	/// <summary>
	/// The normalize path album segment missing test.
	/// </summary>
	[Test]
	public void NormalizePathAlbumMissing()
	{
		using MusicManager musicManager = new (true);

		string location = musicManager.LibraryLocation;

		string fileName = @"Music\Golden Earring\Radar Love.mp3";
		string fullPath = Path.Combine(location, fileName);

		string normalizedFilePath =
			musicManager.NormalizePath(fullPath, true, false);

		string expectedFileName = @"Music\Golden Earring\Album " +
			@"Information Unavailable\Radar Love.mp3";
		string expected = Path.Combine(location, expectedFileName);

		Assert.That(normalizedFilePath, Is.EqualTo(expected));
	}

	/// <summary>
	/// The normalize path same test.
	/// </summary>
	[Test]
	public void NormalizePathSame()
	{
		using MusicManager musicManager = new (true);

		string location = musicManager.LibraryLocation;

		string fileName = @"Music\10cc\The Very Best Of 10cc\" +
			"The Things We Do For Love.mp3";
		string fullPath = Path.Combine(location, fileName);

		string normalizedFilePath = musicManager.NormalizePath(fullPath);

		Assert.That(normalizedFilePath, Is.EqualTo(fullPath));
	}

	/// <summary>
	/// The normalize path updated test.
	/// </summary>
	[Test]
	public void NormalizePathUpdated()
	{
		using MusicManager musicManager = new (true);

		string location = musicManager.LibraryLocation;

		string fileName = @"Music\10cc\The Very Best Of 10cc\" +
			"The Things We Do For Love 2.mp3";
		string fullPath = Path.Combine(location, fileName);

		string normalizedFilePath =
			musicManager.NormalizePath(fullPath, true, false);

		string expectedFileName = @"Music\10cc\The Very Best Of 10cc\" +
			"The Things We Do For Love.mp3";
		string expected = Path.Combine(location, expectedFileName);

		Assert.That(normalizedFilePath, Is.EqualTo(expected));
	}

	/// <summary>
	/// The normalize path only title segment existing test.
	/// </summary>
	[Test]
	public void NormalizePathOnlyTitle()
	{
		using MusicManager musicManager = new (true);

		string location = musicManager.LibraryLocation;

		string fileName = @"Music\Jimi Hendrix   The Star Spangled " +
			"Banner  American Anthem   Live at Woodstock 1969.mp3";
		string fullPath = Path.Combine(location, fileName);

		string normalizedFilePath =
			musicManager.NormalizePath(fullPath, true, false);

		string expectedFileName = @"Music\Unknown Artist\Album " +
			@"Information Unavailable\Jimi Hendrix The Star Spangled " +
			"Banner American Anthem Live at Woodstock 1969.mp3";
		string expected = Path.Combine(location, expectedFileName);

		Assert.That(normalizedFilePath, Is.EqualTo(expected));
	}

	/// <summary>
	/// The regex remove result differnt test.
	/// </summary>
	[Test]
	public void RegexRemoveDifferent()
	{
		string original = "What It Is! Funky Soul And Rare Grooves cd 1";
		string? content = original;
		string pattern = @" cd.*?\d";
		content = Rule.RegexRemove(pattern, content);

		bool result = original.Equals(content, StringComparison.Ordinal);

		Assert.That(result, Is.False);
	}

	/// <summary>
	/// The regex remove result same test.
	/// </summary>
	[Test]
	public void RegexRemoveNoPattern()
	{
		string original = "What It Is! Funky Soul And Rare Grooves";
		string? content = original;
		string? pattern = null;
		content = Rule.RegexRemove(pattern, content);

		Assert.That(content, Is.EqualTo(original));
	}

	/// <summary>
	/// The regex remove result same test.
	/// </summary>
	[Test]
	public void RegexRemoveSame()
	{
		string original = "What It Is! Funky Soul And Rare Grooves";
		string? content = original;
		string pattern = @" cd.*?\d";
		content = Rule.RegexRemove(pattern, content);

		Assert.That(content, Is.EqualTo(original));
	}

	/// <summary>
	/// The save tags to json file fail test.
	/// </summary>
	[Test]
	public void SaveTagsToJsonFileFail()
	{
		string temporaryFile = Path.GetTempFileName();
		FileInfo fileInfo = new (temporaryFile);
		string? destinationPath = Path.GetDirectoryName(temporaryFile);
		string filePath =
			musicManager!.SaveTagsToJsonFile(fileInfo, destinationPath);

		Assert.That(filePath, Is.Null);
	}

	/// <summary>
	/// The save tags to json file success test.
	/// </summary>
	[Test]
	public void SaveTagsToJsonFileSuccess()
	{
		FileInfo fileInfo = new (TestFile);
		string? destinationPath = Path.GetDirectoryName(TestFile);
		string filePath =
			musicManager!.SaveTagsToJsonFile(fileInfo, destinationPath);

		Assert.That(filePath, Is.Not.Null);

		string destinationFile =
			destinationPath + "\\" + fileInfo.Name + ".json";

		bool result = File.Exists(destinationFile);

		Assert.Multiple(() =>
		{
			Assert.That(result, Is.True);
			Assert.That(filePath, Is.EqualTo(destinationFile));
		});
	}

	/// <summary>
	/// The title extract sub-title test.
	/// </summary>
	[Test]
	public void TitleExtractSubTitle()
	{
		string title = "Sakura [Hanami]";

		string subTitle = TitleRules.ExtractSubTitle(title);

		string expected = "Hanami";
		Assert.That(subTitle, Is.EqualTo(expected));
	}

	/// <summary>
	/// Title remove artist method test.
	/// </summary>
	[Test]
	public void TitleRemoveArtist()
	{
		string title = "America - Ventura Highway";
		string artist = "America";

		title = TitleRules.RemoveArtist(title, artist);

		Assert.That(title, Is.Not.Null);
		Assert.That(title, Is.Not.Empty);

		string expected = "Ventura Highway";
		Assert.That(title, Is.EqualTo(expected));
	}

	/// <summary>
	/// Title remove sub-title method test.
	/// </summary>
	[Test]
	public void TitleRemoveSubTitle()
	{
		string title = "Sakura [Hanami]";

		title = TitleRules.RemoveBracketedSubTitle(title);

		Assert.That(title, Is.Not.Null);
		Assert.That(title, Is.Not.Empty);

		string expected = "Sakura";
		Assert.That(title, Is.EqualTo(expected));
	}

	/// <summary>
	/// The update file case difference test.
	/// </summary>
	[Test]
	public void UpdateFileCaseDifference()
	{
		string albumPath = @"\Music\Artist\Album";
		string newFileName =
			MakeTestFileCopy(albumPath, "sakura test.mp4");

		musicManager!.LibraryLocation = TemporaryPath;

		newFileName = musicManager.UpdateFile(newFileName, false);

		string basePath = Paths.GetBasePathFromFilePath(TestFile);
		string expected =
			basePath + @"\Artist\Album\Sakura test.mp4";

		Assert.That(newFileName, Is.EqualTo(expected));

		bool exists = File.Exists(newFileName);
		Assert.That(exists, Is.True);

		// Clean up.
		File.Delete(newFileName);
	}

	/// <summary>
	/// The update file different - moved test.
	/// </summary>
	[Test]
	public void UpdateFileDifferentMoved()
	{
		string originalFileName = MakeTestFileCopy(
			@"\Music\Artist\Album (Disk 2)", "Hanami.mp4");

		string albumPath = @"\Music\Artist\Album";
		string normalizedFileName =
			MakeTestFileCopy(albumPath, "Hanami.mp4");

		// Delete normalized test file, so that we can move the new normalized
		// file there.
		File.Delete(normalizedFileName);

		string newFileName =
			musicManager!.UpdateFile(originalFileName, false);

		// The un-normalized file should have been moved.
		bool exists = File.Exists(originalFileName);
		Assert.That(exists, Is.False);

		string basePath = Paths.GetBasePathFromFilePath(TestFile);
		string expected = basePath + @"\Artist\Album\Hanami.mp4";

		Assert.That(newFileName, Is.EqualTo(expected));

		exists = File.Exists(newFileName);
		Assert.That(exists, Is.True);

		// Clean up.
		File.Delete(newFileName);
	}

	/// <summary>
	/// The update file different - moved to duplicates test.
	/// </summary>
	[Test]
	public void UpdateFileDifferentMovedToDuplicates()
	{
		string originalFileName = MakeTestFileCopy(
			@"\Music\Artist\Album (Disk 2)", "Sakura.mp4");

		// Update file, so it is not quite the same as original.
		using MediaFileTags tags = new (originalFileName);

		tags.Year = 1975;
		tags.Update();

		string newFileName =
			musicManager!.UpdateFile(originalFileName, false);

		string basePath = Paths.GetBasePathFromFilePath(TestFile);

		string newBasePath = basePath + "2";
		string expected = newBasePath + @"\Artist\Album\Sakura.mp4";

		Assert.That(newFileName, Is.EqualTo(expected));

		bool exists = File.Exists(newFileName);
		Assert.That(exists, Is.True);

		// The updated file should have been moved to duplicates.
		exists = File.Exists(originalFileName);
		Assert.That(exists, Is.False);

		// Clean up.
		File.Delete(newFileName);

		if (Directory.Exists(newBasePath))
		{
			Directory.Delete(newBasePath, true);
		}
	}

	/// <summary>
	/// The update file different - exact duplicate test.
	/// </summary>
	[Test]
	public void UpdateFileExactDuplicate()
	{
		string originalFileName = MakeTestFileCopy(
			@"\Music\Artist\Album (Disk 2)", "Hanami.mp4");

		string newFileName =
			musicManager!.UpdateFile(originalFileName, false);

		// The un-normalized file should have been deleted.
		bool exists = File.Exists(originalFileName);
		Assert.That(exists, Is.False);

		string basePath = Paths.GetBasePathFromFilePath(TestFile);
		string expected = basePath + @"\Artist\Album\Hanami.mp4";

		Assert.That(newFileName, Is.EqualTo(expected));

		exists = File.Exists(newFileName);
		Assert.That(exists, Is.True);

		// Clean up.
		File.Delete(newFileName);
	}

	/// <summary>
	/// The update file same test.
	/// </summary>
	[Test]
	public void UpdateFileSame()
	{
		string newFileName = musicManager!.UpdateFile(TestFile, false);

		string basePath = Paths.GetBasePathFromFilePath(TestFile);
		string expected =
			Path.Combine(basePath, @"Artist\Album\Sakura.mp4");

		Assert.That(newFileName, Is.EqualTo(expected));

		bool exists = File.Exists(newFileName);
		Assert.That(exists, Is.True);
	}

	/// <summary>
	/// Dispose method.
	/// </summary>
	/// <param name="disposing">Indicates whether currently disposing
	/// or not.</param>
	private void Dispose(bool disposing)
	{
		if (disposing)
		{
			// dispose managed resources
			musicManager?.Dispose();
			musicManager = null;
		}
	}
}
