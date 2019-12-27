﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MusicUtility
{
	public class Rule
	{
		public object Subject { get; set; }

		public Condition Condition { get; set; }

		public object Conditional { get; set; }

		public Operations Operation { get; set; }

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
				string ruleSubject = (string)this.Subject;
				string baseElement = GetObjectBaseElement(subject);

				Type itemType = item.GetType();
				PropertyInfo propertyInfo =
					itemType.GetProperty(baseElement);
				content = propertyInfo.GetValue(item, null);

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
						content = ConditionEquals(
							item, subject, content, replacement, additionals);
						break;

					case Condition.NotEmpty:
						object tester = GetFullPathObject(item, ruleSubject);

						if (tester is string[] conditional)
						{
							if (conditional.Length > 0)
							{
								content = conditional[0];
							}
						}
						// else test other types

						break;
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

						object nextItem =
							propertyInfo.GetValue(currentItem, null);
						currentItem = nextItem;
					}
				}
			}

			return currentItem;
		}

		private static string RegexReplace(object content, object conditional)
		{
			string subject = null;

			if (content is string)
			{
				subject = (string)content;

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

		private object ConditionEquals(
			object item,
			string subject,
			object content,
			object replacement,
			IDictionary<string, string> additionals = null)
		{
			string ruleSubject = (string)this.Subject;

			if (ruleSubject.Equals(
				subject, System.StringComparison.InvariantCulture))
			{
				string text = (string)content;
				if (text.Equals(this.Conditional))
				{
					if (this.Chain == Chain.And)
					{
						if (additionals != null)
						{
							if (additionals.TryGetValue(
								"subject", out string andSubject))
							{
								andSubject = additionals["subject"];
							}
						}

						if (this.ChainRule != null)
						{
							Rule andRule = this.ChainRule;

							content = andRule.Run(
								item,
								subject,
								replacement,
								additionals);
						}
					}
				}

				content = RegexReplace(content, this.Conditional);
			}

			return content;
		}
	}
}
