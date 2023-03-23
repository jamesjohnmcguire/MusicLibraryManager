/////////////////////////////////////////////////////////////////////////////
// <copyright file="GeneralRules.cs" company="Digital Zen Works">
// Copyright © 2019 - 2023 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Globalization;
using System.Text.RegularExpressions;

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

		/// <summary>
		/// Get title case.
		/// </summary>
		/// <param name="title">The current title.</param>
		/// <returns>The updated title.</returns>
		public static string GetTitleCase(string title)
		{
			CultureInfo cultureInfo = CultureInfo.CurrentCulture;
			TextInfo textInfo = cultureInfo.TextInfo;
			title = textInfo.ToTitleCase(title);

			return title;
		}

		/// <summary>
		/// Remove trailing numbers.
		/// </summary>
		/// <param name="text">The text to process.</param>
		/// <returns>The updated text.</returns>
		public static string RemoveTrailingNumbers(string text)
		{
			text = Regex.Replace(text, @"\s+\d+$", string.Empty);

			return text;
		}
	}
}
