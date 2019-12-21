/////////////////////////////////////////////////////////////////////////////
// <copyright file="UnitTests.cs" company="James John McGuire">
// Copyright © 2006 - 2019 James John McGuire. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using MusicUtility;
using NUnit.Framework;
using System;
using System.IO;
using System.Reflection;

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

			Assert.AreEqual(iTunesDepth, 6);
		}

		[Test]
		public void GetArtistNameFromPath()
		{
			MusicManager musicUtility = new MusicManager();
			string location = musicUtility.ITunesLibraryLocation;

			Assert.IsNotEmpty(location);
		}

		[Test]
		public void RunRuleBasic()
		{
			Rule rule = new Rule();
			rule.Subject = "MusicUtility.Tags.Album";
			rule.Condition = Condition.ContainsRegex;
			rule.Conditional = @"\s*\(Disc.*?\)";
			rule.Operation = Operations.Remove;

			string file = @"C:\Users\JamesMc\Data\External\Entertainment\" +
				@"Music\6ix\What It Is! Funky Soul And Rare Grooves\" +
				"Im Just Like You.mp3";
			Tags tag = new Tags(file);

			Type itemType = tag.GetType();
			PropertyInfo propertyInfo =
				itemType.GetProperty("Album");

			object source = propertyInfo.GetValue(tag, null);

			object result = Rules.RunRule(
				rule, tag, "MusicUtility.Tags.Album", source, null);

			string test = (string)result;

			//Assert.IsNotEmpty(rule);
			Assert.That(test, Is.EqualTo(
				"What It Is! Funky Soul And Rare Grooves"));
		}
	}
}
