using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Threading.Tasks;

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

			album = UpdateAlbumTag(musicFile.Tag.Album);

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

			year = musicFile.Tag.Year;

			if (true == updated)
			{
				musicFile.Save();
			}

			musicFile.Dispose();
		}

		public bool UpdateFileTags()
		{
			bool updated = false;

			return updated;
		}

		private static string UpdateAlbumTag(string album)
		{
			string newAlbum = album;
			string regex = @" \[.*?\]";

			if (album.EndsWith(" (Disc 2)"))
			{
				newAlbum = album.Replace(" (Disc 2)", "");
			}

			if (Regex.IsMatch(newAlbum, regex))
			{
				newAlbum = Regex.Replace(album, regex, @"");
			}

			return newAlbum;
		}
	}
}
