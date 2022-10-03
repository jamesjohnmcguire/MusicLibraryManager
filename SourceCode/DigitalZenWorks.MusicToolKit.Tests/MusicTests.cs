/////////////////////////////////////////////////////////////////////////////
// <copyright file="MusicTests.cs" company="Digital Zen Works">
// Copyright © 2019 - 2022 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.IO;

namespace DigitalZenWorks.MusicToolKit.Tests
{
	/// <summary>
	/// Unit tests class.
	/// </summary>
	[TestFixture]
	public class MusicTests : TestsBase
	{
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

			album = MediaFileTags.AlbumRemoveCd(album);

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

			album = MediaFileTags.AlbumRemoveCd(album);

			Assert.IsNotEmpty(album);

			Assert.That(album, Is.EqualTo(original));
		}

		/// <summary>
		/// Album replace curly braces method test.
		/// </summary>
		[Test]
		public void AlbumReplaceCurlyBraces()
		{
			string album = "Something {In Heaven}";

			album = MediaFileTags.AlbumReplaceCurlyBraces(album);

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

			album = MediaFileTags.AlbumReplaceCurlyBraces(album);

			Assert.That(album, Is.EqualTo(original));
		}

		/// <summary>
		/// Album remove disc method test.
		/// </summary>
		[Test]
		public void AlbumRemoveDisc()
		{
			string album = "What It Is! Funky Soul And Rare Grooves (Disk 2)";

			album = MediaFileTags.AlbumRemoveDisc(album);

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

			album = MediaFileTags.AlbumRemoveDisc(album);

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
		/// The create album path from tag success test.
		/// </summary>
		[Test]
		public void CreateAlbumPathFromTagSuccess()
		{
			string artistPath = Paths.GetArtistPathFromFilePath(TestFile);

			string path =
				MusicManager.CreateAlbumPathFromTag(artistPath, "Album");

			Assert.IsNotEmpty(path);

			bool exists = Directory.Exists(path);
			Assert.True(exists);

			FileInfo fileInfo = new (path);

			string albumPart = fileInfo.Name;

			string expected = "Album";
			Assert.That(albumPart, Is.EqualTo(expected));
		}

		/// <summary>
		/// The create artist path from tag success test.
		/// </summary>
		[Test]
		public void CreateArtistPathFromTagSuccess()
		{
			FileInfo fileInfo = new (TestFile);
			string path =
				MusicManager.CreateArtistPathFromTag(fileInfo, "Artist");

			Assert.IsNotEmpty(path);

			bool exists = Directory.Exists(path);
			Assert.True(exists);

			fileInfo = new (path);

			string artistPart = fileInfo.Name;

			string expected = "Artist";
			Assert.That(artistPart, Is.EqualTo(expected));
		}

		/// <summary>
		/// The get duplicate location test.
		/// </summary>
		[Test]
		public void GetDuplicateLocation()
		{
			using MusicManager musicUtility = new ();
			string location = musicUtility.ITunesLibraryLocation;

			string fileName = @"Music\10cc\The Very Best Of 10cc\" +
				"The Things We Do For Love.mp3";
			string fullPath = Path.Combine(location, fileName);

			string duplicateLocation =
				musicUtility.GetDuplicateLocation(fullPath);

			bool contains = duplicateLocation.Contains(
				"Music2", StringComparison.Ordinal);

			Assert.True(contains);
		}

		/// <summary>
		/// The get itunes path depth method test.
		/// </summary>
		[Test]
		public void GetItunesPathDepth()
		{
			using MusicManager musicUtility = new ();
			string location = musicUtility.ITunesLibraryLocation;
			int iTunesDepth = Paths.GetItunesDirectoryDepth(location);

			Assert.GreaterOrEqual(iTunesDepth, 6);
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

			string album = tags.AlbumRemoveCd();

			File.Delete(newFileName);

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
			using MediaFileTags tags = new (TestFile);
			tags.Album = original;

			string album = tags.AlbumRemoveCd();

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
			tags.Album = "Album (Disk 2)";

			string album = tags.AlbumRemoveDisc();

			File.Delete(newFileName);

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
			using MediaFileTags tags = new (TestFile);
			tags.Album = original;

			string album = tags.AlbumRemoveDisc();

			Assert.IsNotEmpty(album);

			Assert.That(album, Is.EqualTo(original));
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
			content = MediaFileTags.RegexRemove(pattern, content);

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
			content = MediaFileTags.RegexRemove(pattern, content);

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
			content = MediaFileTags.RegexRemove(pattern, content);

			Assert.That(content, Is.EqualTo(original));
		}

		/// <summary>
		/// The save tags to json file success test.
		/// </summary>
		[Test]
		public void SaveTagsToJsonFileSuccess()
		{
			using MusicManager musicUtility = new ();

			FileInfo fileInfo = new (TestFile);
			string destinationPath = Path.GetDirectoryName(TestFile);
			bool result =
				musicUtility.SaveTagsToJsonFile(fileInfo, destinationPath);

			Assert.True(result);
			string destinationFile =
				destinationPath + "\\" + fileInfo.Name + ".json";

			result = File.Exists(destinationFile);
			Assert.True(result);
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

			bool result = tags.Update();
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

			bool result = tags.Update();
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

			bool result = tags.Update();
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

			bool result = tags.Update();
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

			bool result = tags.Update();
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

			bool result = tags.Update();
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

			bool result = tags.Update();
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
		/// The tag file update title from path test.
		/// </summary>
		[Test]
		public void TagFileUpdateTitleFromPath()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album Name", "sakura.mp4");
			using MediaFileTags tags = new (newFileName);

			bool result = tags.Update();
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
			using MusicManager musicUtility = new ();

			string newFileName =
				MakeTestFileCopy(@"\Artist\Album (Disk 2)", "sakura.mp4");

			MediaFileTags tags = new (newFileName);

			// UpdateFile assumes tags have already been cleaned.
			tags.Album = "Album";
			tags.Artist = "Artist";
			tags.Title = "Sakura";

			musicUtility.Tags = tags;

			FileInfo fileInfo = new (newFileName);

			fileInfo = musicUtility.UpdateFile(fileInfo);
			newFileName = fileInfo.FullName;

			// Clean up.
			File.Delete(newFileName);

			string basePath = Paths.GetBasePathFromFilePath(TestFile);

			// Need to go 1 up actually.
			basePath = Path.GetDirectoryName(basePath);
			string temporaryMusicPath = Path.Combine(basePath, "Music2");
			Directory.Delete(temporaryMusicPath, true);

			string expected = basePath + @"\Music2\Artist\Album\Sakura.mp4";

			Assert.That(newFileName, Is.EqualTo(expected));
		}

		/// <summary>
		/// The update file same test.
		/// </summary>
		[Test]
		public void UpdateFileSame()
		{
			using MusicManager musicUtility = new ();

			MediaFileTags tags = new (TestFile);
			tags.Album = "Album";
			tags.Artist = "Artist";
			tags.Title = "Sakura";

			musicUtility.Tags = tags;

			FileInfo fileInfo = new (TestFile);

			fileInfo = musicUtility.UpdateFile(fileInfo);
			string newFileName = fileInfo.FullName;

			string basePath = Paths.GetBasePathFromFilePath(TestFile);

			string expected =
				Path.Combine(basePath, @"Artist\Album\Sakura.mp4");

			Assert.That(newFileName, Is.EqualTo(expected));
		}
	}
}
