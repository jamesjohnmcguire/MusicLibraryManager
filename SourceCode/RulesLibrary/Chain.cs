/////////////////////////////////////////////////////////////////////////////
// <copyright file="Chain.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.RulesLibrary
{
	/// <summary>
	/// Chain types enum.
	/// </summary>
	public enum Chain
	{
		/// <summary>
		/// No chaining.
		/// </summary>
		None,

		/// <summary>
		/// Chain by anding.
		/// </summary>
		And,

		/// <summary>
		/// Chain by oring.
		/// </summary>
		Or,

		/// <summary>
		/// Chain by xoring.
		/// </summary>
		Xor
	}
}
