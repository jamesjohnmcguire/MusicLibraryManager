/////////////////////////////////////////////////////////////////////////////
// <copyright file="MediaFileTagTests.cs" company="Digital Zen Works">
// Copyright © 2019 - 2024 Digital Zen Works. All Rights Reserved.
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
		/// Get tags check test.
		/// </summary>
		[Test]
		public void GetTagsCheck()
		{
			using MediaFileTags tags = new (TestFile, rules);

			string original =
				"What It Is! Funky Soul And Rare Grooves (Disk 2)";
			tags.Album = original;

			tags.Artist = "The Solos";
			tags.Update();

			SortedDictionary<string, object> tagSet = tags.GetTags();

			Assert.That(tags, Is.Not.Null);
			Assert.That(tags.TagFile, Is.Not.Null);
			Assert.That(tagSet, Is.Not.Null);
		}

		/// <summary>
		/// Album remove cd test.
		/// </summary>
		[Test]
		public void AlbumRemoveCd()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album cd 1", "Sakura.mp4");

			using MediaFileTags tags = new (newFileName);
			tags.Album = "Album cd 1";

			bool result = tags.Clean();

			File.Delete(newFileName);

			Assert.That(result, Is.True);

			string album = tags.Album;
			Assert.That(album, Is.Not.Empty);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// Album remove cd no change test.
		/// </summary>
		[Test]
		public void AlbumRemoveCdNoChange()
		{
			string original = "Album";
			using MediaFileTags tags = new (TestFile);
			tags.Album = original;

			bool result = tags.Clean();
			Assert.That(result, Is.False);

			string album = tags.Album;
			Assert.That(album, Is.Not.Empty);

			Assert.That(album, Is.EqualTo(original));
		}

		/// <summary>
		/// Album remove disc test.
		/// </summary>
		[Test]
		public void AlbumRemoveDisc()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album (Disk 2)", "Sakura.mp4");

			using MediaFileTags tags = new (newFileName);
			string album = tags.Album = "Album (Disk 2)";

			bool result = tags.Clean();

			File.Delete(newFileName);

			Assert.That(result, Is.True);

			album = tags.Album;
			Assert.That(album, Is.Not.Empty);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// Album remove cd no change test.
		/// </summary>
		[Test]
		public void AlbumRemoveDiscNoChange()
		{
			string original = "Album";
			using MediaFileTags tags = new (TestFile);
			tags.Album = original;
			tags.Artist = "Artist";
			tags.Title = "Sakura";

			bool result = tags.Clean();
			Assert.That(result, Is.False);

			string album = tags.Album;
			Assert.That(album, Is.Not.Empty);

			Assert.That(album, Is.EqualTo(original));
		}

		/// <summary>
		/// Update album from path test.
		/// </summary>
		[Test]
		public void UpdateAlbumFromPath()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album Name", "Sakura.mp4");

			using MediaFileTags tags = new (newFileName);

			bool result = tags.Clean();
			Assert.That(result, Is.False);

			string album = tags.Album;
			Assert.That(album, Is.Not.Empty);

			File.Delete(newFileName);

			string expected = "Album Name";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// Update album from path with rules test.
		/// </summary>
		[Test]
		public void UpdateAlbumFromPathRules()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album Name", "Sakura.mp4");

			using MediaFileTags tags = new (newFileName, rules);

			bool result = tags.Clean();
			Assert.That(result, Is.False);

			string album = tags.Album;
			Assert.That(album, Is.Not.Empty);

			File.Delete(newFileName);

			string expected = "Album Name";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// Update album remove cd test.
		/// </summary>
		[Test]
		public void UpdateAlbumRemoveCd()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album cd 1", "Sakura.mp4");

			using MediaFileTags tags = new (newFileName);
			tags.Album = "Album cd 1";

			bool result = tags.Clean();
			Assert.That(result, Is.True);

			File.Delete(newFileName);

			string album = tags.Album;
			Assert.That(album, Is.Not.Empty);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// Update album remove cd with rules test.
		/// </summary>
		[Test]
		public void UpdateAlbumRemoveCdRules()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album cd 1", "Sakura.mp4");

			using MediaFileTags tags = new (newFileName, rules);
			tags.Album = "Album cd 1";

			bool result = tags.Clean();
			Assert.That(result, Is.True);

			File.Delete(newFileName);

			string album = tags.Album;
			Assert.That(album, Is.Not.Empty);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// Update album remove curly braces test.
		/// </summary>
		[Test]
		public void UpdateAlbumRemoveCurlyBraces()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album {In Heaven}", "Sakura.mp4");

			using MediaFileTags tags = new (newFileName);
			tags.Album = "Album {In Heaven}";

			bool result = tags.Clean();
			Assert.That(result, Is.True);

			File.Delete(newFileName);

			string album = tags.Album;
			Assert.That(album, Is.Not.Empty);

			string expected = "Album [In Heaven]";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// Update album remove curly braces with rules test.
		/// </summary>
		[Test]
		public void UpdateAlbumRemoveCurlyBracesRules()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album {In Heaven}", "Sakura.mp4");

			using MediaFileTags tags = new (newFileName, rules);
			tags.Album = "Album {In Heaven}";

			bool result = tags.Clean();
			Assert.That(result, Is.True);

			File.Delete(newFileName);

			string album = tags.Album;
			Assert.That(album, Is.Not.Empty);

			string expected = "Album [In Heaven]";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// Update album remove disc test.
		/// </summary>
		[Test]
		public void UpdateAlbumRemoveDisc()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album (Disk 2)", "Sakura.mp4");

			using MediaFileTags tags = new (newFileName);
			tags.Album = "Album (Disk 2)";

			bool result = tags.Clean();
			Assert.That(result, Is.True);

			File.Delete(newFileName);

			string album = tags.Album;
			Assert.That(album, Is.Not.Empty);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// Update album remove disc with rules test.
		/// </summary>
		[Test]
		public void UpdateAlbumRemoveDiscRules()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album (Disk 2)", "Sakura.mp4");

			using MediaFileTags tags = new (newFileName, rules);
			tags.Album = "Album (Disk 2)";

			bool result = tags.Clean();
			Assert.That(result, Is.True);

			File.Delete(newFileName);

			string album = tags.Album;
			Assert.That(album, Is.Not.Empty);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// Update album remove flac test.
		/// </summary>
		[Test]
		public void UpdateAlbumRemoveFlac()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album[FLAC]", "Sakura.mp4");

			using MediaFileTags tags = new (newFileName);
			tags.Album = "Album[FLAC]";

			bool result = tags.Clean();
			Assert.That(result, Is.True);

			File.Delete(newFileName);

			string album = tags.Album;
			Assert.That(album, Is.Not.Empty);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// Update album remove flac with rules test.
		/// </summary>
		[Test]
		public void UpdateAlbumRemoveFlacRules()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album[FLAC]", "Sakura.mp4");

			using MediaFileTags tags = new (newFileName, rules);
			tags.Album = "Album[FLAC]";

			bool result = tags.Clean();
			Assert.That(result, Is.True);

			File.Delete(newFileName);

			string album = tags.Album;
			Assert.That(album, Is.Not.Empty);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// Update artist from path test.
		/// </summary>
		[Test]
		public void UpdateArtistFromPath()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album Name", "Sakura.mp4");
			using MediaFileTags tags = new (newFileName);

			bool result = tags.Clean();
			Assert.That(result, Is.False);

			string artist = tags.Artist;
			Assert.That(artist, Is.Not.Empty);

			File.Delete(newFileName);

			string expected = "Artist";
			Assert.That(artist, Is.EqualTo(expected));
		}

		/// <summary>
		/// Update artist from path with rules test.
		/// </summary>
		[Test]
		public void UpdateArtistFromPathRules()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album Name", "Sakura.mp4");
			using MediaFileTags tags = new (newFileName, rules);

			bool result = tags.Clean();
			Assert.That(result, Is.False);

			string artist = tags.Artist;
			Assert.That(artist, Is.Not.Empty);

			File.Delete(newFileName);

			string expected = "Artist";
			Assert.That(artist, Is.EqualTo(expected));
		}

		/// <summary>
		/// Update change test.
		/// </summary>
		[Test]
		public void UpdateChange()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album (Disk 2)", "Sakura.mp4");

			using MediaFileTags tags = new (newFileName);
			tags.Album = "Album (Disk 2)";

			bool result = tags.Clean();
			Assert.That(result, Is.True);

			string album = tags.Album;
			Assert.That(album, Is.Not.Empty);

			File.Delete(newFileName);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// Update change with rules test.
		/// </summary>
		[Test]
		public void UpdateChangeRules()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album (Disk 2)", "Sakura.mp4");

			using MediaFileTags tags = new (newFileName, rules);
			tags.Album = "Album (Disk 2)";

			bool result = tags.Clean();
			Assert.That(result, Is.True);

			string album = tags.Album;
			Assert.That(album, Is.Not.Empty);

			File.Delete(newFileName);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// Update no change test.
		/// </summary>
		[Test]
		public void UpdateNoChange()
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
			Assert.That(result, Is.False);

			string album = tags.Album;
			Assert.That(album, Is.Not.Empty);

			File.Delete(newFileName);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// Update no change with rules test.
		/// </summary>
		[Test]
		public void UpdateNoChangeRules()
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
			Assert.That(result, Is.False);

			string album = tags.Album;
			Assert.That(album, Is.Not.Empty);

			File.Delete(newFileName);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// Update title from path test.
		/// </summary>
		[Test]
		public void UpdateTitleFromPath()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album Name", "Sakura.mp4");
			using MediaFileTags tags = new (newFileName);

			bool result = tags.Clean();
			Assert.That(result, Is.False);

			string title = tags.Title;
			Assert.That(title, Is.Not.Empty);

			File.Delete(newFileName);

			string expected = "Sakura";
			Assert.That(title, Is.EqualTo(expected));
		}

		/// <summary>
		/// Update title from path with rules test.
		/// </summary>
		[Test]
		public void UpdateTitleFromPathRules()
		{
			string newFileName =
				MakeTestFileCopy(@"\Artist\Album Name", "Sakura.mp4");
			using MediaFileTags tags = new (newFileName, rules);

			bool result = tags.Clean();
			Assert.That(result, Is.False);

			string title = tags.Title;
			Assert.That(title, Is.Not.Empty);

			File.Delete(newFileName);

			string expected = "Sakura";
			Assert.That(title, Is.EqualTo(expected));
		}
	}
}
