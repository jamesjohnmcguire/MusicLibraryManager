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

	/// <summary>
	/// Rules class.
	/// </summary>
	public class Rule
	{
		private ConditionalType conditionalType = ConditionalType.Literal;

		/// <summary>
		/// Initializes a new instance of the <see cref="Rule"/> class.
		/// </summary>
		public Rule() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Rule"/> class.
		/// </summary>
		/// <param name="subject">The rule subject.</param>
		/// <param name="condition">The rule condition.</param>
		/// <param name="conditional">The rule conditional.</param>
		/// <param name="operation">The rule operation.</param>
		/// <param name="replacement">The rule replacement.</param>
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

		/// <summary>
		/// Initializes a new instance of the <see cref="Rule"/> class.
		/// </summary>
		/// <param name="subject">The rule subject.</param>
		/// <param name="condition">The rule condition.</param>
		/// <param name="conditional">The rule conditional.</param>
		/// <param name="operation">The rule operation.</param>
		/// <param name="replacement">The rule replacement.</param>
		/// <param name="chain">The rule chain type.</param>
		/// <param name="chainRule">The rule next chain.</param>
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

		/// <summary>
		/// Gets or sets the subject.
		/// </summary>
		/// <value>The subject.</value>
		public string Subject { get; set; }

		/// <summary>
		/// Gets or sets the condidtion.
		/// </summary>
		/// <value>The condidtion.</value>
		public Condition Condition { get; set; }

		/// <summary>
		/// Gets or sets the condidtional.
		/// </summary>
		/// <value>The condidtional.</value>
		public string Conditional { get; set; }

		/// <summary>
		/// Gets or sets the conditional type.
		/// </summary>
		/// <value>The conditional type.</value>
		public ConditionalType ConditionalType
		{
			get { return conditionalType; }
			set { conditionalType = value; }
		}

		/// <summary>
		/// Gets or sets the operation.
		/// </summary>
		/// <value>The operation.</value>
		public Operation Operation { get; set; }

		/// <summary>
		/// Gets or sets the replacement.
		/// </summary>
		/// <value>The replacement.</value>
		public object Replacement { get; set; }

		/// <summary>
		/// Gets or sets the chain.
		/// </summary>
		/// <value>The chain.</value>
		public Chain Chain { get; set; }

		/// <summary>
		/// Gets or sets the chain rule.
		/// </summary>
		/// <value>The chain rule.</value>
		public Rule ChainRule { get; set; }

		/// <summary>
		/// Get object base element method.
		/// </summary>
		/// <param name="element">The element to check.</param>
		/// <returns>The base element.</returns>
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

		/// <summary>
		/// Run method.
		/// </summary>
		/// <param name="item">The object to process.</param>
		/// <returns>A value indicating success or not.</returns>
		public bool Run(object item)
		{
			bool changed = false;

			if (item != null)
			{
				object content = GetItemSubject(item, Subject);

				bool conditionMet = IsConditionMet(item, content);

				if (conditionMet == true)
				{
					changed = CheckNextRule(item);
				}

				if (this.ChainRule == null)
				{
					changed = Action(item, Subject, content);
				}
			}

			return changed;
		}

		private static bool ConditionNotEmptyTest(object itemSubject)
		{
			bool success = false;

			if (itemSubject is string)
			{
				success = true;
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

		private bool Action(object item, string subject, object content)
		{
			bool result = false;

			switch (this.Condition)
			{
				case Condition.ContainsRegex:
					content = RegexReplace(content, this.Conditional);
					result = SetItemSubject(item, subject, content);
					break;
				default:
					break;
			}

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

					result = SetItemSubject(item, subject, this.Replacement);
					break;
				default:
					break;
			}

			return result;
		}

		private bool CheckNextRule(object item)
		{
			bool changed = false;

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
					changed = nextRule.Run(item);
				}
			}

			return changed;
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
