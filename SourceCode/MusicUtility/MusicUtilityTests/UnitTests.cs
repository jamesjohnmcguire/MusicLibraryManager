﻿/////////////////////////////////////////////////////////////////////////////
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
			MusicManager musicUtility = new MusicManager();
			string location = musicUtility.ITunesLibraryLocation;

			Assert.IsNotEmpty(location);
		}

		[Test]
		public void GetItunesPathDepth()
		{
			MusicManager musicUtility = new MusicManager();
			string location = musicUtility.ITunesLibraryLocation;
			int iTunesDepth = Paths.GetItunesDirectoryDepth(location);

			Assert.GreaterOrEqual(iTunesDepth, 6);
		}

		[Test]
		public void GetArtistNameFromPath()
		{
			MusicManager musicUtility = new MusicManager();
			string location = musicUtility.ITunesLibraryLocation;

			Assert.IsNotEmpty(location);
		}

		[Test]
		public void RunRuleDiscCheck()
		{
			string original =
				"What It Is! Funky Soul And Rare Grooves (Disk 2)";
			string element = "MusicUtility.Tags.Album";

			Rule rule = new Rule();
			rule.Subject = element;
			rule.Condition = Condition.ContainsRegex;
			rule.Conditional = @"\s*\(Dis[A-Za-z].*?\)";
			rule.Operation = Operation.Remove;

			TagSet tags = new TagSet();
			tags.Album = original;

			object result = rule.Run(tags, null, null);

			string test = (string)result;
			Assert.That(test, Is.EqualTo(
				"What It Is! Funky Soul And Rare Grooves"));
		}

		[Test]
		public void RunRuleVariousArtistsCheck()
		{
			string original = "Various Artists";
			string element = "Artists";

			// Set up initial rule - if artists tag equal 'Various Artists'
			Rule rule = new Rule();
			rule.Subject = element;
			rule.Condition = Condition.Equals;
			rule.Conditional = original;
			rule.Chain = Chain.And;

			// Set up additional rule - and if performers tag is not empty,
			Rule chainRule = new Rule();
			chainRule.Subject = "MusicUtility.Tags.TagFile.Tag.Performers";
			chainRule.Condition = Condition.NotEmpty;
			chainRule.Chain = Chain.And;
			rule.ChainRule = chainRule;

			// Set up final rule - and if artists not equal performers,
			// replace artists with performers
			Rule nextChainRule = new Rule();
			nextChainRule.Subject = "Artists";
			nextChainRule.Condition = Condition.NotEquals;
			nextChainRule.Conditional = "Performers";
			nextChainRule.Replacement = "Performers";
			nextChainRule.ConditionalType = ConditionalType.Property;
			nextChainRule.Operation = Operation.Replace;
			chainRule.ChainRule = nextChainRule;

			TagSet tags = new TagSet();
			tags.Artists = new string[1];
			tags.Performers = new string[1];
			tags.Artists[0] = original;
			tags.Performers[0] = "The Solos";

			object result = rule.Run(tags, null, null);

			string test = null;

			if (result is string[] subjectObject)
			{
				string[] temp = subjectObject;
				test = temp[0];
			}
			else if (result is string ruleObject)
			{
				test = ruleObject;
			}

			Assert.That(test, Is.EqualTo(
				"The Solos"));
		}
	}
}
