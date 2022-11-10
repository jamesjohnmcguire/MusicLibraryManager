/////////////////////////////////////////////////////////////////////////////
// <copyright file="ArtistTagRules.cs" company="Digital Zen Works">
// Copyright © 2019 - 2022 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace DigitalZenWorks.MusicToolKit
{
	/// <summary>
	/// Artist tag rules class.
	/// </summary>
	public static class ArtistTagRules
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
	}
}
