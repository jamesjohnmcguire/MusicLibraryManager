/////////////////////////////////////////////////////////////////////////////
// <copyright file="AlbumRules.cs" company="Digital Zen Works">
// Copyright © 2019 - 2025 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using DigitalZenWorks.RulesLibrary;
using System;

namespace DigitalZenWorks.MusicToolKit
{
	/// <summary>
	/// Album tag rules class.
	/// </summary>
	public static class AlbumRules
	{
		/// <summary>
		/// Create album path from tag.
		/// </summary>
		/// <param name="album">The album tag.</param>
		/// <param name="artist">The artist text to remove.</param>
		/// <returns>A new combined path.</returns>
		public static string AlbumGeneralRules(string album, string artist)
		{
			if (!string.IsNullOrWhiteSpace(album))
			{
				album = GeneralRules.ApplyGeneralRules(album);

				album = ReplaceCurlyBraces(album);
				album = RemoveCd(album);
				album = RemoveDisc(album);
				album = RemoveFlac(album);
				album = RemoveCopyAmount(album);
				album = RemoveArtist(album, artist);
			}

			return album;
		}

		/// <summary>
		/// Create album path from tag.
		/// </summary>
		/// <param name="album">The album tag.</param>
		/// <param name="artist">The artist text to remove.</param>
		/// <returns>A new combined path.</returns>
		public static string CleanAlbumFilePath(string album, string artist)
		{
			if (!string.IsNullOrWhiteSpace(album))
			{
				album = AlbumGeneralRules(album, artist);

				album = Paths.RemoveIllegalPathCharacters(album);
				album = album.TrimEnd('.');
			}

			return album;
		}

		/// <summary>
		/// Remove artist method.
		/// </summary>
		/// <param name="album">The album string.</param>
		/// <param name="artist">The artist name to compare.</param>
		/// <returns>An updated album string.</returns>
		public static string RemoveArtist(string album, string artist)
		{
			if (!string.IsNullOrWhiteSpace(album) &&
				!string.IsNullOrWhiteSpace(artist))
			{
				string compareText = artist + " - ";
				album = GeneralRules.CompareRemove(album, compareText);
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

		/// <summary>
		/// Remove flag method.
		/// </summary>
		/// <param name="album">The album string.</param>
		/// <returns>An updated album string.</returns>
		public static string RemoveFlac(string album)
		{
			if (!string.IsNullOrWhiteSpace(album))
			{
				album = album.Replace(
					"[FLAC]",
					string.Empty,
					StringComparison.OrdinalIgnoreCase);
			}

			return album;
		}
	}
}
