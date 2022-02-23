/////////////////////////////////////////////////////////////////////////////
// <copyright file="MediaFileTags.cs" company="Digital Zen Works">
// Copyright © 2019 - 2022 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

#pragma warning disable CS0618 // Type or member is obsolete

using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;

namespace DigitalZenWorks.MusicToolKit
{
	/// <summary>
	/// Media file tags class.
	/// </summary>
	public class MediaFileTags : IDisposable
	{
		private static readonly ResourceManager StringTable =
			new ("DigitalZenWorks.MusicToolKit.Resources", Assembly.GetExecutingAssembly());

		private readonly string filePath;
		private readonly string iTunesLocation;
		private readonly Rules rules;

		/// <summary>
		/// Initializes a new instance of the <see cref="MediaFileTags"/> class.
		/// </summary>
		/// <param name="file">The media file.</param>
		public MediaFileTags(string file)
		{
			filePath = file;

			TagFile = TagLib.File.Create(file);

			if ((TagFile.Tag.Artists.Length > 1) ||
				(TagFile.Tag.Performers.Length > 1))
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MediaFileTags"/> class.
		/// </summary>
		/// <param name="file">The media file.</param>
		/// <param name="iTunesLocation">The iTunes location.</param>
		public MediaFileTags(string file, string iTunesLocation)
			: this(file)
		{
			this.iTunesLocation = iTunesLocation;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MediaFileTags"/> class.
		/// </summary>
		/// <param name="file">The media file.</param>
		/// <param name="iTunesLocation">The iTunes location.</param>
		/// <param name="rules">The rules to use.</param>
		public MediaFileTags(string file, string iTunesLocation, Rules rules)
			: this(file, iTunesLocation)
		{
			this.rules = rules;
		}

		/// <summary>
		/// Gets or sets the album.
		/// </summary>
		/// <value>The album.</value>
		public string Album
		{
			get
			{
				return TagFile.Tag.Album;
			}

			set
			{
				TagFile.Tag.Album = value;
			}
		}

		/// <summary>
		/// Gets or sets the artist.
		/// </summary>
		/// <value>The artist.</value>
		public string Artist
		{
			get
			{
				string artist = null;

				if ((TagFile.Tag.Performers != null) &&
					(TagFile.Tag.Performers.Length > 0))
				{
					artist = TagFile.Tag.Performers[0];
				}

				if (string.IsNullOrWhiteSpace(artist))
				{
					if (TagFile.Tag.AlbumArtists.Length > 0)
					{
						artist = TagFile.Tag.AlbumArtists[0];
					}
				}

				if (string.IsNullOrWhiteSpace(artist))
				{
					if (TagFile.Tag.Artists.Length > 0)
					{
						artist = TagFile.Tag.Artists[0];
					}
				}

				return artist;
			}

			set
			{
				TagFile.Tag.Performers[0] = value;
			}
		}

		/// <summary>
		/// Gets the tag set.
		/// </summary>
		/// <value>The tag set.</value>
		public TagSet TagSet
		{
			get
			{
				TagSet tagSet = new ();
				Type test = tagSet.GetType();
				PropertyInfo[] properties = test.GetProperties();

				foreach (PropertyInfo propertyInfo in properties)
				{
					string name = propertyInfo.Name;

					test = TagFile.Tag.GetType();
					PropertyInfo tagFileInfo = test.GetProperty(name);
					var value = tagFileInfo.GetValue(TagFile.Tag);

					if (propertyInfo.CanWrite)
					{
						propertyInfo.SetValue(tagSet, value);
					}
				}

				return tagSet;
			}
		}

		/// <summary>
		/// Gets or sets the tag file.
		/// </summary>
		/// <value>The tag file.</value>
		public TagLib.File TagFile { get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public string Title { get; set; }

		/// <summary>
		/// Gets or sets the year.
		/// </summary>
		/// <value>The year.</value>
		public uint Year { get; set; }

		/// <summary>
		/// Album remove cd method.
		/// </summary>
		/// <param name="album">The album string.</param>
		/// <returns>An updated album string.</returns>
		public static string AlbumRemoveCd(string album)
		{
			string pattern = @" cd.*?\d";
			album = RegexRemove(pattern, album);

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
			album = RegexRemove(pattern, album);

			return album;
		}

		/// <summary>
		/// Regex remove method.
		/// </summary>
		/// <param name="pattern">The pattern to match.</param>
		/// <param name="content">The content to search.</param>
		/// <returns>An updated album string.</returns>
		public static string RegexRemove(string pattern, string content)
		{
			string output = string.Empty;

			if (Regex.IsMatch(content, pattern, RegexOptions.IgnoreCase))
			{
				output = Regex.Replace(
					content, pattern, string.Empty, RegexOptions.IgnoreCase);
			}

			return output;
		}

		/// <summary>
		/// Album remove cd method.
		/// </summary>
		/// <returns>An updated album string.</returns>
		public string AlbumRemoveCd()
		{
			Album = AlbumRemoveCd(Album);

			return Album;
		}

		/// <summary>
		/// Album remove disc method.
		/// </summary>
		/// <returns>An updated album string.</returns>
		public string AlbumRemoveDisc()
		{
			Album = AlbumRemoveDisc(Album);

			return Album;
		}

		/// <summary>
		/// Dispose method.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Update method.
		/// </summary>
		/// <returns>A value indicating success or not.</returns>
		public bool Update()
		{
			rules.RunRules(this);

			bool artistUpdated = UpdateArtistTag(filePath);

			bool updated = UpdateAlbumTag(filePath);

			bool titleUpdated = UpdateTitleTag();

			Year = TagFile.Tag.Year;

			if ((true == updated) || (true == artistUpdated) ||
				(true == titleUpdated))
			{
				TagFile.Save();
			}

			return updated;
		}

		/// <summary>
		/// Dispose method.
		/// </summary>
		/// <param name="disposing">Indicates whether currently disposing
		/// or not.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// dispose managed resources
				TagFile.Dispose();
				TagFile = null;
			}
		}

		private bool UpdateAlbumTag(string fileName)
		{
			bool updated = false;

			Album = TagFile.Tag.Album;

			// tags are toast, attempt to get from file name
			if (string.IsNullOrWhiteSpace(Album))
			{
				Album = Paths.GetAlbumFromPath(fileName, iTunesLocation);
			}

			Album = AlbumRemoveCd();
			Album = AlbumRemoveDisc();
			Album = Album.Replace(
				"[FLAC]", string.Empty, StringComparison.OrdinalIgnoreCase);
			Album = AlbumReplaceCurlyBraces(Album);

			if (!string.IsNullOrWhiteSpace(Album))
			{
				string breaker = " - ";
				if (Album.Contains(
					breaker, StringComparison.OrdinalIgnoreCase))
				{
					string[] separators = new string[] { breaker };
					string[] parts = Album.Split(
						separators, StringSplitOptions.RemoveEmptyEntries);

					Album = parts[1];
				}

				string pattern = @" \(.*?\)";

				if (Regex.IsMatch(Album, pattern))
				{
					Regex regex = new (pattern);
					MatchCollection matches = regex.Matches(pattern);

					foreach (Match match in matches)
					{
						string foundAtPosition = StringTable.GetString(
								"FOUND_AT_POSITION",
								CultureInfo.InvariantCulture);

						Console.WriteLine(
							foundAtPosition,
							match.Value,
							match.Index);
					}
				}
			}

			if (!Album.Equals(TagFile.Tag.Album, StringComparison.Ordinal))
			{
				updated = true;
			}

			return updated;
		}

		private bool UpdateArtistTag(string fileName)
		{
			bool updated = false;

			if (TagFile.Tag.AlbumArtists.Length > 0)
			{
				Artist = TagFile.Tag.AlbumArtists[0];
			}

			if (string.IsNullOrWhiteSpace(Artist) ||
				Artist.ToUpperInvariant().Equals(
					"VARIOUS ARTISTS", StringComparison.OrdinalIgnoreCase))
			{
				if (TagFile.Tag.Performers.Length > 0)
				{
					Artist = TagFile.Tag.Performers[0];
				}

				if (TagFile.Tag.Artists.Length > 0)
				{
					Artist = TagFile.Tag.Artists[0];
				}
			}

			if (string.IsNullOrWhiteSpace(Artist))
			{
				// attempt to get from filename
				Artist = Paths.GetArtistFromPath(fileName, iTunesLocation);
				updated = true;
			}

			if (!string.IsNullOrWhiteSpace(Artist))
			{
				string breaker = " - ";
				if (Artist.Contains(
					breaker, StringComparison.OrdinalIgnoreCase))
				{
					string[] separators = new string[] { breaker };
					string[] parts = Artist.Split(
						separators, StringSplitOptions.RemoveEmptyEntries);

					Artist = parts[0];
					updated = true;
				}

				string[] regexes =
					new string[] { @" \[.*?\]", @" \(Disc.*?\)", @" Cd.*" };

				foreach (string regex in regexes)
				{
					if (Regex.IsMatch(Artist, regex, RegexOptions.IgnoreCase))
					{
						Artist = Regex.Replace(
							Artist,
							regex,
							string.Empty,
							RegexOptions.IgnoreCase);
						updated = true;
					}
				}
			}

			return updated;
		}

		private bool UpdateTitleTag()
		{
			bool updated = false;

			Title = TagFile.Tag.Title;

			string[] regexes =
				new string[] { @" \[.*?\]", @" \(.*?\)" };

			foreach (string regex in regexes)
			{
				if (Regex.IsMatch(Title, regex))
				{
					Title = Regex.Replace(Title, regex, string.Empty);
					updated = true;
				}
			}

			if ((!string.IsNullOrWhiteSpace(Artist)) &&
				Title.Contains(
					Artist + " - ", StringComparison.OrdinalIgnoreCase))
			{
				Title = Title.Replace(
					Artist + " - ",
					string.Empty,
					StringComparison.OrdinalIgnoreCase);
				TagFile.Tag.Title = Title;
				updated = true;
			}

			return updated;
		}
	}
}
