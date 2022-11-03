/////////////////////////////////////////////////////////////////////////////
// <copyright file="RulesTests.cs" company="Digital Zen Works">
// Copyright © 2019 - 2022 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using DigitalZenWorks.Common.Utilities;
using NUnit.Framework;
using NUnit.Framework.Internal;
using RulesLibrary.Tests;
using System;
using System.Collections.Generic;
using System.IO;

[assembly: CLSCompliant(false)]

namespace DigitalZenWorks.RulesLibrary.Tests
{
	/// <summary>
	/// Rules tests class.
	/// </summary>
	[TestFixture]
	public class RulesTests
	{
		private string rulesFileContent;
		private PocoItem tags;
		private string temporaryPath;
		private Rules testRules;

		/// <summary>
		/// The one time setup method.
		/// </summary>
		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			temporaryPath = Path.GetTempFileName();
			File.Delete(temporaryPath);
			Directory.CreateDirectory(temporaryPath);

			string rulesFile = temporaryPath + @"\rules.json";
			FileUtils.CreateFileFromEmbeddedResource(
				"RulesLibrary.Tests.TestRules.json",
				rulesFile);

			rulesFileContent = File.ReadAllText(rulesFile);
		}

		/// <summary>
		/// The setup before every test method.
		/// </summary>
		[SetUp]
		public void SetUp()
		{
			testRules = new (rulesFileContent);

			tags = new ();

			string original =
				"What It Is! Funky Soul And Rare Grooves (Disk 2)";
			tags.Property1 = original;

			tags.PropertySet = new string[1];
			tags.PropertySet[0] = "Various Artists";
			tags.PropertySet2 = new string[1];
			tags.PropertySet2[0] = "The Solos";
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
				"RulesLibrary.Tests.PocoItem",
				Condition.Equals,
				@"Album",
				Operation.None);

			string subject = "Album";
			PocoItem tags = new ();
			tags.Property1 = "Album";

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
				"RulesLibrary.Tests.PocoItem",
				Condition.Equals,
				"Something",
				Operation.None);

			string subject = "Something Else";
			PocoItem tags = new ();
			tags.Property1 = subject;

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
				"RulesLibrary.Tests.PocoItem",
				Condition.Equals,
				"Property1",
				Operation.Remove);

			rule.ConditionalType = ConditionalType.Property;
			string condition = rule.GetConditionalValue(tags);

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
			string condition = rule.GetConditionalValue(tags);

			Assert.That(condition, Is.EqualTo(original));
		}

		/// <summary>
		/// The get default rules method test.
		/// </summary>
		[Test]
		public void GetDefaultRules()
		{
			for (int index = 0; index < testRules.RulesList.Count; index++)
			{
				Rule rule = testRules.RulesList[index];

				bool result = rule.Run(tags);

				Assert.True(result);

				if (index == 0)
				{
					string test = tags.Property1;
					Assert.That(test, Is.EqualTo(
						"What It Is! Funky Soul And Rare Grooves"));
				}
				else
				{
					string test = tags.PropertySet[0];

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
			string subject = Rule.GetItemSubject(tags, "Property1");

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
			Assert.NotNull(testRules);

			Rule rule = testRules.GetRuleByName("RemoveDiscFromAlbum");
			Assert.NotNull(rule);
		}

		/// <summary>
		/// The get rules list method test.
		/// </summary>
		[Test]
		public void GetRulesList()
		{
			Assert.NotNull(testRules);

			IList<Rule> rulesList = testRules.RulesList;
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

			bool conditionMet = rule.IsConditionMet(tags, original);

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

			bool conditionMet = rule.IsConditionMet(tags, original);

			Assert.True(conditionMet);
		}

		/// <summary>
		/// The check for rules test.
		/// </summary>
		[Test]
		public void CheckForRules()
		{
			Assert.NotNull(testRules);
		}

		/// <summary>
		/// The rule regex remove test.
		/// </summary>
		[Test]
		public void RuleAction()
		{
			Rule rule = new (
				"RulesLibrary.Tests.PocoItem",
				Condition.ContainsRegex,
				@"\s*\(Dis(c|k).*?\)",
				Operation.Remove);

			string subject = "What It Is! Funky Soul And Rare Grooves";
			PocoItem tags = new ();
			tags.Property1 = subject;

			bool result = rule.Action(
				tags, "Property1", "What It Is! Funky Soul And Rare Grooves");
			Assert.True(result);
		}

		/// <summary>
		/// The rule regex remove change test.
		/// </summary>
		[Test]
		public void RuleActionChange()
		{
			Rule rule = new (
				"RulesLibrary.Tests.PocoItem",
				Condition.ContainsRegex,
				@"\s*\(Dis(c|k).*?\)",
				Operation.Remove);

			tags.Property1 = "What It Is! Funky Soul And Rare Grooves (Disk 2)";

			bool result = rule.Action(
				tags, "Property1", "What It Is! Funky Soul And Rare Grooves");
			Assert.True(result);

			string album = tags.Property1;
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

			bool result = rule.CheckNextRule(tags);
			Assert.False(result);
		}

		/// <summary>
		/// The rule check next rule set test.
		/// </summary>
		[Test]
		public void RuleCheckNextRuleSet()
		{
			string original = "Various Artists";
			string element = "PropertySet";

			// Set up final rule - and if PropertySet not equal PropertySet2,
			// replace PropertySet with PropertySet2
			Rule nextChainRule = new (
				"RulesLibrary.Tests.PocoItem.PropertySet",
				Condition.NotEquals,
				"PropertySet2",
				Operation.Replace,
				"PropertySet2");

			nextChainRule.ConditionalType = ConditionalType.Property;

			// Set up additional rule - and if PropertySet2 tag is not empty,
			Rule chainRule = new (
				"RulesLibrary.Tests.PocoItem.PropertySet2",
				Condition.NotEmpty,
				original,
				Operation.None,
				null,
				Chain.And,
				nextChainRule);

			// Set up initial rule - if PropertySet tag equal 'Various Artists'
			Rule rule = new (
				element,
				Condition.Equals,
				original,
				Operation.None,
				null,
				Chain.And,
				chainRule);

			bool result = rule.CheckNextRule(tags);
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
			string element = "RulesLibrary.Tests.PocoItem.Property1";

			Rule rule = new (
				element,
				Condition.ContainsRegex,
				@"\s*\(Dis(c|k).*?\)",
				Operation.Remove);

			bool result = rule.Run(tags);

			Assert.True(result);

			string test = tags.Property1;
			Assert.That(test, Is.EqualTo(
				"What It Is! Funky Soul And Rare Grooves"));
		}

		/// <summary>
		/// The run rule disc check by name method.
		/// </summary>
		[Test]
		public void RunRuleDiscCheckByName()
		{
			Assert.NotNull(testRules);

			Rule rule = testRules.GetRuleByName("RemoveDiscFromAlbum");
			Assert.NotNull(rule);

			bool result = rule.Run(tags);

			Assert.True(result);

			string test = tags.Property1;
			Assert.That(test, Is.EqualTo(
				"What It Is! Funky Soul And Rare Grooves"));
		}

		/// <summary>
		/// The run rules updated test.
		/// </summary>
		[Test]
		public void RunRulesUpdated()
		{
			Assert.NotNull(testRules);

			bool result = testRules.RunRules(tags);

			Assert.True(result);

			string test = tags.Property1;
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
			string element = "PropertySet";

			// Set up final rule - and if artists not equal performers,
			// replace artists with performers
			Rule nextChainRule = new (
				"PropertySet",
				Condition.NotEquals,
				"PropertySet2",
				Operation.Replace,
				"PropertySet2");

			nextChainRule.ConditionalType = ConditionalType.Property;

			// Set up additional rule - and if performers tag is not empty,
			Rule chainRule = new (
				"RulesLibrary.Tests.PocoItem.PropertySet2",
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

			string test = tags.PropertySet[0];

			Assert.That(test, Is.EqualTo("The Solos"));
		}

		/// <summary>
		/// The run rule various artists check by name method test.
		/// </summary>
		[Test]
		public void RunRuleVariousArtistsCheckByName()
		{
			Assert.NotNull(testRules);

			Rule rule = testRules.GetRuleByName("ReplaceVariousArtists");
			Assert.NotNull(rule);

			bool result = rule.Run(tags);

			Assert.True(result);

			string test = tags.PropertySet[0];

			Assert.That(test, Is.EqualTo("The Solos"));
		}

		/// <summary>
		/// The set item subject test.
		/// </summary>
		[Test]
		public void SetItemSubject()
		{
			string newSubject =
				"What It Is! Funky Soul And Rare Grooves";

			bool result = Rule.SetItemSubject(tags, "PropertySet", newSubject);
			Assert.True(result);

			string subject = Rule.GetItemSubject(tags, "PropertySet");
			Assert.That(subject, Is.EqualTo(newSubject));
		}
	}
}
