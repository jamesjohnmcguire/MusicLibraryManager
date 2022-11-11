/////////////////////////////////////////////////////////////////////////////
// <copyright file="TagRules.cs" company="Digital Zen Works">
// Copyright © 2019 - 2022 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Globalization;

namespace DigitalZenWorks.MusicToolKit
{
	/// <summary>
	/// General Tag Rules.
	/// </summary>
	public static class TagRules
	{
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
		/// Trim method.
		/// </summary>
		/// <param name="tag">The tag string.</param>
		/// <returns>An updated tag string.</returns>
		public static string Trim(string tag)
		{
			if (!string.IsNullOrWhiteSpace(tag))
			{
				tag = tag.Trim();
			}

			return tag;
		}
	}
}
