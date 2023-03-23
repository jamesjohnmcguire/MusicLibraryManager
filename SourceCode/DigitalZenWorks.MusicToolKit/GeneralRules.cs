/////////////////////////////////////////////////////////////////////////////
// <copyright file="GeneralRules.cs" company="Digital Zen Works">
// Copyright © 2019 - 2023 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using DigitalZenWorks.RulesLibrary;
using System;

namespace DigitalZenWorks.MusicToolKit
{
	/// <summary>
	/// General rules class.
	/// </summary>
	public static class GeneralRules
	{
		/// <summary>
		/// Apply general rules.
		/// </summary>
		/// <param name="item">The item to process.</param>
		/// <returns>The updated item.</returns>
		public static string ApplyGeneralRules(string item)
		{
			if (item != null)
			{
				item = item.Trim();
				item = item.Replace("  ", " ", StringComparison.OrdinalIgnoreCase);
			}

			return item;
		}
	}
}
