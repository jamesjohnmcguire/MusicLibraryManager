/////////////////////////////////////////////////////////////////////////////
// <copyright file="MediaFileTags.cs" company="Digital Zen Works">
// Copyright © 2019 - 2022 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Common.Logging;
using DigitalZenWorks.RulesLibrary;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Resources;

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
				}

				return artist;
			}

			set
			{
				artist = value;

				if ((TagFile.Tag.Performers != null) &&
					(TagFile.Tag.Performers.Length > 0))
				{
					TagFile.Tag.Performers[0] = artist;
				}
				else
				{
					string[] artists = new string[1];
					artists[0] = artist;

					TagFile.Tag.Performers = artists;
				}
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

				tags = GetTagFromPropertyInfo(tags, propertyInfo);
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

			bool subTitleUpdated = UpdateSubTitleTag();

			if (rulesUpdated == true || albumUpdated == true ||
				artistUpdated == true || subTitleUpdated == true ||
				titleUpdated == true)
			{
				TagFile.Save();
				updated = true;

				Log.Info("Updated Tags in: " + filePath);
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

		private SortedDictionary<string, object> GetTagFromPropertyInfo(
			SortedDictionary<string, object> tags, PropertyInfo propertyInfo)
		{
			Type tagType = TagFile.Tag.GetType();

			string name = propertyInfo.Name;

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

			return tags;
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

			if (!string.IsNullOrWhiteSpace(Album))
			{
				Album = TagRules.Trim(Album);

				Album = AlbumTagRules.RemoveCd(Album);
				Album = AlbumTagRules.RemoveDisc(Album);
				Album = AlbumTagRules.RemoveFlac(Album);
				Album = AlbumTagRules.ReplaceCurlyBraces(Album);
				Album = AlbumTagRules.RemoveCopyAmount(Album);

				Album = AlbumTagRules.RemoveArtist(Album);
			}

			if (!string.IsNullOrWhiteSpace(Album) &&
				!Album.Equals(previousAlbum, StringComparison.Ordinal))
			{
				updated = true;
				Log.Info("Updating Album Tag");
			}

			return updated;
		}

		private bool UpdateArtistTag(string fileName)
		{
			bool updated = false;
			string previousArtist = Artist;

			if (TagFile.Tag.Performers.Length > 0)
			{
				Artist = TagFile.Tag.Performers[0];
			}

			if (string.IsNullOrWhiteSpace(Artist) &&
				TagFile.Tag.AlbumArtists.Length > 0)
			{
				Artist = TagFile.Tag.AlbumArtists[0];
			}

			if (string.IsNullOrWhiteSpace(Artist))
			{
				// attempt to get from filename
				Artist = Paths.GetArtistFromPath(fileName);
			}

			if (!string.IsNullOrWhiteSpace(Artist))
			{
				Artist = TagRules.Trim(Artist);

				Artist = TagRules.GetTitleCase(Artist);
				Artist = ArtistTagRules.ApplyExceptions(Artist);

				Artist = ArtistTagRules.ReplaceVariousArtists(
					Artist, TagFile.Tag.Performers[0]);
				Artist = ArtistTagRules.RemoveAlbum(Artist);
			}

			if (!string.IsNullOrWhiteSpace(Artist) &&
				!Artist.Equals(previousArtist, StringComparison.Ordinal))
			{
				updated = true;
				Log.Info("Updating Artist Tag");
			}

			return updated;
		}

		private bool UpdateSubTitleTag()
		{
			bool updated = false;

			string previousSubTitle = TagFile.Tag.Subtitle;

			if (string.IsNullOrWhiteSpace(previousSubTitle))
			{
				string subTitle = TitleTagRules.ExtractSubTitle(Title);

				if (!string.IsNullOrWhiteSpace(subTitle))
				{
					TagFile.Tag.Subtitle = subTitle;

					updated = true;
					Log.Info("Updating Sub Title Tag");
				}
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
				Title = TagRules.Trim(Title);

				Title = TitleTagRules.RemoveBracketedSubTitle(Title);

				Title = TitleTagRules.RemoveArtist(Title, Artist);
			}

			if (!string.IsNullOrWhiteSpace(Title) &&
				!Title.Equals(previousTitle, StringComparison.Ordinal))
			{
				updated = true;
				Log.Info("Updating Title Tag");
			}

			return updated;
		}
	}
}
