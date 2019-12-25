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
			string original =
				"What It Is! Funky Soul And Rare Grooves (Disk 2)";
			string element = "MusicUtility.Tags.Album";

			Rule rule = new Rule();
			rule.Subject = element;
			rule.Condition = Condition.ContainsRegex;
			rule.Conditional = @"\s*\(Dis[A-Za-z].*?\)";
			rule.Operation = Operations.Remove;

			TagSet tags = new TagSet();
			tags.Album = original;

			string baseElement = Rule.GetObjectBaseElement(element);
			Type itemType = tags.GetType();
			PropertyInfo propertyInfo =
				itemType.GetProperty(baseElement);
			object source = propertyInfo.GetValue(tags, null);

			object result = rule.Run(
				tags, element, source, null);

			string test = (string)result;
			Assert.That(test, Is.EqualTo(
				"What It Is! Funky Soul And Rare Grooves"));
		}
	}
}
