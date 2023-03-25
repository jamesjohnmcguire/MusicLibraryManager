/////////////////////////////////////////////////////////////////////////////
// <copyright file="TitleRules.cs" company="Digital Zen Works">
// Copyright © 2019 - 2023 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using DigitalZenWorks.RulesLibrary;
using Serilog;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DigitalZenWorks.MusicToolKit
{
	/// <summary>
	/// Title tag rules class.
	/// </summary>
	public static class TitleRules
	{
		/// <summary>
		/// Apply title file rules.
		/// </summary>
		/// <param name="title">The title to process.</param>
		/// <param name="artist">The artist to check for.</param>
		/// <param name="isFile">Indicates whether this is for a file
		/// path name.</param>
		/// <returns>The updated title.</returns>
		public static string ApplyTitleFileRules(
			string title, string artist, bool isFile)
		{
			string[] excludes =
			{
				"Back In Black", "Givin The Dog A Bone", "I Have A Dream",
				"Lay All Your Love On Me", "OAMs", "One Of Us",
				"Take A Chance On Me", "Thank You For The Music",
				"The Name Of The Game"
			};

			if (isFile == true)
			{
				title = Paths.RemoveIllegalPathCharacters(title);
			}

			title = GeneralRules.ApplyGeneralRules(title);
			title = GeneralRules.RemoveTrailingNumbers(title);

			if (!excludes.Contains(title))
			{
				title = GeneralRules.GetTitleCase(title);
			}

			title = RemoveBracketedSubTitle(title);
			title = RemoveArtist(title, artist);

			return title;
		}

		/// <summary>
		/// Extract sub title method.
		/// </summary>
		/// <param name="title">The title to check.</param>
		/// <returns>The sub-title.</returns>
		public static string ExtractSubTitle(string title)
		{
			string subTitle = null;

			try
			{
				string pattern = @" \[(.*?)\]";
				Match match = Regex.Match(
					title, pattern, RegexOptions.IgnoreCase);

				if (match.Success == true)
				{
					var groups = match.Groups;

					if (groups.Count > 1)
					{
						subTitle = match.Groups[1].Value;
					}
					else
					{
						subTitle = match.Groups[0].Value;
					}
				}
			}
			catch (Exception exception) when
				(exception is ArgumentException ||
				exception is ArgumentNullException ||
				exception is ArgumentOutOfRangeException ||
				exception is RegexMatchTimeoutException)
			{
				Log.Error(exception.ToString());
			}

			return subTitle;
		}

		/// <summary>
		/// Remove artist method.
		/// </summary>
		/// <param name="title">The title string.</param>
		/// <param name="artist">The artist to compare upon.</param>
		/// <returns>An updated title string.</returns>
		public static string RemoveArtist(string title, string artist)
		{
			if (!string.IsNullOrWhiteSpace(title) &&
				!string.IsNullOrWhiteSpace(artist))
			{
				string compareText = artist + " - ";
				title = GeneralRules.CompareRemove(title, compareText);
			}

			return title;
		}

		/// <summary>
		/// Remove bracketed sub-title method.
		/// </summary>
		/// <param name="title">The title string.</param>
		/// <returns>An updated title string.</returns>
		public static string RemoveBracketedSubTitle(string title)
		{
			if (!string.IsNullOrWhiteSpace(title))
			{
				string regex = @" \[.*?\]";

				title = Rule.RegexRemove(regex, title);
			}

			return title;
		}
	}
}
