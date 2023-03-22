/////////////////////////////////////////////////////////////////////////////
// <copyright file="MusicTests.cs" company="Digital Zen Works">
// Copyright © 2019 - 2023 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using DigitalZenWorks.Common.Utilities;
using DigitalZenWorks.MusicToolKit.Decoders;
using DigitalZenWorks.RulesLibrary;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

[assembly: CLSCompliant(false)]

namespace DigitalZenWorks.MusicToolKit.Tests
{
	/// <summary>
	/// Unit tests class.
	/// </summary>
	[TestFixture]
	public class MusicTests : IDisposable
	{
		private MusicManager musicManager;
		private Rules rules;
		private string temporaryPath;
		private string testFile;

		/// <summary>
		/// The one time setup method.
		/// </summary>
		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			musicManager = new MusicManager(false);

			temporaryPath = Path.GetTempFileName();
			File.Delete(temporaryPath);
			Directory.CreateDirectory(temporaryPath);

			testFile = temporaryPath + @"\Artist\Album\sakura.mp4";
		}

		/// <summary>
		/// The setup before every test method.
		/// </summary>
		[SetUp]
		public void SetUp()
		{
			FileUtils.CreateFileFromEmbeddedResource(
				"DigitalZenWorks.MusicToolKit.Tests.sakura.mp4", testFile);

			rules = musicManager.Rules;
		}

		/// <summary>
		/// One time tear down method.
		/// </summary>
		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			bool result = Directory.Exists(temporaryPath);

			if (true == result)
			{
				Directory.Delete(temporaryPath, true);
			}

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

			Assert.IsNotEmpty(album);

			string expected = "The Very Best Of 10cc";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// Album remove cd method test.
		/// </summary>
		[Test]
		public void AlbumRemoveCd()
		{
			string album = "Den Bosh cd 1";

			album = AlbumTagRules.RemoveCd(album);

			Assert.IsNotEmpty(album);

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

			album = AlbumTagRules.RemoveCd(album);

			Assert.IsNotEmpty(album);

			Assert.That(album, Is.EqualTo(original));
		}

		/// <summary>
		/// Album remove copy amount test.
		/// </summary>
		[Test]
		public void AlbumRemoveCopyAmount()
		{
			string album = "Den Bosh (1)";

			album = AlbumTagRules.RemoveCopyAmount(album);

			Assert.IsNotEmpty(album);

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

			album = AlbumTagRules.RemoveCopyAmount(album);

			Assert.IsNotEmpty(album);

			string expected = "Den Bosh (Nice Day)";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// Album replace curly braces method test.
		/// </summary>
		[Test]
		public void AlbumReplaceCurlyBraces()
		{
			string album = "Something {In Heaven}";

			album = AlbumTagRules.ReplaceCurlyBraces(album);

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

			album = AlbumTagRules.ReplaceCurlyBraces(album);

			Assert.That(album, Is.EqualTo(original));
		}

		/// <summary>
		/// Album remove disc method test.
		/// </summary>
		[Test]
		public void AlbumRemoveDisc()
		{
			string album = "What It Is! Funky Soul And Rare Grooves (Disk 2)";

			album = AlbumTagRules.RemoveDisc(album);

			Assert.IsNotEmpty(album);

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

			album = AlbumTagRules.RemoveDisc(album);

			Assert.IsNotEmpty(album);

			Assert.That(album, Is.EqualTo(original));
		}

		/// <summary>
		/// Regex remove cd method test.
		/// </summary>
		[Test]
		public void AlbumRemoveFlac()
		{
			string album = "Talking Heads - Brick(2005)[FLAC]";

			album = album.Replace(
				"[FLAC]", string.Empty, StringComparison.OrdinalIgnoreCase);

			Assert.IsNotEmpty(album);

			string expected = "Talking Heads - Brick(2005)";
			Assert.That(album, Is.EqualTo(expected));
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

			Assert.IsNotEmpty(artist);

			string expected = "10cc";
			Assert.That(artist, Is.EqualTo(expected));
		}

		/// <summary>
		/// The clean album success test.
		/// </summary>
		[Test]
		public void CleanAlbumSuccess()
		{
			string artistPath = Paths.GetArtistPathFromFilePath(testFile);

			string album = MusicManager.CleanAlbum("Album");

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// The clean artist success test.
		/// </summary>
		[Test]
		public void CleanArtistSuccess()
		{
			string artist = MusicManager.CleanArtist("Artist");

			Assert.IsNotEmpty(artist);

			string expected = "Artist";
			Assert.That(artist, Is.EqualTo(expected));
		}

		/// <summary>
		/// The decode file test.
		/// </summary>
		[Test]
		public void DecodeFile()
		{
			using NAudioDecoder decoder = new (testFile);
			AudioConsumer consumer = new ();

			bool result = decoder.Decode(consumer, 240);

			Assert.True(result);
		}

		/// <summary>
		/// The get duplicate location test.
		/// </summary>
		[Test]
		public void GetDuplicateLocation()
		{
			string location = musicManager.LibraryLocation;

			string fileName = @"Music\10cc\The Very Best Of 10cc\" +
				"The Things We Do For Love.mp3";
			string fullPath = Path.Combine(location, fileName);

			string duplicateLocation =
				MusicManager.GetDuplicateLocation(fullPath);

			bool contains = duplicateLocation.Contains(
				"Music2", StringComparison.Ordinal);

			Assert.True(contains);
		}

		/// <summary>
		/// Instance Album remove cd method test.
		/// </summary>
		[Test]
		public void InstanceAlbumRemoveCd()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album cd 1", "sakura.mp4");

			using MediaFileTags tags = new (newFileName);
			tags.Album = "Album cd 1";

			bool result = tags.Clean();

			File.Delete(newFileName);

			Assert.True(result);

			string album = tags.Album;
			Assert.IsNotEmpty(album);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// Instance Album remove cd no change test.
		/// </summary>
		[Test]
		public void InstanceAlbumRemoveCdNoChange()
		{
			string original = "Album";
			using MediaFileTags tags = new (testFile);
			tags.Album = original;

			bool result = tags.Clean();
			Assert.True(result);

			string album = tags.Album;
			Assert.IsNotEmpty(album);

			Assert.That(album, Is.EqualTo(original));
		}

		/// <summary>
		/// Instance Album remove disc method test.
		/// </summary>
		[Test]
		public void InstanceAlbumRemoveDisc()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album (Disk 2)", "sakura.mp4");

			using MediaFileTags tags = new (newFileName);
			string album = tags.Album = "Album (Disk 2)";

			bool result = tags.Clean();

			File.Delete(newFileName);

			Assert.True(result);

			album = tags.Album;
			Assert.IsNotEmpty(album);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// Instance Album remove cd no change test.
		/// </summary>
		[Test]
		public void InstanceAlbumRemoveDiscNoChange()
		{
			string original = "Album";
			using MediaFileTags tags = new (testFile);
			tags.Album = original;
			tags.Artist = "Artist";
			tags.Title = "Sakura";

			bool result = tags.Clean();
			Assert.False(result);

			string album = tags.Album;
			Assert.IsNotEmpty(album);

			Assert.That(album, Is.EqualTo(original));
		}

		/// <summary>
		/// The run rule disc check method.
		/// </summary>
		[Test]
		public void MediaFileGetTagsCheck()
		{
			using MediaFileTags tags = new (testFile, rules);

			string original =
				"What It Is! Funky Soul And Rare Grooves (Disk 2)";
			tags.Album = original;

			tags.Artist = "The Solos";
			tags.Update();

			SortedDictionary<string, object> tagSet = tags.GetTags();

			Assert.NotNull(tags);
			Assert.NotNull(tags.TagFile);
			Assert.NotNull(tagSet);
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

			rules.RulesList.Add(rule);

			using MusicManager musicManager2 = new (rules, false);

			Rules rules2 = musicManager2.Rules;

			int count1 = rules.RulesList.Count;
			int count2 = rules2.RulesList.Count;
			Assert.AreEqual(count1, count2);
		}

		/// <summary>
		/// The normalize path same test.
		/// </summary>
		[Test]
		public void NormalizePathSame()
		{
			string location = musicManager.LibraryLocation;

			string fileName = @"Music\10cc\The Very Best Of 10cc\" +
				"The Things We Do For Love.mp3";
			string fullPath = Path.Combine(location, fileName);
			FileInfo file = new (fullPath);

			string normalizedFilePath = MusicManager.NormalizePath(file);

			bool result =
				fullPath.Equals(normalizedFilePath, StringComparison.Ordinal);

			Assert.True(result);
		}

		/// <summary>
		/// The normalize path updated test.
		/// </summary>
		[Test]
		public void NormalizePathUpdated()
		{
			string location = musicManager.LibraryLocation;

			string fileName = @"Music\10cc\The Very Best Of 10cc\" +
				"The Things We Do For Love2.mp3";
			string fullPath = Path.Combine(location, fileName);
			FileInfo file = new (fullPath);

			string normalizedFilePath = MusicManager.NormalizePath(file);

			bool result =
				fullPath.Equals(normalizedFilePath, StringComparison.Ordinal);

			Assert.True(result);
		}

		/// <summary>
		/// The regex remove result differnt test.
		/// </summary>
		[Test]
		public void RegexRemoveDifferent()
		{
			string original = "What It Is! Funky Soul And Rare Grooves cd 1";
			string content = original;
			string pattern = @" cd.*?\d";
			content = Rule.RegexRemove(pattern, content);

			bool result = original.Equals(content, StringComparison.Ordinal);

			Assert.False(result);
		}

		/// <summary>
		/// The regex remove result same test.
		/// </summary>
		[Test]
		public void RegexRemoveNoPattern()
		{
			string original = "What It Is! Funky Soul And Rare Grooves";
			string content = original;
			string pattern = null;
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
			string content = original;
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
			string destinationPath = Path.GetDirectoryName(temporaryFile);
			string filePath =
				musicManager.SaveTagsToJsonFile(fileInfo, destinationPath);

			Assert.Null(filePath);
		}

		/// <summary>
		/// The save tags to json file success test.
		/// </summary>
		[Test]
		public void SaveTagsToJsonFileSuccess()
		{
			FileInfo fileInfo = new (testFile);
			string destinationPath = Path.GetDirectoryName(testFile);
			string filePath =
				musicManager.SaveTagsToJsonFile(fileInfo, destinationPath);

			Assert.NotNull(filePath);
			string destinationFile =
				destinationPath + "\\" + fileInfo.Name + ".json";

			bool result = File.Exists(destinationFile);
			Assert.True(result);

			Assert.That(filePath, Is.EqualTo(destinationFile));
		}

		/// <summary>
		/// The tag file update album from path test.
		/// </summary>
		[Test]
		public void TagFileUpdateAlbumFromPath()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album Name", "sakura.mp4");

			using MediaFileTags tags = new (newFileName);

			bool result = tags.Clean();
			Assert.True(result);

			string album = tags.Album;
			Assert.IsNotEmpty(album);

			File.Delete(newFileName);

			string expected = "Album Name";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// The tag file update album from path with rules test.
		/// </summary>
		[Test]
		public void TagFileUpdateAlbumFromPathRules()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album Name", "sakura.mp4");

			using MediaFileTags tags = new (newFileName, rules);

			bool result = tags.Clean();
			Assert.True(result);

			string album = tags.Album;
			Assert.IsNotEmpty(album);

			File.Delete(newFileName);

			string expected = "Album Name";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// The tag file update album remove cd test.
		/// </summary>
		[Test]
		public void TagFileUpdateAlbumRemoveCd()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album cd 1", "sakura.mp4");

			using MediaFileTags tags = new (newFileName);
			tags.Album = "Album cd 1";

			bool result = tags.Clean();
			Assert.True(result);

			File.Delete(newFileName);

			string album = tags.Album;
			Assert.IsNotEmpty(album);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// The tag file update album remove cd with rules test.
		/// </summary>
		[Test]
		public void TagFileUpdateAlbumRemoveCdRules()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album cd 1", "sakura.mp4");

			using MediaFileTags tags = new (newFileName, rules);
			tags.Album = "Album cd 1";

			bool result = tags.Clean();
			Assert.True(result);

			File.Delete(newFileName);

			string album = tags.Album;
			Assert.IsNotEmpty(album);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// The tag file update album remove curly braces test.
		/// </summary>
		[Test]
		public void TagFileUpdateAlbumRemoveCurlyBraces()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album {In Heaven}", "sakura.mp4");

			using MediaFileTags tags = new (newFileName);
			tags.Album = "Album {In Heaven}";

			bool result = tags.Clean();
			Assert.True(result);

			File.Delete(newFileName);

			string album = tags.Album;
			Assert.IsNotEmpty(album);

			string expected = "Album [In Heaven]";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// The tag file update album remove curly braces with rules test.
		/// </summary>
		[Test]
		public void TagFileUpdateAlbumRemoveCurlyBracesRules()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album {In Heaven}", "sakura.mp4");

			using MediaFileTags tags = new (newFileName, rules);
			tags.Album = "Album {In Heaven}";

			bool result = tags.Clean();
			Assert.True(result);

			File.Delete(newFileName);

			string album = tags.Album;
			Assert.IsNotEmpty(album);

			string expected = "Album [In Heaven]";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// The tag file update album remove disc test.
		/// </summary>
		[Test]
		public void TagFileUpdateAlbumRemoveDisc()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album (Disk 2)", "sakura.mp4");

			using MediaFileTags tags = new (newFileName);
			tags.Album = "Album (Disk 2)";

			bool result = tags.Clean();
			Assert.True(result);

			File.Delete(newFileName);

			string album = tags.Album;
			Assert.IsNotEmpty(album);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// The tag file update album remove disc with rules test.
		/// </summary>
		[Test]
		public void TagFileUpdateAlbumRemoveDiscRules()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album (Disk 2)", "sakura.mp4");

			using MediaFileTags tags = new (newFileName, rules);
			tags.Album = "Album (Disk 2)";

			bool result = tags.Clean();
			Assert.True(result);

			File.Delete(newFileName);

			string album = tags.Album;
			Assert.IsNotEmpty(album);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// The tag file update album remove flac test.
		/// </summary>
		[Test]
		public void TagFileUpdateAlbumRemoveFlac()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album[FLAC]", "sakura.mp4");

			using MediaFileTags tags = new (newFileName);
			tags.Album = "Album[FLAC]";

			bool result = tags.Clean();
			Assert.True(result);

			File.Delete(newFileName);

			string album = tags.Album;
			Assert.IsNotEmpty(album);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// The tag file update album remove flac with rules test.
		/// </summary>
		[Test]
		public void TagFileUpdateAlbumRemoveFlacRules()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album[FLAC]", "sakura.mp4");

			using MediaFileTags tags = new (newFileName, rules);
			tags.Album = "Album[FLAC]";

			bool result = tags.Clean();
			Assert.True(result);

			File.Delete(newFileName);

			string album = tags.Album;
			Assert.IsNotEmpty(album);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// The tag file update artist from path test.
		/// </summary>
		[Test]
		public void TagFileUpdateArtistFromPath()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album Name", "sakura.mp4");
			using MediaFileTags tags = new (newFileName);

			bool result = tags.Clean();
			Assert.True(result);

			string artist = tags.Artist;
			Assert.IsNotEmpty(artist);

			File.Delete(newFileName);

			string expected = "Artist";
			Assert.That(artist, Is.EqualTo(expected));
		}

		/// <summary>
		/// The tag file update artist from path with rules test.
		/// </summary>
		[Test]
		public void TagFileUpdateArtistFromPathRules()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album Name", "sakura.mp4");
			using MediaFileTags tags = new (newFileName, rules);

			bool result = tags.Clean();
			Assert.True(result);

			string artist = tags.Artist;
			Assert.IsNotEmpty(artist);

			File.Delete(newFileName);

			string expected = "Artist";
			Assert.That(artist, Is.EqualTo(expected));
		}

		/// <summary>
		/// The tag file update change test.
		/// </summary>
		[Test]
		public void TagFileUpdateChange()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album (Disk 2)", "sakura.mp4");

			using MediaFileTags tags = new (newFileName);
			tags.Album = "Album (Disk 2)";

			bool result = tags.Clean();
			Assert.True(result);

			string album = tags.Album;
			Assert.IsNotEmpty(album);

			File.Delete(newFileName);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// The tag file update change with rules test.
		/// </summary>
		[Test]
		public void TagFileUpdateChangeRules()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album (Disk 2)", "sakura.mp4");

			using MediaFileTags tags = new (newFileName, rules);
			tags.Album = "Album (Disk 2)";

			bool result = tags.Clean();
			Assert.True(result);

			string album = tags.Album;
			Assert.IsNotEmpty(album);

			File.Delete(newFileName);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// The tag file update no change test.
		/// </summary>
		[Test]
		public void TagFileUpdateNoChange()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album Name", "sakura.mp4");
			using MediaFileTags tags = new (newFileName);
			tags.Artist = "Artist";
			tags.Album = "Album";
			tags.Title = "Sakura";
			tags.Update();

			tags.Album = "Album";
			bool result = tags.Update();
			Assert.False(result);

			string album = tags.Album;
			Assert.IsNotEmpty(album);

			File.Delete(newFileName);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// The tag file update no change with rules test.
		/// </summary>
		[Test]
		public void TagFileUpdateNoChangeRules()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album Name", "sakura.mp4");
			using MediaFileTags tags = new (newFileName, rules);
			tags.Artist = "Artist";
			tags.Album = "Album";
			tags.Title = "Sakura";
			tags.Update();

			tags.Album = "Album";
			bool result = tags.Clean();
			Assert.False(result);

			string album = tags.Album;
			Assert.IsNotEmpty(album);

			File.Delete(newFileName);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// The tag file update title from path test.
		/// </summary>
		[Test]
		public void TagFileUpdateTitleFromPath()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album Name", "sakura.mp4");
			using MediaFileTags tags = new (newFileName);

			bool result = tags.Clean();
			Assert.True(result);

			string title = tags.Title;
			Assert.IsNotEmpty(title);

			File.Delete(newFileName);

			string expected = "Sakura";
			Assert.That(title, Is.EqualTo(expected));
		}

		/// <summary>
		/// The tag file update title from path with rules test.
		/// </summary>
		[Test]
		public void TagFileUpdateTitleFromPathRules()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album Name", "sakura.mp4");
			using MediaFileTags tags = new (newFileName, rules);

			bool result = tags.Clean();
			Assert.True(result);

			string title = tags.Title;
			Assert.IsNotEmpty(title);

			File.Delete(newFileName);

			string expected = "Sakura";
			Assert.That(title, Is.EqualTo(expected));
		}

		/// <summary>
		/// The update file different test.
		/// </summary>
		[Test]
		public void UpdateFileDifferent()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album (Disk 2)", "sakura.mp4");

			using MediaFileTags tags = new (newFileName);

			var test = tags.TagFile.Tag;
			var test2 = test.Performers;

			// UpdateFile assumes tags have already been cleaned.
			tags.Album = "Album";
			tags.Artist = "Artist";
			tags.Title = "Sakura";
			tags.Update();

			FileInfo fileInfo = new (newFileName);

			fileInfo = MusicManager.UpdateFile(fileInfo);
			newFileName = fileInfo.FullName;

			// Clean up.
			File.Delete(newFileName);

			string basePath = Paths.GetBasePathFromFilePath(testFile);

			// Need to go 1 up actually.
			// basePath = Path.GetDirectoryName(basePath);
			string newBasePath =
				basePath + 2.ToString(CultureInfo.InvariantCulture);

			if (Directory.Exists(newBasePath))
			{
				Directory.Delete(newBasePath, true);
			}

			string expected = newBasePath + @"\Artist\Album\Sakura.mp4";

			Assert.That(newFileName, Is.EqualTo(expected));
		}

		/// <summary>
		/// The update file same test.
		/// </summary>
		[Test]
		public void UpdateFileSame()
		{
			using MediaFileTags tags = new (testFile);
			tags.Album = "Album";
			tags.Artist = "Artist";
			tags.Title = "Sakura";
			tags.Update();

			FileInfo fileInfo = new (testFile);

			fileInfo = MusicManager.UpdateFile(fileInfo);
			string newFileName = fileInfo.FullName;

			string basePath = Paths.GetBasePathFromFilePath(testFile);

			string expected =
				Path.Combine(basePath, @"Artist\Album\Sakura.mp4");

			Assert.That(newFileName, Is.EqualTo(expected));
		}

		/// <summary>
		/// Dispose method.
		/// </summary>
		/// <param name="disposing">Indicates whether currently disposing
		/// or not.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// dispose managed resources
				if (musicManager != null)
				{
					musicManager.Dispose();
					musicManager = null;
				}
			}
		}

		/// <summary>
		/// Make test file copy.
		/// </summary>
		/// <param name="directory">The directory to create.</param>
		/// <param name="fileName">The file name to create.</param>
		/// <returns>The file path of the copy file.</returns>
		private string MakeTestFileCopy(string directory, string fileName)
		{
			string newPath = temporaryPath + directory;
			Directory.CreateDirectory(newPath);

			string newFileName = newPath + @"\" + fileName;

			File.Copy(testFile, newFileName);

			return newFileName;
		}
	}
}
