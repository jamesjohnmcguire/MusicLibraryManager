/////////////////////////////////////////////////////////////////////////////
// <copyright file="GeneralRules.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MusicToolKit
{
	using System;
	using System.Globalization;
	using System.Text.RegularExpressions;
	using DigitalZenWorks.Common.Utilities.Extensions;

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

				// Remove extra spaces.
				item = Regex.Replace(item, @"\s+", " ");

				item = CapitalizeFirstCharacter(item);
			}

			return item;
		}

		/// <summary>
		/// Capitalize First Character.
		/// </summary>
		/// <param name="item">The item to process.</param>
		/// <returns>The updated item.</returns>
		public static string CapitalizeFirstCharacter(string item)
		{
			if (!string.IsNullOrWhiteSpace(item))
			{
				char first = item[0];
				first = char.ToUpper(first, CultureInfo.InvariantCulture);

				string remaining = item[1..];

				item = string.Concat(first, remaining);
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
			if (!string.IsNullOrWhiteSpace(text))
			{
				Match match = Regex.Match(
					text, @"\d+$", RegexOptions.RightToLeft);

				if (match.Success == true)
				{
					string numbersText = match.Value;
					int numbers = Convert.ToInt32(
						numbersText, CultureInfo.InvariantCulture);

					if (numbers < 10)
					{
						// Trailing numbers greater then 9 probably mean
						// something else besides a duplicate indicator.
						text = Regex.Replace(text, @"\s+\d+$", string.Empty);
					}
				}
			}

			return text;
		}
	}
}
