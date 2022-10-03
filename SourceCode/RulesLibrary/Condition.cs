/////////////////////////////////////////////////////////////////////////////
// <copyright file="Condition.cs" company="Digital Zen Works">
// Copyright © 2019 - 2022 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalZenWorks.MusicToolKit
{
	/// <summary>
	/// Condition types enum.
	/// </summary>
	public enum Condition
	{
		/// <summary>
		/// Condition contains value.
		/// </summary>
		Contains,

		/// <summary>
		/// Condition contains regex value.
		/// </summary>
		ContainsRegex,

		/// <summary>
		/// Condition empty.
		/// </summary>
		Empty,

		/// <summary>
		/// Condition equals.
		/// </summary>
		Equals,

		/// <summary>
		/// Condition greater than.
		/// </summary>
		GreaterThan,

		/// <summary>
		/// Condition less than.
		/// </summary>
		LessThan,

		/// <summary>
		/// Condition matches.
		/// </summary>
		Matches,

		/// <summary>
		/// Condition not empty.
		/// </summary>
		NotEmpty,

		/// <summary>
		/// Condtion not equals.
		/// </summary>
		NotEquals
	}
}
