/////////////////////////////////////////////////////////////////////////////
// <copyright file="UnitTests.cs" company="James John McGuire">
// Copyright © 2019 - 2021 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using MusicUtility;
using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;
using TagLib;

namespace MusicUtility.Tests
{
	[TestFixture]
	public class UnitTests
	{
		[Test]
		public void ITunesPathLocation()
		{
			MusicManager musicUtility = new ();
			string location = musicUtility.ITunesLibraryLocation;

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
		public void RunRuleDiscCheck()
		{
			string original =
				"What It Is! Funky Soul And Rare Grooves (Disk 2)";
			string element = "MusicUtility.Tags.Album";

			Rule rule = new (
				element,
				Condition.ContainsRegex,
				@"\s*\(Dis[A-Za-z].*?\)",
				Operation.Remove);

			TagSet tags = new ();
			tags.Album = original;

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

			TagSet tags = new ();
			tags.Artists = new string[1];
			tags.Performers = new string[1];
			tags.Artists[0] = original;

			tags.Performers[0] = "The Solos";

			bool result = rule.Run(tags);

			Assert.True(result);

			string test = tags.Artists[0];

			Assert.That(test, Is.EqualTo(
				"The Solos"));
		}
	}
}
