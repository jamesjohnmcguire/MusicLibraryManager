using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
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
				rule.Run(null, null, null, null);
			}
		}

		public bool RunRules(object item)
		{
			bool changed = false;

			if (item != null)
			{
				Type classType = item.GetType();
				string className = classType.FullName;

				PropertyInfo[] properties = classType.GetProperties();

				foreach (PropertyInfo property in properties)
				{
					string name = property.Name;
					string fullName = string.Format(
						CultureInfo.InvariantCulture,
						"{0}.{1}",
						className,
						name);

					object source = property.GetValue(item, null);

					foreach (Rule rule in rules)
					{
						object newValue =
							rule.Run(item, fullName, source, null);

						if ((source != null) && !source.Equals(newValue))
						{
							classType.GetProperty(name).SetValue(
								item, newValue, null);

							changed = true;
						}
					}
				}
			}

			return changed;
		}
	}
}
