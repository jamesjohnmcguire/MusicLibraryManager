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
