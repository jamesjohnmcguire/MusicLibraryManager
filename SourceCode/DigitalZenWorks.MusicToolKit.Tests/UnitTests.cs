/////////////////////////////////////////////////////////////////////////////
// <copyright file="UnitTests.cs" company="Digital Zen Works">
// Copyright © 2019 - 2022 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Common.Logging;
using DigitalZenWorks.Common.Utilities;
using iTunesLib;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

using CommonLogging = Common.Logging;

[assembly: CLSCompliant(true)]

namespace DigitalZenWorks.MusicToolKit.Tests
{
	/// <summary>
	/// Unit tests class.
	/// </summary>
	[TestFixture]
	public class UnitTests
	{
		private static readonly ILog Log = LogManager.GetLogger(
			System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private TagSet tags;
		private string temporaryPath;
		private string testFile;

		/// <summary>
		/// Initializes a new instance of the <see cref="UnitTests"/> class.
		/// </summary>
		public UnitTests()
		{
		}

		/// <summary>
		/// The one time setup method.
		/// </summary>
		[SetUp]
		public void OneTimeSetUp()
		{
			LogInitialization();

			temporaryPath = Path.GetTempFileName();
			File.Delete(temporaryPath);
			Directory.CreateDirectory(temporaryPath);

			testFile = temporaryPath + @"\Artist\Album\sakura.mp4";
			FileUtils.CreateFileFromEmbeddedResource(
				"DigitalZenWorks.MusicToolKit.Tests.sakura.mp4", testFile);

			tags = new ();

			string original =
				"What It Is! Funky Soul And Rare Grooves (Disk 2)";
			tags.Album = original;

			tags.Artists = new string[1];
			tags.Performers = new string[1];
			tags.Artists[0] = "Various Artists";
			tags.Performers[0] = "The Solos";
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
		/// The are file and track the same yes test.
		/// </summary>
		[Test]
		public void AreFileAndTrackTheSameYes()
		{
			using MusicManager musicUtility = new ();

			iTunesApp iTunes = musicUtility.ItunesCom;

			if (iTunes != null)
			{
				string searchName = "The Things We Do For Love";

				IITLibraryPlaylist playList = iTunes.LibraryPlaylist;
				IITTrackCollection tracks = playList.Search(
					searchName,
					ITPlaylistSearchField.ITPlaylistSearchFieldAll);

				Assert.NotNull(tracks);

				if (null != tracks)
				{
					string fileName = musicUtility.ITunesLibraryLocation +
						@"Music\10cc\The Very Best Of 10cc\" +
						"The Things We Do For Love.mp3";

					// tracks is a list of potential matches
					foreach (IITTrack track in tracks)
					{
						bool same = MusicManager.AreFileAndTrackTheSame(
							fileName, track);
						Assert.True(same);
					}
				}
			}
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
			string artistPath = Paths.GetArtistPathFromFilePath(testFile);

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
			FileInfo fileInfo = new (testFile);
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
		/// The get default rules method test.
		/// </summary>
		[Test]
		public void GetDefaultRules()
		{
			Rules rules = GetRules();

			for (int index = 0; index < rules.RulesList.Count; index++)
			{
				Rule rule = rules.RulesList[index];

				bool result = rule.Run(tags);

				Assert.True(result);

				if (index == 0)
				{
					string test = tags.Album;
					Assert.That(test, Is.EqualTo(
						"What It Is! Funky Soul And Rare Grooves"));
				}
				else
				{
					string test = tags.Artists[0];

					Assert.That(test, Is.EqualTo("The Solos"));
				}
			}
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
		/// The get rules list method test.
		/// </summary>
		[Test]
		public void GetRulesList()
		{
			using MusicManager musicUtility = new ();

			Rules rules = musicUtility.Rules;
			Assert.NotNull(rules);

			IList<Rule> rulesList = rules.RulesList;
			int count = rulesList.Count;

			Assert.GreaterOrEqual(count, 1);
		}

		/// <summary>
		/// Instance Album remove cd method test.
		/// </summary>
		[Test]
		public void InstanceAlbumRemoveCd()
		{
			string newPath =
				temporaryPath + @"\Artist\Album cd 1";
			Directory.CreateDirectory(newPath);

			string newFileName =
				temporaryPath + @"\Artist\Album cd 1\sakura.mp4";

			File.Copy(testFile, newFileName);

			using MediaFileTags tags = new (newFileName);
			tags.Album = "Album cd 1";

			string album = tags.AlbumRemoveCd();

			File.Delete(newFileName);

			Log.Info("album: " + album);
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

			string album = tags.AlbumRemoveCd();

			Log.Info("album: " + album);
			Assert.IsNotEmpty(album);

			Assert.That(album, Is.EqualTo(original));
		}

		/// <summary>
		/// Instance Album remove disc method test.
		/// </summary>
		[Test]
		public void InstanceAlbumRemoveDisc()
		{
			string newPath =
				temporaryPath + @"\Artist\Album (Disk 2)";
			Directory.CreateDirectory(newPath);

			string newFileName =
				temporaryPath + @"\Artist\Album (Disk 2)\sakura.mp4";

			File.Copy(testFile, newFileName);

			using MediaFileTags tags = new (newFileName);
			tags.Album = "Album (Disk 2)";

			string album = tags.AlbumRemoveDisc();

			Log.Info("album: " + album);
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

			string album = tags.AlbumRemoveDisc();

			Log.Info("album: " + album);
			Assert.IsNotEmpty(album);

			Assert.That(album, Is.EqualTo(original));
		}

		/// <summary>
		/// ITunes path location method test.
		/// </summary>
		[Test]
		public void ITunesPathLocation()
		{
			using MusicManager musicUtility = new ();
			string location = musicUtility.ITunesLibraryLocation;

			Log.Info("ITunesPathLocation: " + location);
			Assert.IsNotEmpty(location);
		}

		/// <summary>
		/// iTunes Xml Create with invalid file test.
		/// </summary>
		[Test]
		public void ItunesXmlFileCreateInvalidFile()
		{
			ITunesXmlFile iTunesXmlFile = null;

			string nonExistantFilePath = Path.GetTempFileName();
			File.Delete(nonExistantFilePath);

			FileNotFoundException exception =
				Assert.Throws<FileNotFoundException>(() =>
				iTunesXmlFile = new ITunesXmlFile(nonExistantFilePath));

			Assert.NotNull(exception);

			Assert.Null(iTunesXmlFile);
		}

		/// <summary>
		/// iTunes Xml Create success test.
		/// </summary>
		[Test]
		public void ItunesXmlFileCreateSuccess()
		{
			string xmlFile = Path.GetTempFileName();
			File.Delete(xmlFile);

			FileUtils.CreateFileFromEmbeddedResource(
				"DigitalZenWorks.MusicToolKit.Tests.XMLFile.xml", xmlFile);

			ITunesXmlFile iTunesXmlFile = new (xmlFile);

			Assert.NotNull(iTunesXmlFile);
		}

		/// <summary>
		/// Load iTunes XML file method test.
		/// </summary>
		[Test]
		public void LoadiTunesXmlFile()
		{
			using MusicManager musicUtility = new ();
			string xmlFilePath = musicUtility.ITunesLibraryXMLPath;

			Dictionary<string, object> result =
				ITunesXmlFile.LoadItunesXmlFile(xmlFilePath);

			Assert.NotNull(result);

			int count = result.Count;
			Assert.GreaterOrEqual(count, 1);
		}

		/// <summary>
		/// Load iTunes XML file not exists test.
		/// </summary>
		[Test]
		public void LoadiTunesXmlFileNotExists()
		{
			string temporaryPath = Path.GetTempFileName();
			File.Delete(temporaryPath);

			Dictionary<string, object> result =
				ITunesXmlFile.LoadItunesXmlFile(temporaryPath);

			Assert.Null(result);
		}

		/// <summary>
		/// Load iTunes XML file not xml filetest.
		/// </summary>
		[Test]
		public void LoadiTunesXmlFileNotXmlFile()
		{
			string temporaryPath = Path.GetTempFileName();

			Dictionary<string, object> result =
				ITunesXmlFile.LoadItunesXmlFile(temporaryPath);

			Assert.Null(result);
		}

		/// <summary>
		/// The run rule disc check method.
		/// </summary>
		[Test]
		public void MediaFileTagsCheck()
		{
			Rules rules = GetRules();

			using MediaFileTags tags = new (testFile, rules);

			Assert.NotNull(tags);
			Assert.NotNull(tags.TagFile);
			Assert.NotNull(tags.TagSet);
		}

		/// <summary>
		/// The MusicManager check for rules test.
		/// </summary>
		[Test]
		public void MusicManagerCheckForRules()
		{
			using MusicManager musicManager = new ();

			Rules rules = musicManager.Rules;

			Assert.NotNull(rules);
		}

		/// <summary>
		/// The MusicManager check for rules test.
		/// </summary>
		[Test]
		public void MusicManagerCheckForSameRules()
		{
			Rules rules = GetRules();

			Rule rule = new (
				"DigitalZenWorks.MusicToolKit.Tags.Album",
				Condition.ContainsRegex,
				@"\s*\(Disk).*?\)",
				Operation.Remove);

			rules.RulesList.Add(rule);

			using MusicManager musicManager = new (rules);

			Rules rules2 = musicManager.Rules;

			int count1 = rules.RulesList.Count;
			int count2 = rules2.RulesList.Count;
			Assert.AreEqual(count1, count2);
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
		/// The run rule disc check method.
		/// </summary>
		[Test]
		public void RunRuleDiscCheck()
		{
			string element = "DigitalZenWorks.MusicToolKit.Tags.Album";

			Rule rule = new (
				element,
				Condition.ContainsRegex,
				@"\s*\(Dis(c|k).*?\)",
				Operation.Remove);

			bool result = rule.Run(tags);

			Assert.True(result);

			string test = tags.Album;
			Assert.That(test, Is.EqualTo(
				"What It Is! Funky Soul And Rare Grooves"));
		}

		/// <summary>
		/// The run rule disc check by name method.
		/// </summary>
		[Test]
		public void RunRuleDiscCheckByName()
		{
			using MusicManager musicUtility = new ();

			Rules rules = musicUtility.Rules;
			Assert.NotNull(rules);

			Rule rule = rules.GetRuleByName("RemoveDiscFromAlbum");
			Assert.NotNull(rule);

			bool result = rule.Run(tags);

			Assert.True(result);

			string test = tags.Album;
			Assert.That(test, Is.EqualTo(
				"What It Is! Funky Soul And Rare Grooves"));
		}

		/// <summary>
		/// The run rule various artists check method test.
		/// </summary>
		[Test]
		public void RunRuleVariousArtistsCheck()
		{
			string original = "Various Artists";
			string element = "Artists";

			// Set up final rule - and if artists not equal performers,
			// replace artists with performers
			Rule nextChainRule = new (
				"Artists",
				Condition.NotEquals,
				"Performers",
				Operation.Replace,
				"Performers");

			nextChainRule.ConditionalType = ConditionalType.Property;

			// Set up additional rule - and if performers tag is not empty,
			Rule chainRule = new (
				"DigitalZenWorks.MusicToolKit.Tags.TagFile.Tag.Performers",
				Condition.NotEmpty,
				original,
				Operation.None,
				null,
				Chain.And,
				nextChainRule);

			// Set up initial rule - if artists tag equal 'Various Artists'
			Rule rule = new (
				element,
				Condition.Equals,
				original,
				Operation.None,
				null,
				Chain.And,
				chainRule);

			bool result = rule.Run(tags);

			Assert.True(result);

			string test = tags.Artists[0];

			Assert.That(test, Is.EqualTo("The Solos"));
		}

		/// <summary>
		/// The run rule various artists check by name method test.
		/// </summary>
		[Test]
		public void RunRuleVariousArtistsCheckByName()
		{
			using MusicManager musicUtility = new ();

			Rules rules = musicUtility.Rules;
			Assert.NotNull(rules);

			Rule rule = rules.GetRuleByName("ReplaceVariousArtists");
			Assert.NotNull(rule);

			bool result = rule.Run(tags);

			Assert.True(result);

			string test = tags.Artists[0];

			Assert.That(test, Is.EqualTo("The Solos"));
		}

		/// <summary>
		/// The save tags to json file success test.
		/// </summary>
		[Test]
		public void SaveTagsToJsonFile()
		{
			using MusicManager musicUtility = new ();

			string temporaryFile = Path.GetTempFileName();
			FileInfo fileInfo = new (temporaryFile);
			string destinationPath = Path.GetDirectoryName(temporaryFile);
			bool result =
				musicUtility.SaveTagsToJsonFile(fileInfo, destinationPath);

			Assert.False(result);
		}

		/// <summary>
		/// The save tags to json file success test.
		/// </summary>
		[Test]
		public void SaveTagsToJsonFileSuccess()
		{
			using MusicManager musicUtility = new ();

			FileInfo fileInfo = new (testFile);
			string destinationPath = Path.GetDirectoryName(testFile);
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
			using MediaFileTags tags = new (testFile);

			bool result = tags.Update();
			Assert.True(result);

			string album = tags.Album;
			Assert.IsNotEmpty(album);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// The tag file update album remove cd test.
		/// </summary>
		[Test]
		public void TagFileUpdateAlbumRemoveCd()
		{
			string newPath =
				temporaryPath + @"\Artist\Album cd 1";
			Directory.CreateDirectory(newPath);

			string newFileName =
				temporaryPath + @"\Artist\Album cd 1\sakura.mp4";

			File.Copy(testFile, newFileName);

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
			string newPath =
				temporaryPath + @"\Artist\Album {In Heaven}";
			Directory.CreateDirectory(newPath);

			string newFileName =
				temporaryPath + @"\Artist\Album {In Heaven}\sakura.mp4";

			File.Copy(testFile, newFileName);

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
			string newPath =
				temporaryPath + @"\Artist\Album (Disk 2)";
			Directory.CreateDirectory(newPath);

			string newFileName =
				temporaryPath + @"\Artist\Album (Disk 2)\sakura.mp4";

			File.Copy(testFile, newFileName);

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
			string newPath =
				temporaryPath + @"\Artist\Album[FLAC]";
			Directory.CreateDirectory(newPath);

			string newFileName =
				temporaryPath + @"\Artist\Album[FLAC]\sakura.mp4";

			File.Copy(testFile, newFileName);

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
			using MediaFileTags tags = new (testFile);

			bool result = tags.Update();
			Assert.True(result);

			string artist = tags.Artist;
			Assert.IsNotEmpty(artist);

			string expected = "Artist";
			Assert.That(artist, Is.EqualTo(expected));
		}

		/// <summary>
		/// The tag file update change test.
		/// </summary>
		[Test]
		public void TagFileUpdateChange()
		{
			string newPath =
				temporaryPath + @"\Artist\Album (Disk 2)";
			Directory.CreateDirectory(newPath);

			string newFileName =
				temporaryPath + @"\Artist\Album (Disk 2)\sakura.mp4";

			File.Copy(testFile, newFileName);

			using MediaFileTags tags = new (newFileName);
			tags.Album = "Album (Disk 2)";

			bool result = tags.Update();
			Assert.True(result);

			string album = tags.Album;
			Assert.IsNotEmpty(album);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// The tag file update no change test.
		/// </summary>
		[Test]
		public void TagFileUpdateNoChange()
		{
			using MediaFileTags tags = new (testFile);
			tags.Artist = "Artist";
			tags.Album = "Album";
			tags.Title = "Sakura";
			tags.Update();

			tags.Album = "Album";
			bool result = tags.Update();
			Assert.False(result);

			string album = tags.Album;
			Assert.IsNotEmpty(album);

			string expected = "Album";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// The tag file update title from path test.
		/// </summary>
		[Test]
		public void TagFileUpdateTitleFromPath()
		{
			using MediaFileTags tags = new (testFile);

			bool result = tags.Update();
			Assert.True(result);

			string title = tags.Title;
			Assert.IsNotEmpty(title);

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

			string newPath =
				temporaryPath + @"\Artist\Album (Disk 2)";
			Directory.CreateDirectory(newPath);

			string newFileName =
				temporaryPath + @"\Artist\Album (Disk 2)\sakura.mp4";

			File.Copy(testFile, newFileName);

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
			string basePath = Paths.GetBasePathFromFilePath(testFile);

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

			MediaFileTags tags = new (testFile);
			tags.Album = "Album";
			tags.Artist = "Artist";
			tags.Title = "Sakura";

			musicUtility.Tags = tags;

			FileInfo fileInfo = new (testFile);

			fileInfo = musicUtility.UpdateFile(fileInfo);
			string newFileName = fileInfo.FullName;

			string basePath = Paths.GetBasePathFromFilePath(testFile);

			string expected =
				Path.Combine(basePath, @"Artist\Album\Sakura.mp4");

			Assert.That(newFileName, Is.EqualTo(expected));
		}

		/// <summary>
		/// The update iTunes test.
		/// </summary>
		[Test]
		public void UpdateItunes()
		{
			using MusicManager musicUtility = new ();

			string location = musicUtility.ITunesLibraryLocation;

			string fileName = @"Music\10cc\The Very Best Of 10cc\" +
				"The Things We Do For Love.mp3";
			string fullPath = Path.Combine(location, fileName);
			FileInfo fileInfo = new (fullPath);

			bool updated = musicUtility.UpdateItunes(fileInfo);

			Assert.False(updated);
		}

		private static Rules GetRules()
		{
			string resourceName =
				"DigitalZenWorks.MusicToolKit.DefaultRules.json";
			Assembly assembly = typeof(MusicManager).Assembly;

			using Stream templateObjectStream =
				assembly.GetManifestResourceStream(resourceName);

			Assert.NotNull(templateObjectStream);

			using StreamReader reader = new (templateObjectStream);
			string contents = reader.ReadToEnd();

			Rules rules = new (contents);

			return rules;
		}

		private static void LogInitialization()
		{
			string applicationDataDirectory = @"DigitalZenWorks\MusicManager";
			string baseDataDirectory = Environment.GetFolderPath(
				Environment.SpecialFolder.ApplicationData,
				Environment.SpecialFolderOption.Create) + @"\" +
				applicationDataDirectory;

			string logFilePath = baseDataDirectory + @"\MusicManager.log";
			const string outputTemplate =
				"[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] " +
				"{Message:lj}{NewLine}{Exception}";

			LoggerConfiguration configuration = new ();
			configuration = configuration.MinimumLevel.Verbose();

			LoggerSinkConfiguration sinkConfiguration = configuration.WriteTo;
			sinkConfiguration.Console(LogEventLevel.Verbose, outputTemplate);
			sinkConfiguration.File(
				logFilePath,
				LogEventLevel.Verbose,
				outputTemplate,
				flushToDiskInterval: TimeSpan.FromSeconds(1));
			Serilog.Log.Logger = configuration.CreateLogger();

			LogManager.Adapter =
				new CommonLogging.Serilog.SerilogFactoryAdapter();
		}
	}
}
