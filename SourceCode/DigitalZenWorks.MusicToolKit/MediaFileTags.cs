/////////////////////////////////////////////////////////////////////////////
// <copyright file="MediaFileTags.cs" company="Digital Zen Works">
// Copyright © 2019 - 2022 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

#pragma warning disable CS0618 // Type or member is obsolete

using Common.Logging;
using DigitalZenWorks.RulesLibrary;
using System;
using System.Collections.Generic;
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
		private static readonly ILog Log = LogManager.GetLogger(
			MethodBase.GetCurrentMethod().DeclaringType);

		private static readonly ResourceManager StringTable =
			new ("DigitalZenWorks.MusicToolKit.Resources", Assembly.GetExecutingAssembly());

		private readonly string filePath;
		private readonly Rules rules;

		private string artist;

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
		/// <param name="rules">The rules to use.</param>
		public MediaFileTags(string file, Rules rules)
			: this(file)
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
		/// Gets or sets the primary artist.
		/// </summary>
		/// <value>The artist.</value>
		public string Artist
		{
			get
			{
				if (string.IsNullOrWhiteSpace(artist))
				{
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
				}

				return artist;
			}

			set
			{
				artist = value;

				string[] artists = new string[1];
				artists[0] = artist;

				TagFile.Tag.Performers = artists;
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
		public string Title
		{
			get
			{
				return TagFile.Tag.Title;
			}

			set
			{
				TagFile.Tag.Title = value;
			}
		}

		/// <summary>
		/// Gets or sets the year.
		/// </summary>
		/// <value>The year.</value>
		public uint Year
		{
			get
			{
				return TagFile.Tag.Year;
			}

			set
			{
				TagFile.Tag.Year = value;
			}
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
		/// Get the full set of tags.
		/// </summary>
		/// <returns>The full set of tags.</returns>
		public SortedDictionary<string, object> GetTags()
		{
			SortedDictionary<string, object> tags = new ();

			Type tagType = TagFile.Tag.GetType();

			PropertyInfo[] properties = tagType.GetProperties();

			foreach (PropertyInfo propertyInfo in properties)
			{
				string name = propertyInfo.Name;

				if (name.Equals(
						"EndTag", StringComparison.OrdinalIgnoreCase) ||
					name.Equals(
						"StartTag", StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}

				PropertyInfo tagFileInfo = tagType.GetProperty(name);
				object value = tagFileInfo.GetValue(TagFile.Tag);

				if (value != null)
				{
// Closing square brackets should be spaced correctly
#pragma warning disable SA1011
					switch (value)
					{
						case bool:
							break;
						case double number:
							if (!double.IsNaN(number))
							{
								tags.Add(name, value);
							}

							break;
						case string[] stringArray:
							if (stringArray.Length > 0)
							{
								tags.Add(name, value);
							}

							break;
						case TagLib.IPicture[] picture:
							if (picture.Length > 0)
							{
								tags.Add(name, value);
							}

							break;
						case TagLib.Tag[]:
							break;
						case uint number:
							if (number > 0)
							{
								tags.Add(name, value);
							}

							break;
						default:
							tags.Add(name, value);
							break;
					}

// Closing square brackets should be spaced correctly
#pragma warning restore SA1011
				}
			}

			return tags;
		}

		/// <summary>
		/// Update method.
		/// </summary>
		/// <returns>A value indicating success or not.</returns>
		public bool Update()
		{
			bool updated = false;
			bool rulesUpdated = false;

			if (rules != null)
			{
				rulesUpdated = rules.RunRules(this);
			}

			bool artistUpdated = UpdateArtistTag(filePath);

			bool albumUpdated = UpdateAlbumTag(filePath);

			bool titleUpdated = UpdateTitleTag();

			if ((true == albumUpdated) || (true == artistUpdated) ||
				(true == titleUpdated) || true == rulesUpdated)
			{
				TagFile.Save();
				updated = true;
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

		private static string ExtractSubTitle(string title)
		{
			string subTitle = null;

			try
			{
				string pattern = @" \[.*?\]";
				Match match = Regex.Match(
					title, pattern, RegexOptions.IgnoreCase);

				if (match.Success == true)
				{
					subTitle = match.Value;
					subTitle = subTitle.Trim();
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

		private bool UpdateAlbumTag(string fileName)
		{
			bool updated = false;
			Album = TagFile.Tag.Album;
			string previousAlbum = Album;

			// tags are toast, attempt to get from file name
			if (string.IsNullOrWhiteSpace(Album))
			{
				Album = Paths.GetAlbumFromPath(fileName);
			}

			Album = AlbumTagRules.AlbumRemoveCd(Album);
			Album = AlbumTagRules.AlbumRemoveDisc(Album);
			Album = Album.Replace(
				"[FLAC]", string.Empty, StringComparison.OrdinalIgnoreCase);
			Album = AlbumTagRules.AlbumReplaceCurlyBraces(Album);
			Album = AlbumTagRules.AlbumRemoveCopyAmount(Album);

			if (!string.IsNullOrWhiteSpace(Album))
			{
				Album = Album.Trim();

				string breaker = " - ";
				if (Album.Contains(
					breaker, StringComparison.OrdinalIgnoreCase))
				{
					string[] separators = new string[] { breaker };
					string[] parts = Album.Split(
						separators, StringSplitOptions.RemoveEmptyEntries);

					Album = parts[1];
				}
			}

			if (!string.IsNullOrWhiteSpace(Album) &&
				!Album.Equals(previousAlbum, StringComparison.Ordinal))
			{
				updated = true;
			}

			return updated;
		}

		private bool UpdateArtistTag(string fileName)
		{
			bool updated = false;
			string previousArtist = Artist;

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
				Artist = Paths.GetArtistFromPath(fileName);
			}

			if (!string.IsNullOrWhiteSpace(Artist))
			{
				Artist = Artist.Trim();

				string breaker = " - ";
				if (Artist.Contains(
					breaker, StringComparison.OrdinalIgnoreCase))
				{
					string[] separators = new string[] { breaker };
					string[] parts = Artist.Split(
						separators, StringSplitOptions.RemoveEmptyEntries);

					Artist = parts[0];
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
					}
				}
			}

			if (!string.IsNullOrWhiteSpace(Artist) &&
				!Artist.Equals(previousArtist, StringComparison.Ordinal))
			{
				updated = true;
			}

			return updated;
		}

		private bool UpdateTitleTag()
		{
			bool updated = false;

			Title = TagFile.Tag.Title;
			string previousTitle = Title;

			if (string.IsNullOrEmpty(Title))
			{
				Title = Paths.GetTitleFromPath(filePath);
			}

			if (!string.IsNullOrEmpty(Title))
			{
				Title = Title.Trim();

				string[] regexes =
				new string[] { @" \[.*?\]" };

				foreach (string regex in regexes)
				{
					string subTitle = ExtractSubTitle(Title);

					if (!string.IsNullOrWhiteSpace(subTitle))
					{
						TagFile.Tag.Subtitle = subTitle;
					}

					Title = Rule.RegexRemove(regex, Title);
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
				}
			}

			if (!string.IsNullOrWhiteSpace(Title) &&
				!Title.Equals(previousTitle, StringComparison.Ordinal))
			{
				updated = true;
			}

			return updated;
		}
	}
}
