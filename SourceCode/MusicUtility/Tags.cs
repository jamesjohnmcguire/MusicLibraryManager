using System.Text.RegularExpressions;

namespace MusicUtility
{
	public class Tags
	{
		private string album = null;
		private string artist = null;
		private TagLib.File file = null;
		private string title = null;
		private uint year = 0;

		public string Album
		{
			get
			{
				return album;
			}
		}

		public string Artist
		{
			get
			{
				return artist;
			}
		}

		public string Title
		{
			get
			{
				return title;
			}
		}

		public uint Year
		{
			get
			{
				return year;
			}
		}

		public Tags(string file)
		{
			bool updated = false;
			string regex = @" \[.*?\]";

			TagLib.File musicFile = TagLib.File.Create(file);

			UpdateFileTags(musicFile, file);
			album = musicFile.Tag.Album;

			if (musicFile.Tag.AlbumArtists.Length > 0)
			{
				artist = musicFile.Tag.AlbumArtists[0];
			}
			else if (musicFile.Tag.Performers.Length > 0)
			{
				artist = musicFile.Tag.Performers[0];
			}
			else if (musicFile.Tag.Artists.Length > 0)
			{
				artist = musicFile.Tag.Artists[0];
			}

			title = musicFile.Tag.Title;
			year = musicFile.Tag.Year;

			musicFile.Dispose();
		}

		public bool UpdateFileTags(TagLib.File musicFile, string fileName)
		{
			bool updated = false;
			string regex = @" \[.*?\]";

			string album = UpdateAlbumTag(musicFile, fileName);

			if (!album.Equals(musicFile.Tag.Album))
			{
				musicFile.Tag.Album = album;
				updated = true;
			}

			if (musicFile.Tag.AlbumArtists.Length > 0)
			{
				artist = musicFile.Tag.AlbumArtists[0];
			}
			else if (musicFile.Tag.Performers.Length > 0)
			{
				artist = musicFile.Tag.Performers[0];
			}
			else if (musicFile.Tag.Artists.Length > 0)
			{
				artist = musicFile.Tag.Artists[0];
			}

			title = musicFile.Tag.Title;
			if (Regex.IsMatch(title, regex))
			{
				title = Regex.Replace(title, regex, @"");
				musicFile.Tag.Title = title;
				updated = true;
			}

			if ((!string.IsNullOrWhiteSpace(artist)) &&
				(title.Contains(artist + " - ")))
			{
				title = title.Replace(artist + " - ", "");
				musicFile.Tag.Title = title;
				updated = true;
			}

			if (true == updated)
			{
				musicFile.Save();
			}

			return updated;
		}

		private static string UpdateAlbumTag(
			TagLib.File musicFile, string fileName)
		{
			string album = musicFile.Tag.Album;

			if (string.IsNullOrWhiteSpace(album))
			{
				//foreach (TagLib.Tag tags in tag.Tags)
				//{
				//	album = tags.Album;

				//	if (!string.IsNullOrWhiteSpace(album))
				//	{
				//		break;
				//	}
				//}
			}

			// tags are toast, attempt to get from file name
			if (string.IsNullOrWhiteSpace(album))
			{
			}

			if (!string.IsNullOrWhiteSpace(album))
			{
				string[] regexes =
					new string[] { @" \[.*?\]", @" \(Disc.*?Side\)",
						@" \(Disc.*?Res\)" };

				foreach (string regex in regexes)
				{
					if (Regex.IsMatch(album, regex))
					{
						album = Regex.Replace(album, regex, @"");
					}
				}

				if (album.EndsWith(" (Disc 2)"))
				{
					album = album.Replace(" (Disc 2)", "");
				}
			}

			return album;
		}
	}
}
