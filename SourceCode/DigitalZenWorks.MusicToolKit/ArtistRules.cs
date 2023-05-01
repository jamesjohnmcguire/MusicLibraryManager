/////////////////////////////////////////////////////////////////////////////
// <copyright file="ArtistRules.cs" company="Digital Zen Works">
// Copyright © 2019 - 2023 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace DigitalZenWorks.MusicToolKit
{
	/// <summary>
	/// Artist tag rules class.
	/// </summary>
	public static class ArtistRules
	{
		/// <summary>
		/// Apply artist general rules.
		/// </summary>
		/// <param name="artist">The artist text.</param>
		/// <param name="album">The album text to remove.</param>
		/// <param name="performer">The performer tag to check.</param>
		/// <returns>The updated artist text.</returns>
		public static string ArtistGeneralRules(
			string artist, string album, string performer)
		{
			string[] excludes = { "10cc", "ABBA", "AC/DC" };
			string extraPeriods = @"\.{2,}";

			if (!string.IsNullOrWhiteSpace(artist))
			{
				artist = GeneralRules.ApplyGeneralRules(artist);

				if (!excludes.Contains(artist))
				{
					artist = GeneralRules.GetTitleCase(artist);
				}

				artist = ReplaceVariousArtists(artist, performer);
				artist = RemoveAlbum(artist, album);

				artist = Regex.Replace(artist, extraPeriods, string.Empty);
			}

			return artist;
		}

		/// <summary>
		/// Create artist path from tag.
		/// </summary>
		/// <param name="artist">The artist text.</param>
		/// <param name="album">The album text to remove.</param>
		/// <param name="performer">The performer tag to check.</param>
		/// <returns>The updated artist text.</returns>
		public static string CleanArtistFilePath(
			string artist, string album, string performer)
		{
			if (!string.IsNullOrWhiteSpace(artist))
			{
				artist = ArtistGeneralRules(artist, album, performer);

				artist = Paths.RemoveIllegalPathCharacters(artist);
				artist = artist.TrimEnd('.');
			}

			return artist;
		}

		/// <summary>
		/// Remove album method.
		/// </summary>
		/// <param name="artist">The artist string.</param>
		/// <param name="album">The album name to compare.</param>
		/// <returns>An updated artist string.</returns>
		public static string RemoveAlbum(string artist, string album)
		{
			if (!string.IsNullOrWhiteSpace(artist) &&
				!string.IsNullOrWhiteSpace(album))
			{
				string compareText = " - " + album;
				artist = GeneralRules.CompareRemove(artist, compareText);
			}

			return artist;
		}

		/// <summary>
		/// Replace various artists method.
		/// </summary>
		/// <param name="artist">The artist string.</param>
		/// <param name="replacement">The replacement artist name.</param>
		/// <returns>An updated artist string.</returns>
		public static string ReplaceVariousArtists(
			string artist, string replacement)
		{
			if (!string.IsNullOrWhiteSpace(artist))
			{
				if (artist.ToUpperInvariant().Equals(
						"VARIOUS ARTISTS", StringComparison.OrdinalIgnoreCase))
				{
					artist = replacement;
				}
			}

			return artist;
		}
	}
}
