/////////////////////////////////////////////////////////////////////////////
// <copyright file="MediaFileTagTests.cs" company="Digital Zen Works">
// Copyright © 2019 - 2023 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using DigitalZenWorks.Common.Utilities;
using DigitalZenWorks.RulesLibrary;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Collections.Generic;
using System.IO;

namespace DigitalZenWorks.MusicToolKit.Tests
{
	/// <summary>
	/// Media file tag tests class.
	/// </summary>
	[TestFixture]
	public class MediaFileTagTests : BaseTestsSupport
	{
		private Rules rules;

		/// <summary>
		/// The setup before every test method.
		/// </summary>
		[SetUp]
		public void SetUp()
		{
			FileUtils.CreateFileFromEmbeddedResource(
				"DigitalZenWorks.MusicToolKit.Tests.Sakura.mp4", TestFile);

			rules = MusicManager.GetDefaultRules();
		}

		/// <summary>
		/// The run rule disc check method.
		/// </summary>
		[Test]
		public void MediaFileGetTagsCheck()
		{
			using MediaFileTags tags = new (TestFile, rules);

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
		/// Tag file Album remove cd method test.
		/// </summary>
		[Test]
		public void TagFileAlbumRemoveCd()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album cd 1", "Sakura.mp4");

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
		/// Tag file Album remove cd no change test.
		/// </summary>
		[Test]
		public void TagFileInstanceAlbumRemoveCdNoChange()
		{
			string original = "Album";
			using MediaFileTags tags = new (TestFile);
			tags.Album = original;

			bool result = tags.Clean();
			Assert.False(result);

			string album = tags.Album;
			Assert.IsNotEmpty(album);

			Assert.That(album, Is.EqualTo(original));
		}

		/// <summary>
		/// Tag file Album remove disc method test.
		/// </summary>
		[Test]
		public void TagFileAlbumRemoveDisc()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album (Disk 2)", "Sakura.mp4");

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
		/// Tag file Album remove cd no change test.
		/// </summary>
		[Test]
		public void TagFileAlbumRemoveDiscNoChange()
		{
			string original = "Album";
			using MediaFileTags tags = new (TestFile);
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
		/// The tag file update album from path test.
		/// </summary>
		[Test]
		public void TagFileUpdateAlbumFromPath()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album Name", "Sakura.mp4");

			using MediaFileTags tags = new (newFileName);

			bool result = tags.Clean();
			Assert.False(result);

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
				MakeTestFileCopy(@"\Artist\Album Name", "Sakura.mp4");

			using MediaFileTags tags = new (newFileName, rules);

			bool result = tags.Clean();
			Assert.False(result);

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
			Assert.False(result);

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
				MakeTestFileCopy(@"\Artist\Album Name", "Sakura.mp4");
			using MediaFileTags tags = new (newFileName, rules);

			bool result = tags.Clean();
			Assert.False(result);

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
				MakeTestFileCopy(@"\Artist\Album Name", "Sakura.mp4");
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
				MakeTestFileCopy(@"\Artist\Album Name", "Sakura.mp4");
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
				MakeTestFileCopy(@"\Artist\Album Name", "Sakura.mp4");
			using MediaFileTags tags = new (newFileName);

			bool result = tags.Clean();
			Assert.False(result);

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
				MakeTestFileCopy(@"\Artist\Album Name", "Sakura.mp4");
			using MediaFileTags tags = new (newFileName, rules);

			bool result = tags.Clean();
			Assert.False(result);

			string title = tags.Title;
			Assert.IsNotEmpty(title);

			File.Delete(newFileName);

			string expected = "Sakura";
			Assert.That(title, Is.EqualTo(expected));
		}
	}
}
