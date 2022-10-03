/////////////////////////////////////////////////////////////////////////////
// <copyright file="RulesTests.cs" company="Digital Zen Works">
// Copyright © 2019 - 2022 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DigitalZenWorks.MusicToolKit.Tests
{
	/// <summary>
	/// Rules tests class.
	/// </summary>
	[TestFixture]
	public class RulesTests : TestsBase
	{
		/// <summary>
		/// The condition equals test fail test.
		/// </summary>
		[Test]
		public void ConditionEqualsTestFail()
		{
			Rule rule = new (
				"DigitalZenWorks.MusicToolKit.Tags.Album",
				Condition.Equals,
				@"Album",
				Operation.None);

			string subject = "Something else";

			bool result = rule.ConditionEqualsTest(subject);
			Assert.False(result);
		}

		/// <summary>
		/// The condition equals test success test.
		/// </summary>
		[Test]
		public void ConditionEqualsTestSuccess()
		{
			Rule rule = new (
				"DigitalZenWorks.MusicToolKit.Tags.Album",
				Condition.Equals,
				@"Album",
				Operation.None);

			string subject = "Album";

			bool result = rule.ConditionEqualsTest(subject);
			Assert.True(result);
		}

		/// <summary>
		/// The condition not equals test fail test.
		/// </summary>
		[Test]
		public void ConditionNotEqualsTestFail()
		{
			Rule rule = new (
				"DigitalZenWorks.MusicToolKit.Tags.Album",
				Condition.Equals,
				@"Album",
				Operation.None);

			string subject = "Album";
			using MediaFileTags tags = new (TestFile);
			tags.Album = "Album";

			bool result = rule.ConditionNotEqualsTest(tags, subject);
			Assert.False(result);
		}

		/// <summary>
		/// The condition not equals test success test.
		/// </summary>
		[Test]
		public void ConditionNotEqualsTestSuccess()
		{
			Rule rule = new (
				"DigitalZenWorks.MusicToolKit.Tags.Album",
				Condition.Equals,
				@"Something Else",
				Operation.None);

			string subject = "Album";
			using MediaFileTags tags = new (TestFile);
			tags.Album = "Something Else";

			bool result = rule.ConditionNotEqualsTest(tags, subject);
			Assert.True(result);
		}

		/// <summary>
		/// The condition not empty test fail test.
		/// </summary>
		[Test]
		public void ConditionNotEmptyTestFail()
		{
			object subject = DateTime.Now;

			bool result = Rule.ConditionNotEmptyTest(subject);
			Assert.False(result);
		}

		/// <summary>
		/// The condition not empty test success test.
		/// </summary>
		[Test]
		public void ConditionNotEmptyTestSuccess()
		{
			string album = "Album";
			object subject = album;

			bool result = Rule.ConditionNotEmptyTest(subject);
			Assert.True(result);

			string[] subjects = new string[] { "Album" };
			subject = subjects;

			result = Rule.ConditionNotEmptyTest(subject);
			Assert.True(result);
		}

		/// <summary>
		/// The condition regex match fail test.
		/// </summary>
		[Test]
		public void ConditionRegexMatchFail()
		{
			string subject =
				"What It Is! Funky Soul And Rare Grooves";
			string pattern = @"\s*\(Dis(c|k).*?\)";

			Rule rule = new (
				"DigitalZenWorks.MusicToolKit.Tags.Album",
				Condition.ContainsRegex,
				pattern,
				Operation.Remove);

			bool result = rule.ConditionRegexMatch(subject);
			Assert.False(result);
		}

		/// <summary>
		/// The condition regex match success test.
		/// </summary>
		[Test]
		public void ConditionRegexMatchSuccess()
		{
			string subject =
				"What It Is! Funky Soul And Rare Grooves (Disk 2)";
			string pattern = @"\s*\(Dis(c|k).*?\)";

			Rule rule = new (
				"DigitalZenWorks.MusicToolKit.Tags.Album",
				Condition.ContainsRegex,
				pattern,
				Operation.Remove);

			bool result = rule.ConditionRegexMatch(subject);
			Assert.True(result);
		}

		/// <summary>
		/// The get conditional value test.
		/// </summary>
		[Test]
		public void GetConditionalValue()
		{
			string original =
				"What It Is! Funky Soul And Rare Grooves (Disk 2)";

			Rule rule = new (
				"DigitalZenWorks.MusicToolKit.Tags.Album",
				Condition.ContainsRegex,
				"DigitalZenWorks.MusicToolKit.Tags.Album",
				Operation.Remove);

			rule.ConditionalType = ConditionalType.Property;
			string condition = rule.GetConditionalValue(Tags);

			Assert.That(condition, Is.EqualTo(original));
		}

		/// <summary>
		/// The get conditional value literal test.
		/// </summary>
		[Test]
		public void GetConditionalValueLiternal()
		{
			string original = "Something";

			Rule rule = new (
				"DigitalZenWorks.MusicToolKit.Tags.Album",
				Condition.ContainsRegex,
				original,
				Operation.Remove);

			rule.ConditionalType = ConditionalType.Literal;
			string condition = rule.GetConditionalValue(Tags);

			Assert.That(condition, Is.EqualTo(original));
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

				bool result = rule.Run(Tags);

				Assert.True(result);

				if (index == 0)
				{
					string test = Tags.Album;
					Assert.That(test, Is.EqualTo(
						"What It Is! Funky Soul And Rare Grooves"));
				}
				else
				{
					string test = Tags.Artists[0];

					Assert.That(test, Is.EqualTo("The Solos"));
				}
			}
		}

		/// <summary>
		/// The get item subject test.
		/// </summary>
		[Test]
		public void GetItemSubject()
		{
			string subject = Rule.GetItemSubject(Tags, "Album");

			string original =
				"What It Is! Funky Soul And Rare Grooves (Disk 2)";

			Assert.That(subject, Is.EqualTo(original));
		}

		/// <summary>
		/// The get object base item test.
		/// </summary>
		[Test]
		public void GetObjectBaseItem()
		{
			string fullType = "DigitalZenWorks.MusicToolKit.Tags.Album";
			string baseItem = Rule.GetObjectBaseElement(fullType);

			string expected = "Album";
			Assert.That(baseItem, Is.EqualTo(expected));
		}

		/// <summary>
		/// The get object base item test.
		/// </summary>
		[Test]
		public void GetObjectBaseItemNone()
		{
			string baseItem = Rule.GetObjectBaseElement(null);

			Assert.Null(baseItem);

			baseItem = Rule.GetObjectBaseElement(string.Empty);

			Assert.Null(baseItem);
		}

		/// <summary>
		/// The get object base item test.
		/// </summary>
		[Test]
		public void GetObjectBaseItemSingle()
		{
			string fullType = "Album";
			string baseItem = Rule.GetObjectBaseElement(fullType);

			string expected = "Album";
			Assert.That(baseItem, Is.EqualTo(expected));
		}

		/// <summary>
		/// The get rule by name test.
		/// </summary>
		[Test]
		public void GetRuleByName()
		{
			using MusicManager musicUtility = new ();

			Rules rules = musicUtility.Rules;
			Assert.NotNull(rules);

			Rule rule = rules.GetRuleByName("RemoveDiscFromAlbum");
			Assert.NotNull(rule);
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
		/// The is condition met fail test.
		/// </summary>
		[Test]
		public void IsConditionMetFail()
		{
			string original =
				"What It Is! Funky Soul And Rare Grooves";
			string pattern = @"\s*\(Dis(c|k).*?\)";

			Rule rule = new (
				"DigitalZenWorks.MusicToolKit.Tags.Album",
				Condition.ContainsRegex,
				pattern,
				Operation.Remove);

			bool conditionMet = rule.IsConditionMet(Tags, original);

			Assert.False(conditionMet);
		}

		/// <summary>
		/// The is condition met success test.
		/// </summary>
		[Test]
		public void IsConditionMetSuccess()
		{
			string original =
				"What It Is! Funky Soul And Rare Grooves (Disk 2)";
			string pattern = @"\s*\(Dis(c|k).*?\)";

			Rule rule = new (
				"DigitalZenWorks.MusicToolKit.Tags.Album",
				Condition.ContainsRegex,
				pattern,
				Operation.Remove);

			bool conditionMet = rule.IsConditionMet(Tags, original);

			Assert.True(conditionMet);
		}

		/// <summary>
		/// The run rule disc check method.
		/// </summary>
		[Test]
		public void MediaFileTagsCheck()
		{
			Rules rules = GetRules();

			using MediaFileTags tags = new (TestFile, rules);

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
			string pattern = @"\s*\(Dis(c|k).*?\)";

			Rule rule = new (
				"DigitalZenWorks.MusicToolKit.Tags.Album",
				Condition.ContainsRegex,
				pattern,
				Operation.Remove);

			rules.RulesList.Add(rule);

			using MusicManager musicManager = new (rules);

			Rules rules2 = musicManager.Rules;

			int count1 = rules.RulesList.Count;
			int count2 = rules2.RulesList.Count;
			Assert.AreEqual(count1, count2);
		}

		/// <summary>
		/// The rule regex remove test.
		/// </summary>
		[Test]
		public void RuleAction()
		{
			string element = "DigitalZenWorks.MusicToolKit.Tags.Album";

			Rule rule = new (
				element,
				Condition.ContainsRegex,
				@"\s*\(Dis(c|k).*?\)",
				Operation.Remove);

			bool result = rule.Action(
				Tags, "Album", "What It Is! Funky Soul And Rare Grooves");
			Assert.True(result);
		}

		/// <summary>
		/// The rule regex remove change test.
		/// </summary>
		[Test]
		public void RuleActionChange()
		{
			string element = "DigitalZenWorks.MusicToolKit.Tags.Album";

			Rule rule = new (
				element,
				Condition.ContainsRegex,
				@"\s*\(Dis(c|k).*?\)",
				Operation.Remove);

			Tags.Album = "What It Is! Funky Soul And Rare Grooves (Disk 2)";

			bool result = rule.Action(
				Tags, "Album", "What It Is! Funky Soul And Rare Grooves");
			Assert.True(result);

			string album = Tags.Album;
			string expected = "What It Is! Funky Soul And Rare Grooves";
			Assert.That(album, Is.EqualTo(expected));
		}

		/// <summary>
		/// The rule check next rule none test.
		/// </summary>
		[Test]
		public void RuleCheckNextRuleNone()
		{
			string element = "DigitalZenWorks.MusicToolKit.Tags.Album";

			Rule rule = new (
				element,
				Condition.ContainsRegex,
				@"\s*\(Dis(c|k).*?\)",
				Operation.Remove);

			bool result = rule.CheckNextRule(Tags);
			Assert.False(result);
		}

		/// <summary>
		/// The rule check next rule set test.
		/// </summary>
		[Test]
		public void RuleCheckNextRuleSet()
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

			bool result = rule.CheckNextRule(Tags);
			Assert.True(result);
		}

		/// <summary>
		/// The rule regex remove test.
		/// </summary>
		[Test]
		public void RuleRegexRemove()
		{
			string original = "What It Is! Funky Soul And Rare Grooves cd 1";
			string pattern = @" cd.*?\d";

			string result = Rule.RegexReplace(original, pattern);

			string expected = "What It Is! Funky Soul And Rare Grooves";
			Assert.That(result, Is.EqualTo(expected));
		}

		/// <summary>
		/// The rule regex remove test.
		/// </summary>
		[Test]
		public void RuleRegexRemoveSame()
		{
			string original = "What It Is! Funky Soul And Rare Grooves";
			string pattern = @" cd.*?\d";

			string result = Rule.RegexReplace(original, pattern);

			Assert.That(result, Is.EqualTo(original));
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

			bool result = rule.Run(Tags);

			Assert.True(result);

			string test = Tags.Album;
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

			bool result = rule.Run(Tags);

			Assert.True(result);

			string test = Tags.Album;
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

			bool result = rule.Run(Tags);

			Assert.True(result);

			string test = Tags.Artists[0];

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

			bool result = rule.Run(Tags);

			Assert.True(result);

			string test = Tags.Artists[0];

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
		/// The set item subject test.
		/// </summary>
		[Test]
		public void SetItemSubject()
		{
			string newSubject =
				"What It Is! Funky Soul And Rare Grooves";

			bool result = Rule.SetItemSubject(Tags, "Album", newSubject);
			Assert.True(result);

			string subject = Rule.GetItemSubject(Tags, "Album");
			Assert.That(subject, Is.EqualTo(newSubject));
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
	}
}
