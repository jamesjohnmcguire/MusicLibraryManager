using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MusicUtility
{
	/////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Represents a method that generates source file contents.
	/// </summary>
	/// <param name="template">The template file for this genterator.</param>
	/// <returns>Returns the source file Contents.</returns>
	/////////////////////////////////////////////////////////////////////////
	public delegate string ContentsGenerator(string template);

	public class Rules
	{
		private readonly IList<Rule> rules;

		public Rules(string data)
		{
			if (!string.IsNullOrEmpty(data))
			{
				rules = JsonConvert.DeserializeObject<IList<Rule>>(data);
			}
		}

		public IList<Rule> RulesList
		{
			get
			{
				return rules;
			}
		}

		public static object RunRule(
			Rule rule,
			string subject,
			object content,
			object replacement,
			IDictionary<string, string> additionals = null)
		{
			string ruleSubject = (string)rule.Subject;

			string text;

			switch (rule.Condition)
			{
				case Condition.ContainsRegex:
					if (ruleSubject.Equals(
						subject, System.StringComparison.InvariantCulture))
					{
						content = RegexReplace(content, rule.Conditional);
					}

					break;

				case Condition.Equals:
					content = ConditionEquals(
						rule, subject, content, replacement, additionals);
					break;

				case Condition.NotEmpty:
					if (content is string[] item)
					{
						if (item.Length > 0)
						{
							text = item[0];
						}
					}

					break;
			}

			return content;
		}

		public void RunRules()
		{
			foreach (Rule rule in rules)
			{
				RunRule(rule, null, null, null);
			}
		}

		public void RunRules(object item)
		{
			Type classType = item.GetType();
			string className = classType.FullName;

			PropertyInfo[] properties = classType.GetProperties();

			foreach (PropertyInfo property in properties)
			{
				string name = property.Name;
				string fullName = string.Format(
					CultureInfo.InvariantCulture, "{0}.{1}", className, name);

				object source = property.GetValue(item, null);

				foreach (Rule rule in rules)
				{
					object newValue = RunRule(rule, fullName, source, null);

					if (!source.Equals(newValue))
					{
						classType.GetProperty(name).SetValue(
							item, newValue, null);
					}
				}
			}
		}

		private static object ConditionEquals(
			Rule rule,
			string subject,
			object content,
			object replacement,
			IDictionary<string, string> additionals = null)
		{
			string ruleSubject = (string)rule.Subject;

			if (ruleSubject.Equals(
				subject, System.StringComparison.InvariantCulture))
			{
				string text = (string)content;
				if (text.Equals(rule.Conditional))
				{
					if (rule.Chain == Chain.And)
					{
						Rule andRule = rule.ChainRule;

						if (additionals != null)
						{
							string andSubject;
							if (additionals.TryGetValue(
								"subject", out andSubject))
							{
								andSubject = additionals["subject"];
							}
						}

						RunRule(
							andRule,
							subject,
							content,
							replacement,
							additionals);
					}
				}

				content = RegexReplace(content, rule.Conditional);
			}

			return content;
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
	}
}
