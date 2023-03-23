﻿/////////////////////////////////////////////////////////////////////////////
// <copyright file="ArtistRules.cs" company="Digital Zen Works">
// Copyright © 2019 - 2023 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace DigitalZenWorks.MusicToolKit
{
	/// <summary>
	/// Artist tag rules class.
	/// </summary>
	public static class ArtistRules
	{
		/// <summary>
		/// Apply exceptions to artist name.
		/// </summary>
		/// <param name="artist">The artist name.</param>
		/// <returns>The updated artist name.</returns>
		public static string ApplyExceptions(string artist)
		{
			if (!string.IsNullOrWhiteSpace(artist))
			{
				Dictionary<string, string> exceptions = new ();
				exceptions.Add("10Cc", "10cc");

				foreach (KeyValuePair<string, string> exception in exceptions)
				{
					artist = artist.Replace(
						exception.Key,
						exception.Value,
						StringComparison.OrdinalIgnoreCase);
				}
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
			if (!string.IsNullOrWhiteSpace(artist))
			{
				string breaker = " - ";
				if (artist.Contains(
					breaker, StringComparison.OrdinalIgnoreCase))
				{
					string[] separators = new string[] { breaker };
					string[] parts = artist.Split(
						separators, StringSplitOptions.RemoveEmptyEntries);

					if (parts.Length > 1)
					{
						string albumCompareCheck = parts[1];

						// Confirm that the first part really contains
						// the artist name.
						if (albumCompareCheck.Equals(
							album, StringComparison.OrdinalIgnoreCase))
						{
							artist = parts[0];
						}
					}
				}
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
