/////////////////////////////////////////////////////////////////////////////
// <copyright file="GeneralRules.cs" company="Digital Zen Works">
// Copyright © 2019 - 2023 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using DigitalZenWorks.Common.Utilities.Extensions;
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
				item = Regex.Replace(item, @"\s+", " ");
			}

			return item;
		}

		/// <summary>
		/// Compare and remove, if matched.
		/// </summary>
		/// <param name="item">The item to check.</param>
		/// <param name="compareText">The text to check for.</param>
		/// <returns>The updated item.</returns>
		public static string CompareRemove(string item, string compareText)
		{
			if (!string.IsNullOrWhiteSpace(item) &&
				!string.IsNullOrWhiteSpace(compareText))
			{
				if (item.Contains(
					compareText, StringComparison.OrdinalIgnoreCase))
				{
					item = item.Replace(
						compareText,
						string.Empty,
						StringComparison.OrdinalIgnoreCase);
				}
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
			title = title.ToTitleCase();

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
