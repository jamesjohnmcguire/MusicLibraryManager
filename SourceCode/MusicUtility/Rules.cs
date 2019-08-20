using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace MusicUtility
{
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

		public void RunRules()
		{
			foreach (Rule rule in rules)
			{
				RunRule(rule, null, null);
			}
		}

		public static object RunRule(Rule rule, string subject, object content)
		{
			string ruleSubject = (string)rule.Subject;

			if (ruleSubject.Equals(
				subject, System.StringComparison.InvariantCulture))
			{
				string text = (string)content;

				switch (rule.Condition)
				{
					case Condition.ContainsRegex:
						subject = RegexReplace(content, rule.Conditional);
						break;
					case Condition.Equals:
						subject = RegexReplace(content, rule.Conditional);
						break;
				}
			}

			return subject;
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
