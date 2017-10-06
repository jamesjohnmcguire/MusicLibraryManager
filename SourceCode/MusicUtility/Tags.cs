using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicUtility
{
	public class Tags
	{
		private string album = null;
		private string artist = null;
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
			TagLib.File musicFile = TagLib.File.Create(file);

			album = musicFile.Tag.Album;
			string[] artists = musicFile.Tag.AlbumArtists;

			if (artists.Length > 0)
			{
				artist = artists[0];
			}

			title = musicFile.Tag.Title;
			year = musicFile.Tag.Year;
		}
	}
}
