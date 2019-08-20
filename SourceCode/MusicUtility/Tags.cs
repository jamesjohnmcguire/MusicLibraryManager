#pragma warning disable CS0618 // Type or member is obsolete
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MusicUtility
{
	public class Tags : IDisposable
	{
		private readonly string filePath;
		private readonly string iTunesLocation = null;
		private readonly Rules rules = null;
		private TagLib.File tagFile = null;

		public Tags(string file, string iTunesLocation)
		{
			this.iTunesLocation = iTunesLocation;

			filePath = file;

			tagFile = TagLib.File.Create(file);

			if ((tagFile.Tag.Artists.Length > 1) ||
				(tagFile.Tag.Performers.Length > 1))
			{
				throw new NotSupportedException();
			}
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
				return tagFile.Tag.Album;
			}

			set
			{
				tagFile.Tag.Album = value;
			}
		}

		public string Artist
		{
			get
			{
				string artist = null;

				if (tagFile.Tag.AlbumArtists.Length > 0)
				{
					artist = tagFile.Tag.AlbumArtists[0];
				}

				if (string.IsNullOrWhiteSpace(artist))
				{
					if (tagFile.Tag.Performers.Length > 0)
					{
						artist = tagFile.Tag.Performers[0];
					}
				}

				if (string.IsNullOrWhiteSpace(artist))
				{
					if (tagFile.Tag.Artists.Length > 0)
					{
						artist = tagFile.Tag.Artists[0];
					}
				}

				return artist;
			}

			set
			{

			}

			//set
			//{
			//	tagFile.Tag.Performers[0] = value;
			//}
		}

		public string Title { get; set; }

		public uint Year { get; set; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public bool Update()
		{
			Type classType = typeof(Tags);
			string className = classType.FullName;
			string name = nameof(Album);
			string fullName = string.Format(
				CultureInfo.InvariantCulture, "{0}.{1}", className, name);

			PropertyInfo[] properties = classType.GetProperties();

			foreach (Rule rule in rules.RulesList)
			{
				foreach (PropertyInfo property in properties)
				{
					name = property.Name;
					fullName = string.Format(
						CultureInfo.InvariantCulture, "{0}.{1}", className, name);

					object source = property.GetValue(this, null);

					source = Rules.RunRule(rule, fullName, source);
				}
			}

			bool artistUpdated = UpdateArtistTag(filePath);

			bool updated = UpdateAlbumTag(filePath);

			bool titleUpdated = UpdateTitleTag();

			Year = tagFile.Tag.Year;

			if ((true == updated) || (true == artistUpdated) ||
				(true == titleUpdated))
			{
				tagFile.Save();
			}

			return updated;
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

			if (string.IsNullOrWhiteSpace(Artist) ||
				Artist.ToLower().Equals("various artists"))
			{
				if (tagFile.Tag.Performers.Length > 0)
				{
					Artist = tagFile.Tag.Performers[0];
				}
			}

			if (string.IsNullOrWhiteSpace(Artist) ||
				Artist.ToLower().Equals("various artists"))
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

			Title = tagFile.Tag.Title;

			string[] regexes =
				new string[] { @" \[.*?\]", @" \(.*?\)"  };

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
				tagFile.Tag.Title = Title;
				updated = true;
			}

			return updated;
		}
	}
}
