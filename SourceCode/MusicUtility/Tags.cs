﻿#pragma warning disable CS0618 // Type or member is obsolete
using System;
using System.Text.RegularExpressions;

namespace MusicUtility
{
	public class Tags : IDisposable
	{
		private string iTunesLocation = null;
		private TagLib.File tagFile = null;

		public string Album { get; set; }

		public string Artist { get; set; }

		public string Title { get; set; }

		public uint Year { get; set; }

		public Tags(string file, string iTunesLocation)
		{
			this.iTunesLocation = iTunesLocation;

			tagFile = TagLib.File.Create(file);

			UpdateFileTags(file);

			tagFile.Dispose();
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// dispose managed resources
				tagFile.Dispose();
				tagFile = null;
			}
		}

		private bool UpdateAlbumTag(string fileName)
		{
			bool updated = false;

			Album = tagFile.Tag.Album;

			// tags are toast, attempt to get from file name
			if (string.IsNullOrWhiteSpace(Album))
			{
				Album = Paths.GetAlbumFromPath(fileName, iTunesLocation);
			}

			if (!string.IsNullOrWhiteSpace(Album))
			{
				string[] regexes =
					new string[] { @" \[.*?\]", @" \(Disc.*?Side\)",
						@" \(Disc.*?Res\)", @" \(Disc.*?\)", @" Cd.*",
						@" \(disc \d+\)" };

				foreach (string regex in regexes)
				{
					if (Regex.IsMatch(Album, regex, RegexOptions.IgnoreCase))
					{
						Album = Regex.Replace(
							Album, regex, @"", RegexOptions.IgnoreCase);
					}
				}

				if (Album.EndsWith(" (Disc 2)"))
				{
					Album = Album.Replace(" (Disc 2)", "");
				}

				//string breaker = " - ";
				//if (Album.Contains(breaker))
				//{
				//	string[] separators = new string[] { breaker };
				//	string[] parts = Album.Split(
				//		separators, StringSplitOptions.RemoveEmptyEntries);

				//	Album = parts[1];
				//}
			}

			if (!Album.Equals(tagFile.Tag.Album))
			{
				updated = true;
			}

			return updated;
		}

		private bool UpdateArtistTag(string fileName)
		{
			bool updated = false;

			if (tagFile.Tag.AlbumArtists.Length > 0)
			{
				Artist = tagFile.Tag.AlbumArtists[0];
			}

			if ((string.IsNullOrWhiteSpace(Artist)) ||
				(Artist.ToLower().Equals("various artists")))
			{
				if (tagFile.Tag.Performers.Length > 0)
				{
					Artist = tagFile.Tag.Performers[0];
				}
			}

			if ((string.IsNullOrWhiteSpace(Artist)) ||
				(Artist.ToLower().Equals("various artists")))
			{
				if (tagFile.Tag.Artists.Length > 0)
				{
					Artist = tagFile.Tag.Artists[0];
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
				//string breaker = " - ";
				//if (Artist.Contains(breaker))
				//{
				//	string[] separators = new string[] { breaker };
				//	string[] parts = Artist.Split(
				//		separators, StringSplitOptions.RemoveEmptyEntries);

				//	Artist = parts[0];
				//	updated = true;
				//}

				string[] regexes =
					new string[] { @" \[.*?\]", @" \(Disc.*?\)", @" Cd.*" };

				foreach (string regex in regexes)
				{
					if (Regex.IsMatch(Artist, regex, RegexOptions.IgnoreCase))
					{
						Artist = Regex.Replace(
							Artist, regex, @"", RegexOptions.IgnoreCase);
						updated = true;
					}
				}
			}

			return updated;
		}

		private bool UpdateFileTags(string fileName)
		{
			bool artistUpdated = UpdateArtistTag(fileName);

			bool updated = UpdateAlbumTag(fileName);

			bool titleUpdated = UpdateTitleTag();

			Year = tagFile.Tag.Year;

			if ((true == updated) || (true == artistUpdated) ||
				(true == titleUpdated))
			{
				tagFile.Save();
			}

			return updated;
		}

		private bool UpdateTitleTag()
		{
			bool updated = false;

			Title = tagFile.Tag.Title;

			string[] regexes =
				new string[] { @" \[.*?\]", @" \(.*?\)"  };

			foreach (string regex in regexes)
			{
				if (Regex.IsMatch(Title, regex))
				{
					Title = Regex.Replace(Title, regex, @"");
					updated = true;
				}
			}

			if ((!string.IsNullOrWhiteSpace(Artist)) &&
				(Title.Contains(Artist + " - ")))
			{
				Title = Title.Replace(Artist + " - ", "");
				tagFile.Tag.Title = Title;
				updated = true;
			}

			return updated;
		}
	}
}
