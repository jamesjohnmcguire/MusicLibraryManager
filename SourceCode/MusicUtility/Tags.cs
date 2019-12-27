#pragma warning disable CS0618 // Type or member is obsolete
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using TagLib;

namespace MusicUtility
{
	public class Tags : IDisposable
	{
		private readonly string filePath;
		private readonly string iTunesLocation = null;
		private readonly Rules rules = null;

		public Tags(string file)
		{
			filePath = file;

			TagFile = TagLib.File.Create(file);

			if ((TagFile.Tag.Artists.Length > 1) ||
				(TagFile.Tag.Performers.Length > 1))
			{
				throw new NotSupportedException();
			}
		}

		public Tags(string file, string iTunesLocation)
			: this(file)
		{
			this.iTunesLocation = iTunesLocation;
		}

		public Tags(string file, string iTunesLocation, Rules rules)
			: this(file, iTunesLocation)
		{
			this.rules = rules;
		}

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

		public Tag TagSet { get; }

		public TagLib.File TagFile { get; set; }

		public string Title { get; set; }

		public uint Year { get; set; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public bool Update()
		{
			bool rulesUpdates = rules.RunRules(this);

			bool artistUpdated = UpdateArtistTag(filePath);

			bool updated = UpdateAlbumTag(filePath);

			bool titleUpdated = UpdateTitleTag();

			Year = TagFile.Tag.Year;

			if ((true == updated) || (true == artistUpdated) ||
				(true == titleUpdated) || (rulesUpdates == true))
			{
				TagFile.Save();
			}

			return updated;
		}

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
							Album,
							regex,
							string.Empty,
							RegexOptions.IgnoreCase);
					}
				}

				if (Album.EndsWith(" (Disc 2)"))
				{
					Album = Album.Replace(" (Disc 2)", string.Empty);
				}

				if (Album.EndsWith(" (Disc 2)"))
				{
					Album = Album.Replace(" (Disc 2)", string.Empty);
				}

				string breaker = " - ";
				if (Album.Contains(breaker))
				{
					string[] separators = new string[] { breaker };
					string[] parts = Album.Split(
						separators, StringSplitOptions.RemoveEmptyEntries);

					Album = parts[1];
				}

				string pattern = @" \(.*?\)";

				if (Regex.IsMatch(Album, pattern))
				{
					Regex regex = new Regex(pattern);
					MatchCollection matches = regex.Matches(pattern);

					foreach (Match match in matches)
					{
						Console.WriteLine(
							"Found '{0}' at position {1}",
							match.Value,
							match.Index);
					}
				}
			}

			if (!Album.Equals(TagFile.Tag.Album))
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
				if (Artist.Contains(breaker))
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
				Title.Contains(Artist + " - "))
			{
				Title = Title.Replace(Artist + " - ", string.Empty);
				TagFile.Tag.Title = Title;
				updated = true;
			}

			return updated;
		}
	}
}
