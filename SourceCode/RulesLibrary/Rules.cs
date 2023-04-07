/////////////////////////////////////////////////////////////////////////////
// <copyright file="Rules.cs" company="Digital Zen Works">
// Copyright © 2019 - 2023 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Newtonsoft.Json;

[assembly: CLSCompliant(true)]

namespace DigitalZenWorks.RulesLibrary
{
	/// <summary>
	/// Rules class.
	/// </summary>
	public class Rules
	{
		private readonly IList<Rule> rules;

		/// <summary>
		/// Initializes a new instance of the <see cref="Rules"/> class.
		/// </summary>
		/// <param name="data">The serialized rules data.</param>
		public Rules(string data)
		{
			if (!string.IsNullOrEmpty(data))
			{
				rules = JsonConvert.DeserializeObject<IList<Rule>>(data);
			}
		}

		/// <summary>
		/// Gets the rule list.
		/// </summary>
		/// <value>The rule list.</value>
		public IList<Rule> RulesList
		{
			get
			{
				return rules;
			}
		}

		/// <summary>
		/// Get rule by name method.
		/// </summary>
		/// <param name="name">The name of the rule to get.</param>
		/// <returns>The rule with matching name.</returns>
		public Rule GetRuleByName(string name)
		{
			Rule rule = null;

			foreach (Rule checkRule in rules)
			{
				if (checkRule.Name.Equals(
					name, StringComparison.OrdinalIgnoreCase))
				{
					rule = checkRule;
					break;
				}
			}

			return rule;
		}

		/// <summary>
		/// Run rules method.
		/// </summary>
		/// <param name="item">The object to process.</param>
		/// <returns>A value indicating whether the the item was updated
		/// or not.</returns>
		public bool RunRules(object item)
		{
			bool updated = false;

			if (rules != null && item != null)
			{
				foreach (Rule rule in rules)
				{
					bool ruleUpdated = rule.Run(item);

					if (ruleUpdated == true)
					{
						updated = true;
					}
				}
			}

			return updated;
		}
	}
}
