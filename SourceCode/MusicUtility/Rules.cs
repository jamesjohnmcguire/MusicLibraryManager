/////////////////////////////////////////////////////////////////////////////
// <copyright file="Rules.cs" company="Digital Zen Works">
// Copyright © 2019 - 2021 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace MusicUtility
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
		/// Run rules method.
		/// </summary>
		/// <param name="item">The object to process.</param>
		public void RunRules(object item)
		{
			if (item != null)
			{
				foreach (Rule rule in rules)
				{
					rule.Run(item);
				}
			}
		}
	}
}
