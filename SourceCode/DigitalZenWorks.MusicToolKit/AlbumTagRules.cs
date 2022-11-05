/////////////////////////////////////////////////////////////////////////////
// <copyright file="AlbumTagRules.cs" company="Digital Zen Works">
// Copyright © 2019 - 2022 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using DigitalZenWorks.RulesLibrary;
using System;
using System.Text;

namespace DigitalZenWorks.MusicToolKit
{
	/// <summary>
	/// Album tag rules class.
	/// </summary>
	public static class AlbumTagRules
	{
		/// <summary>
		/// Album remove cd method.
		/// </summary>
		/// <param name="album">The album string.</param>
		/// <returns>An updated album string.</returns>
		public static string AlbumRemoveCd(string album)
		{
			string pattern = @" cd.*?\d";
			album = Rule.RegexRemove(pattern, album);

			return album;
		}

		/// <summary>
		/// Album remove copy amount method.
		/// </summary>
		/// <param name="album">The album string.</param>
		/// <returns>An updated album string.</returns>
		public static string AlbumRemoveCopyAmount(string album)
		{
			if (!string.IsNullOrWhiteSpace(album))
			{
				string pattern = @" \(\d*\)$";
				album = Rule.RegexRemove(pattern, album);
			}

			return album;
		}

		/// <summary>
		/// Album replace curly braces method.
		/// </summary>
		/// <param name="album">The album string.</param>
		/// <returns>An updated album string.</returns>
		public static string AlbumReplaceCurlyBraces(string album)
		{
			if (!string.IsNullOrWhiteSpace(album))
			{
				album = album.Replace('{', '[');
				album = album.Replace('}', ']');
			}

			return album;
		}

		/// <summary>
		/// Album remove disc method.
		/// </summary>
		/// <param name="album">The album string.</param>
		/// <returns>An updated album string.</returns>
		public static string AlbumRemoveDisc(string album)
		{
			string pattern = @"\s*\(Dis(c|k).*?\)";
			album = Rule.RegexRemove(pattern, album);

			return album;
		}
	}
}
