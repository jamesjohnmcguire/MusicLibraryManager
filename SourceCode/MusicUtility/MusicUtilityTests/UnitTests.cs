/////////////////////////////////////////////////////////////////////////////
// <copyright file="UnitTests.cs" company="Digital Zen Works">
// Copyright © 2019 - 2021 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Common.Logging;
using NUnit.Framework;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using System;
using System.IO;
using System.Reflection;

namespace MusicUtility.Tests
{
	[TestFixture]
	public class UnitTests
	{
		private static readonly ILog Log = LogManager.GetLogger(
			System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		private TagSet tags;

		public UnitTests()
		{
		}

		[SetUp]
		public void OneTimeSetUp()
		{
			LogInitialization();

			tags = new ();

			string original =
				"What It Is! Funky Soul And Rare Grooves (Disk 2)";
			tags.Album = original;

			tags.Artists = new string[1];
			tags.Performers = new string[1];
			tags.Artists[0] = "Various Artists";
			tags.Performers[0] = "The Solos";
		}

		[Test]
		public void ITunesPathLocation()
		{
			MusicManager musicUtility = new ();
			string location = musicUtility.ITunesLibraryLocation;

			Log.Info("ITunesPathLocation: " + location);
			Assert.IsNotEmpty(location);
		}

		[Test]
		public void GetItunesPathDepth()
		{
			MusicManager musicUtility = new ();
			string location = musicUtility.ITunesLibraryLocation;
			int iTunesDepth = Paths.GetItunesDirectoryDepth(location);

			Assert.GreaterOrEqual(iTunesDepth, 6);
		}

		[Test]
		public void GetArtistNameFromPath()
		{
			MusicManager musicUtility = new ();
			string location = musicUtility.ITunesLibraryLocation;

			Assert.IsNotEmpty(location);
		}

		[Test]
		public void GetDefaultRules()
		{
			string resourceName = "MusicUtility.DefaultRules.json";
			Assembly assembly = typeof(MusicManager).Assembly;

			using Stream templateObjectStream =
				assembly.GetManifestResourceStream(resourceName);

			Assert.NotNull(templateObjectStream);

			using StreamReader reader = new (templateObjectStream);
			string contents = reader.ReadToEnd();

			Rules rules = new (contents);

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

		[Test]
		public void RunRuleDiscCheck()
		{
			string element = "MusicUtility.Tags.Album";

			Rule rule = new (
				element,
				Condition.ContainsRegex,
				@"\s*\(Dis[A-Za-z].*?\)",
				Operation.Remove);

			bool result = rule.Run(tags);

			Assert.True(result);

			string test = tags.Album;
			Assert.That(test, Is.EqualTo(
				"What It Is! Funky Soul And Rare Grooves"));
		}

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
				"MusicUtility.Tags.TagFile.Tag.Performers",
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
				new Common.Logging.Serilog.SerilogFactoryAdapter();
		}
	}
}
