/////////////////////////////////////////////////////////////////////////////
// <copyright file="Rule.cs" company="Digital Zen Works">
// Copyright © 2019 - 2021 Digital Zen Works. All Rights Reserved.
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
	/// <param name="item">The item to be checked.</param>
	/// <param name="itemSubject">The item subject to evaluate.</param>
	/// <param name="conditional">The value to compare with.</param>
	/// <returns>Returns the source file Contents.</returns>
	/////////////////////////////////////////////////////////////////////////
	public delegate bool CheckCondition(
		object item, object itemSubject, object conditional);

	/////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Represents a method that generates source file contents.
	/// </summary>
	/// <param name="ruleSubject">The subject within the rule to evaluate.</param>
	/// <param name="subject">The subject value to compare.</param>
	/// <returns>Returns the source file Contents.</returns>
	/////////////////////////////////////////////////////////////////////////
	public delegate bool CheckSubject(string ruleSubject, string subject);

	public class Rule
	{
		private ConditionalType conditionalType = ConditionalType.Literal;

		public Rule() { }

		public Rule(
			string subject,
			Condition condition,
			string conditional,
			Operation operation,
			object replacement = null)
		{
			Subject = subject;
			Condition = condition;
			Conditional = conditional;
			Operation = operation;
			Replacement = replacement;
		}

		public Rule(
			string subject,
			Condition condition,
			string conditional,
			Operation operation,
			object replacement,
			Chain chain,
			Rule chainRule)
			: this(subject, condition, conditional, operation, replacement)
		{
			Chain = chain;
			ChainRule = chainRule;
		}

		public string Subject { get; set; }

		public Condition Condition { get; set; }

		public string Conditional { get; set; }

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

		public object Run(object item)
		{
			object content = null;

			if (item != null)
			{
				content = GetItemSubject(item, Subject);

				bool conditionMet = IsConditionMet(item, content);

				switch (this.Condition)
				{
					case Condition.ContainsRegex:
						content = RegexReplace(content, this.Conditional);
						break;
					default:
						break;
				}

				if (conditionMet == true)
				{
					content = CheckNextRule(item, content);
				}

				if (this.ChainRule == null)
				{
					content = Action(item, Subject, content);
				}
			}

			return content;
		}

		private static bool ConditionNotEmptyTest(object itemSubject)
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

		private static string GetItemSubject(object item, string subject)
		{
			string itemSubject = null;
			string baseElement = GetObjectBaseElement(subject);

			// Get the property info of the 'subject' from
			// the item being inspected
			Type itemType = item.GetType();
			PropertyInfo propertyInfo =
				itemType.GetProperty(baseElement);

			if (propertyInfo != null)
			{
				object propertyValue = propertyInfo.GetValue(item, null);

				if (propertyValue is string propertyText)
				{
					itemSubject = propertyText;
				}
				else if (propertyValue is string[] propertyArray)
				{
					foreach (string nextSubject in propertyArray)
					{
						if (!string.IsNullOrWhiteSpace(nextSubject))
						{
							itemSubject = nextSubject;
							break;
						}
					}
				}
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
				object propertyValue = propertyInfo.GetValue(item, null);

				if (propertyValue is string)
				{
					propertyInfo.SetValue(item, newValue, null);
					result = true;
				}
				else if (propertyValue is string[])
				{
					string[] newValueArray = new string[1];
					newValueArray[0] = (string)newValue;

					propertyInfo.SetValue(item, newValueArray, null);
					result = true;
				}
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

		private object Action(object item, string subject, object content)
		{
			switch (this.Operation)
			{
				case Operation.Replace:
					if (this.ConditionalType == ConditionalType.Literal)
					{
						this.Replacement = this.Conditional;
					}
					else
					{
						// need to get the value of the property
						this.Replacement =
							GetItemSubject(item, (string)this.Replacement);
					}

					SetItemSubject(item, subject, this.Replacement);
					content = GetItemSubject(item, subject);
					break;
				default:
					break;
			}

			return content;
		}

		private object CheckNextRule(object item, object content)
		{
			if (this.ChainRule != null)
			{
				Rule nextRule = null;

				switch (this.Chain)
				{
					case Chain.And:
						nextRule = this.ChainRule;
						break;
					case Chain.Or:
						nextRule = this.ChainRule;
						break;
				}

				if (nextRule != null)
				{
					content = nextRule.Run(item);
				}
			}

			return content;
		}

		private bool ConditionEqualsTest(object itemSubject)
		{
			bool success = false;
			string testing = (string)Conditional;

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

		private bool ConditionNotEqualsTest(object item, object itemSubject)
		{
			bool success = false;
			Conditional = GetConditionalValue(item);

			if (itemSubject is string subject)
			{
				if (!subject.Equals(Conditional, StringComparison.Ordinal))
				{
					success = true;
				}
			}
			else if (itemSubject is string[] subjectObject)
			{
				foreach (string nextSubject in subjectObject)
				{
					if (!nextSubject.Equals(Conditional, StringComparison.Ordinal))
					{
						success = true;
					}
				}
			}

			return success;
		}

		private bool ConditionRegexMatch(string content)
		{
			bool conditionMet = false;

			if (Regex.IsMatch(content, Conditional, RegexOptions.IgnoreCase))
			{
				conditionMet = true;
			}

			return conditionMet;
		}

		private string GetConditionalValue(object item)
		{
			string objectValue;

			if (this.ConditionalType == ConditionalType.Literal)
			{
				objectValue = Conditional;
			}
			else
			{
				objectValue = GetItemSubject(item, Conditional);
			}

			return objectValue;
		}

		private bool IsConditionMet(object item, object content)
		{
			bool conditionMet = false;

			switch (Condition)
			{
				case Condition.ContainsRegex:
					string contentText = (string)content;
					conditionMet = ConditionRegexMatch(contentText);
					break;
				case Condition.Equals:
					conditionMet = ConditionEqualsTest(content);
					break;
				case Condition.NotEmpty:
					conditionMet = ConditionNotEmptyTest(content);
					break;
				case Condition.NotEquals:
					conditionMet = ConditionNotEqualsTest(item, content);
					break;
			}

			return conditionMet;
		}
	}
}
