/////////////////////////////////////////////////////////////////////////////
// <copyright file="AlbumTagRules.cs" company="Digital Zen Works">
// Copyright © 2019 - 2022 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using DigitalZenWorks.RulesLibrary;
using System;

namespace DigitalZenWorks.MusicToolKit
{
	/// <summary>
	/// Album tag rules class.
	/// </summary>
	public static class AlbumTagRules
	{
		/// <summary>
		/// Remove artist method.
		/// </summary>
		/// <param name="album">The album string.</param>
		/// <returns>An updated album string.</returns>
		public static string RemoveArtist(string album)
		{
			if (!string.IsNullOrWhiteSpace(album))
			{
				string breaker = " - ";
				if (album.Contains(
					breaker, StringComparison.OrdinalIgnoreCase))
				{
					string[] separators = new string[] { breaker };
					string[] parts = album.Split(
						separators, StringSplitOptions.RemoveEmptyEntries);

					if (parts.Length > 1)
					{
						album = parts[1];
					}
				}
			}

			return album;
		}

		/// <summary>
		/// Remove cd method.
		/// </summary>
		/// <param name="album">The album string.</param>
		/// <returns>An updated album string.</returns>
		public static string RemoveCd(string album)
		{
			string pattern = @" cd.*?\d";
			album = Rule.RegexRemove(pattern, album);

			return album;
		}

		/// <summary>
		/// Remove copy amount method.
		/// </summary>
		/// <param name="album">The album string.</param>
		/// <returns>An updated album string.</returns>
		public static string RemoveCopyAmount(string album)
		{
			if (!string.IsNullOrWhiteSpace(album))
			{
				string pattern = @" \(\d*\)$";
				album = Rule.RegexRemove(pattern, album);
			}

			return album;
		}

		/// <summary>
		/// Replace curly braces method.
		/// </summary>
		/// <param name="album">The album string.</param>
		/// <returns>An updated album string.</returns>
		public static string ReplaceCurlyBraces(string album)
		{
			if (!string.IsNullOrWhiteSpace(album))
			{
				album = album.Replace('{', '[');
				album = album.Replace('}', ']');
			}

			return album;
		}

		/// <summary>
		/// Remove disc method.
		/// </summary>
		/// <param name="album">The album string.</param>
		/// <returns>An updated album string.</returns>
		public static string RemoveDisc(string album)
		{
			string pattern = @"\s*\(Dis(c|k).*?\)";
			album = Rule.RegexRemove(pattern, album);

			return album;
		}
	}
}
