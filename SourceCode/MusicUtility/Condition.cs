using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicUtility
{
	public enum Condition
	{
		Contains,
		ContainsRegex,
		Empty,
		Equals,
		GreaterThan,
		LessThan,
		Matches,
		NotEmpty,
		NotEqual
	}
}
