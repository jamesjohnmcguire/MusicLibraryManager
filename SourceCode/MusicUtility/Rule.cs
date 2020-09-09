/////////////////////////////////////////////////////////////////////////////
// <copyright file="Rule.cs" company="Digital Zen Works">
// Copyright © 2019 - 2020 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MusicUtility
{
	/////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Represents a method that generates source file contents.
	/// </summary>
	/// <param name="condition">The template file for this genterator.</param>
	/// <returns>Returns the source file Contents.</returns>
	/////////////////////////////////////////////////////////////////////////
	public delegate bool CheckCondition(object item, object itemSubject, object conditional);

	/////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Represents a method that generates source file contents.
	/// </summary>
	/// <param name="condition">The template file for this genterator.</param>
	/// <returns>Returns the source file Contents.</returns>
	/////////////////////////////////////////////////////////////////////////
	public delegate bool CheckSubject(string ruleSubject, string subject);

	public class Rule
	{
		private ConditionalType conditionalType = ConditionalType.Literal;

		public object Subject { get; set; }

		public Condition Condition { get; set; }

		public object Conditional { get; set; }

		public ConditionalType ConditionalType
		{
			get { return conditionalType; }
			set { conditionalType = value; }
		}

		public Operation Operation { get; set; }

		public object Replacement { get; set; }

		public Chain Chain { get; set; }

		public Rule ChainRule { get; set; }

		public static string GetObjectBaseElement(string element)
		{
			string baseElement = null;

			if (!string.IsNullOrEmpty(element))
			{
				string[] parts = element.Split('.');

				baseElement = parts[parts.Length - 1];
			}

			return baseElement;
		}

		public object Run(
			object item,
			string subject,
			object replacement,
			IDictionary<string, string> additionals = null)
		{
			object content = null;

			if (item != null)
			{
				bool matching = false;
				CheckCondition test;
				content = GetItemSubject(item, subject);

				string ruleSubject = GetStringFromStringOrArray(this.Subject);

				switch (this.Condition)
				{
					case Condition.ContainsRegex:
						if (ruleSubject.Equals(
							subject, System.StringComparison.InvariantCulture))
						{
							content = RegexReplace(content, this.Conditional);
						}

						break;
					case Condition.Equals:
						test = new CheckCondition(ConditionEqualsTest);

						matching = test(item, content, this.Conditional);
						break;
					case Condition.NotEmpty:
						// object tester = GetFullPathObject(item, ruleSubject);
						// content = GetStringFromStringOrArray(tester);
						test = new CheckCondition(ConditionNotEmptyTest);

						matching = test(item, content, this.Conditional);
						break;
					case Condition.NotEquals:
						test = new CheckCondition(ConditionNotEqualsTest);

						matching = test(item, content, this.Conditional);
						break;
				}

				if (matching == true)
				{
					content = CheckNextRule(item, subject, content, replacement, additionals);
				}

				if (this.ChainRule == null)
				{
					Action(item, subject);
				}
			}

			return content;
		}

		private static object GetFullPathObject(object item, string subject)
		{
			object currentItem = item;

			string[] parts = subject.Split('.');

			string path = string.Empty;

			for (int index = 0; index < parts.Length; index++)
			{
				if (index == 0)
				{
					// namespace
					path = parts[0];
				}
				else
				{
					path += '.' + parts[index];

					Type itemType = currentItem.GetType();
					if (path.Equals(
						itemType.FullName, StringComparison.Ordinal))
					{
						continue;
					}
					else
					{
						PropertyInfo propertyInfo =
							itemType.GetProperty(parts[index]);

						if (propertyInfo != null)
						{
							object nextItem =
								propertyInfo.GetValue(currentItem, null);
							currentItem = nextItem;
						}
					}
				}
			}

			return currentItem;
		}

		private static object GetItemSubject(object item, string subject)
		{
			object itemSubject = null;
			string baseElement = GetObjectBaseElement(subject);

			// Get the property info of the 'subject' from
			// the item being inspected
			Type itemType = item.GetType();
			PropertyInfo propertyInfo =
				itemType.GetProperty(baseElement);

			if (propertyInfo != null)
			{
				itemSubject = propertyInfo.GetValue(item, null);
			}

			return itemSubject;
		}

		private static bool SetItemSubject(object item, string subject, object newValue)
		{
			bool result = false;

			string baseElement = GetObjectBaseElement(subject);

			// Get the property info of the 'subject' from
			// the item being inspected
			Type itemType = item.GetType();
			PropertyInfo propertyInfo =
				itemType.GetProperty(baseElement);

			if (propertyInfo != null)
			{
				propertyInfo.SetValue(item, newValue, null);
				result = true;
			}

			return result;
		}

		private static string GetStringFromStringOrArray(object subject)
		{
			string result = null;

			if (subject is string[] subjectObject)
			{
				if (subjectObject.Length > 0)
				{
					result = subjectObject[0];
				}
			}
			else if (subject is string ruleObject)
			{
				result = ruleObject;
			}

			return result;
		}

		private static string RegexReplace(object content, object conditional)
		{
			string subject = null;

			if (content is string @string)
			{
				subject = @string;

				string find = (string)conditional;

				if (Regex.IsMatch(subject, find, RegexOptions.IgnoreCase))
				{
					subject = Regex.Replace(
						subject,
						find,
						string.Empty,
						RegexOptions.IgnoreCase);
				}
			}

			return subject;
		}

		private object Action(object item, string subject)
		{
			object content = GetItemSubject(item, subject);

			switch (this.Operation)
			{
				case Operation.Remove:
					break;
				case Operation.Replace:
					SetItemSubject(item, subject, this.Replacement);
					break;
			}

			return null;
		}

		private object CheckNextRule(
			object item,
			string subject,
			object content,
			object replacement,
			IDictionary<string, string> additionals = null)
		{
			if (this.ChainRule != null)
			{
				Rule nextRule = null;

				switch (this.Chain)
				{
					case Chain.And:
						if (additionals != null)
						{
							if (additionals.TryGetValue(
								"subject", out string andSubject))
							{
								andSubject = additionals["subject"];
							}
						}

						nextRule = this.ChainRule;
						break;
					case Chain.Or:
						nextRule = this.ChainRule;
						break;
				}

				if (nextRule != null)
				{
					content = nextRule.Run(
						item, subject, replacement, additionals);
				}
			}

			return content;
		}

		private bool ConditionEqualsTest(object item, object itemSubject, object conditional)
		{
			bool success = false;
			string testing = (string)conditional;

			if (itemSubject is string subject)
			{
				if (subject.Equals(testing, StringComparison.Ordinal))
				{
					success = true;
				}
			}
			else if (itemSubject is string[] subjectObject)
			{
				foreach (string nextSubject in subjectObject)
				{
					if (nextSubject.Equals(testing, StringComparison.Ordinal))
					{
						success = true;
						break;
					}
				}
			}

			return success;
		}

		private bool ConditionNotEmptyTest(object item, object itemSubject, object conditional)
		{
			bool success = false;

			if (itemSubject is string subject)
			{
				if (!string.IsNullOrWhiteSpace(subject))
				{
					success = true;
				}
			}
			else if (itemSubject is string[] subjectObject)
			{
				foreach (string nextSubject in subjectObject)
				{
					if (!string.IsNullOrWhiteSpace(nextSubject))
					{
						success = true;
						break;
					}
				}
			}

			return success;
		}

		private bool ConditionNotEqualsTest(object item, object itemSubject, object conditional)
		{
			bool success = false;
			conditional = GetConditionalValue(item, conditional);

			if (itemSubject is string subject)
			{
				if (conditional is string conditionalTest)
				{
					if (!subject.Equals(conditionalTest, StringComparison.Ordinal))
					{
						success = true;
					}
				}
				else if (conditional is string[] conditionalObject)
				{
					foreach (string nextConditional in conditionalObject)
					{
						if (!subject.Equals(nextConditional, StringComparison.Ordinal))
						{
							success = true;
							break;
						}
					}
				}
			}
			else if (itemSubject is string[] subjectObject)
			{
				foreach (string nextSubject in subjectObject)
				{
					if (conditional is string conditionalTest)
					{
						if (!nextSubject.Equals(conditionalTest, StringComparison.Ordinal))
						{
							success = true;
						}
					}
					else if (conditional is string[] conditionalObject)
					{
						foreach (string nextConditional in conditionalObject)
						{
							if (!nextSubject.Equals(nextConditional, StringComparison.Ordinal))
							{
								success = true;
								break;
							}
						}
					}
				}
			}

			return success;
		}

		private object GetConditionalValue(object item, object conditional)
		{
			object objectValue = null;

			if (this.ConditionalType == ConditionalType.Literal)
			{
				objectValue = conditional;
			}
			else
			{
				objectValue = GetItemSubject(item, (string)conditional);
			}

			return objectValue;
		}
	}
}
