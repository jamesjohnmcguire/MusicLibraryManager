/////////////////////////////////////////////////////////////////////////////
// <copyright file="Condition.cs" company="Digital Zen Works">
// Copyright © 2019 - 2020 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

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
		NotEquals
	}
}
