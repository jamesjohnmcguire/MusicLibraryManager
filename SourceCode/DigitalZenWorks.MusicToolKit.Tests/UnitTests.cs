﻿/////////////////////////////////////////////////////////////////////////////
// <copyright file="UnitTests.cs" company="Digital Zen Works">
// Copyright © 2019 - 2022 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Common.Logging;
using DigitalZenWorks.Common.Utilities;
using NUnit.Framework;
using NUnit.Framework.Internal;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using System;
using System.Collections.Generic;
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
			bool result = FileUtils.CreateFileFromEmbeddedResource(
				"DigitalZenWorks.MusicToolKit.Tests.sakura.mp4", testFile);

			result = File.Exists(testFile);

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
			using MusicManager musicUtility = new ();
			string location = musicUtility.ITunesLibraryLocation;

			string fileName =
				location + @"Music\10cc\The Very Best Of 10cc\" +
				@"The Things We Do For Love.mp3";

			string album = Paths.GetAlbumFromPath(fileName, location);

			Log.Info("album: " + album);
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

			Log.Info("album: " + album);
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

			Log.Info("album: " + album);
			Assert.IsNotEmpty(album);

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

			Log.Info("album: " + album);
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

			Log.Info("album: " + album);
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

			Log.Info("album: " + album);
			Assert.IsNotEmpty(album);

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
		/// The artist name get from path method test.
		/// </summary>
		[Test]
		public void ArtistNameGetFromPath()
		{
			using MusicManager musicUtility = new ();
			string location = musicUtility.ITunesLibraryLocation;

			Assert.IsNotEmpty(location);

			string fileName =
				location + @"Music\10cc\The Very Best Of 10cc\" +
				@"The Things We Do For Love.mp3";

			string artist = Paths.GetArtistFromPath(fileName, location);

			Log.Info("artist: " + artist);
			Assert.IsNotEmpty(artist);

			string expected = "10cc";
			Assert.That(artist, Is.EqualTo(expected));
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
		/// The run rule disc check method.
		/// </summary>
		[Test]
		public void MediaFileTagsCheck()
		{
			using MusicManager musicUtility = new ();
			string location = musicUtility.ITunesLibraryLocation;

			string fileName =
				location + @"Music\10cc\The Very Best Of 10cc\" +
				@"The Things We Do For Love.mp3";

			Rules rules = GetRules();

			using MediaFileTags tags = new (fileName, location, rules);

			Assert.NotNull(tags);
			Assert.NotNull(tags.TagFile);
			Assert.NotNull(tags.TagSet);
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