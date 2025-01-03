/////////////////////////////////////////////////////////////////////////////
// <copyright file="MediaFileTags.cs" company="Digital Zen Works">
// Copyright © 2019 - 2025 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Common.Logging;
using DigitalZenWorks.RulesLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
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

#pragma warning disable CA1823
		private static readonly ResourceManager StringTable =
			new ("DigitalZenWorks.MusicToolKit.Resources",
				Assembly.GetExecutingAssembly());
#pragma warning restore CA1823

		private readonly string filePath;
		private readonly Rules rules;

		private string artist;

		private bool updated;

		/// <summary>
		/// Initializes a new instance of the <see cref="MediaFileTags"/> class.
		/// </summary>
		/// <param name="file">The media file.</param>
		public MediaFileTags(string file)
		{
			filePath = file;

			TagFile = TagLib.File.Create(file);

			artist = GetFirstPerformerSafe();

			if (string.IsNullOrWhiteSpace(artist))
			{
				if ((TagFile.Tag.AlbumArtists != null) &&
					TagFile.Tag.AlbumArtists.Length > 0)
				{
					artist = TagFile.Tag.AlbumArtists[0];
				}
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
				if ((TagFile.Tag.Album != null &&
					!TagFile.Tag.Album.Equals(
						value, StringComparison.Ordinal)) ||
					(TagFile.Tag.Album == null && value != null))
				{
					updated = true;
					TagFile.Tag.Album = value;
				}
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
				return artist;
			}

			set
			{
				if ((artist != null &&
					!artist.Equals(value, StringComparison.Ordinal)) ||
					(artist == null && value != null))
				{
					updated = true;
					artist = value;
				}

				if ((TagFile.Tag.Performers != null) &&
					(TagFile.Tag.Performers.Length > 0))
				{
					TagFile.Tag.Performers[0] = artist;
				}
				else
				{
					string[] artists = [artist];
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
				if ((TagFile.Tag.Title != null &&
					!TagFile.Tag.Title.Equals(
						value, StringComparison.Ordinal)) ||
					(TagFile.Tag.Title == null && value != null))
				{
					updated = true;
					TagFile.Tag.Title = value;
				}
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
				if (TagFile.Tag.Year != value)
				{
					updated = true;
					TagFile.Tag.Year = value;
				}
			}
		}

		/// <summary>
		/// Clean tags method.
		/// </summary>
		/// <param name="useFilePath">Indicates whether to use the file path
		/// segments, when a tag is empty.</param>
		/// <returns>A value indicating whether the tags were updated
		/// or not.</returns>
		public bool Clean(bool useFilePath = false)
		{
			bool isUpdated = false;
			bool rulesUpdated = false;

			if (rules != null)
			{
				rulesUpdated = rules.RunRules(this);
			}

			bool artistUpdated = UpdateArtistTag(filePath, useFilePath);

			bool albumUpdated = UpdateAlbumTag(filePath, useFilePath);

			bool titleUpdated = UpdateTitleTag(useFilePath);

			bool subTitleUpdated = UpdateSubTitleTag();

			if (rulesUpdated == true || albumUpdated == true ||
				artistUpdated == true || subTitleUpdated == true ||
				titleUpdated == true)
			{
				Update();

				isUpdated = true;
			}

			return isUpdated;
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
			SortedDictionary<string, object> tags = [];

			Type tagType = TagFile.Tag.GetType();

			PropertyInfo[] properties = tagType.GetProperties();

			foreach (PropertyInfo propertyInfo in properties)
			{
				string name = propertyInfo.Name;

				if (name.Equals(
						"EndTag", StringComparison.OrdinalIgnoreCase) ||
					name.Equals(
						"Pictures", StringComparison.OrdinalIgnoreCase) ||
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
			bool isUpdated = updated;

			if (updated == true)
			{
				TagFile.Save();

				Log.Info("Updated Tags in: " + filePath);

				// reset for next time
				updated = false;
			}

			return isUpdated;
		}

		/// <summary>
		/// Update tags from the file path segments.
		/// </summary>
		/// <param name="force">Indicates whether to always update the tags or
		/// only when the tags are empty.</param>
		public void UpdateTagsFromFilPath(bool force = false)
		{
			if (string.IsNullOrWhiteSpace(TagFile.Tag.Album) || force == true)
			{
				Album = Paths.GetAlbumFromPath(filePath);
			}

			if (string.IsNullOrWhiteSpace(Artist) || force == true)
			{
				Artist = Paths.GetArtistFromPath(filePath);
			}

			if (string.IsNullOrWhiteSpace(TagFile.Tag.Title) || force == true)
			{
				Title = Paths.GetTitleFromPath(filePath);
			}
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

		private string GetFirstPerformerSafe()
		{
			string peformer = null;

			if ((TagFile.Tag.Performers != null) &&
				(TagFile.Tag.Performers.Length > 0))
			{
				peformer = TagFile.Tag.Performers[0];
			}

			return peformer;
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

		private bool UpdateAlbumTag(string fileName, bool useFilePath)
		{
			bool updated = false;
			Album = TagFile.Tag.Album;
			string previousAlbum = Album;

			if (string.IsNullOrWhiteSpace(Album) && useFilePath == true)
			{
				Album = Paths.GetAlbumFromPath(fileName);
			}

			if (!string.IsNullOrWhiteSpace(Album))
			{
				Album = AlbumRules.AlbumGeneralRules(Album, Artist);

				if (!Album.Equals(previousAlbum, StringComparison.Ordinal))
				{
					updated = true;
					Log.Info("Updating Album Tag");
				}
			}

			return updated;
		}

		private bool UpdateArtistTag(string fileName, bool useFilePath)
		{
			bool updated = false;
			string previousArtist = Artist;

			Artist = GetFirstPerformerSafe();

			if (string.IsNullOrWhiteSpace(Artist) &&
				TagFile.Tag.AlbumArtists.Length > 0)
			{
				Artist = TagFile.Tag.AlbumArtists[0];
			}

			if (string.IsNullOrWhiteSpace(Artist) && useFilePath == true)
			{
				Artist = Paths.GetArtistFromPath(fileName);
			}

			if (!string.IsNullOrWhiteSpace(Artist))
			{
				string performer = GetFirstPerformerSafe();
				Artist = ArtistRules.ArtistGeneralRules(
					Artist, Album, performer);

				if (!Artist.Equals(previousArtist, StringComparison.Ordinal))
				{
					updated = true;
					Log.Info("Updating Artist Tag");
				}
			}

			return updated;
		}

		private bool UpdateSubTitleTag()
		{
			bool updated = false;

			string previousSubTitle = TagFile.Tag.Subtitle;

			if (string.IsNullOrWhiteSpace(previousSubTitle))
			{
				string subTitle = TitleRules.ExtractSubTitle(Title);

				if (!string.IsNullOrWhiteSpace(subTitle))
				{
					TagFile.Tag.Subtitle = subTitle;

					updated = true;
					Log.Info("Updating Sub Title Tag");
				}
			}

			return updated;
		}

		private bool UpdateTitleTag(bool useFilePath)
		{
			bool updated = false;

			Title = TagFile.Tag.Title;
			string previousTitle = Title;

			if (string.IsNullOrEmpty(Title) && useFilePath == true)
			{
				Title = Paths.GetTitleFromPath(filePath);
			}

			if (!string.IsNullOrEmpty(Title))
			{
				Title = TitleRules.ApplyTitleRules(Title, Artist);
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
