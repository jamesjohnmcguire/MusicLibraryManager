using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicUtility
{
	class Paths
	{
		public static string GetAlbumFromPath(string path, string iTunesPath)
		{
			string album = GetPathPart(path, iTunesPath, 2);
			album = GetTitleCase(album);

			return album;
		}

		public static string GetArtistFromPath(string path, string iTunesPath)
		{
			string artist = Paths.GetPathPart(path, iTunesPath, 1);

			return artist;
		}

		public static int GetItunesDirectoryDepth(string iTunesPath)
		{
			string[] iTunesPathParts =
				iTunesPath.Split(Path.DirectorySeparatorChar);
			int depth = iTunesPathParts.Length;

			string lastPath = iTunesPathParts[depth - 1];
			if (string.IsNullOrEmpty(lastPath))
			{
				depth--;
			}

			return depth;
		}

		public static string GetPathPart(
			string path, string iTunesPath, int index)
		{
			string part = string.Empty;

			string cleanPath = RemoveIntermediaryPath(path, iTunesPath);

			string[] pathParts =
				cleanPath.Split(Path.DirectorySeparatorChar);
			int iTunesDepth = GetItunesDirectoryDepth(iTunesPath);
			int position = iTunesDepth + index;

			if (pathParts.Length > position)
			{
				part = pathParts[position];
			}

			return part;
		}

		public static string GetPathPartFromTag(string tag, string path)
		{
			if (!string.IsNullOrWhiteSpace(tag))
			{
				path = tag;
				char[] illegalCharactors = new char[]
					{ '<', '>', '"', '?', '*', '\'' };

				foreach (char charactor in illegalCharactors)
				{
					if (path.Contains(charactor))
					{
						path =
							path.Replace(charactor.ToString(), string.Empty);
					}
				}

				illegalCharactors = new char[] { ':', '/', '\\', '|' };

				foreach (char charactor in illegalCharactors)
				{
					if (path.Contains(charactor))
					{
						path = path.Replace(charactor.ToString(), " - ");
					}
				}

				path = path.Replace("  ", " ");
			}

			return path;
		}

		public static string GetTitleFromPath(string path, string iTunesPath)
		{
			string title = GetPathPart(path, iTunesPath, 3);
			title = GetTitleCase(title);

			return title;
		}

		private static string GetTitleCase(string title)
		{
			CultureInfo cultureInfo = CultureInfo.CurrentCulture;
			TextInfo textInfo = cultureInfo.TextInfo;
			title = textInfo.ToTitleCase(title);

			return title;
		}

		private static string RemoveIntermediaryPath(
			string path, string iTunesPath)
		{
			string newPath = path;

			string[] pathParts =
				path.Split(Path.DirectorySeparatorChar);
			int iTunesDepth = GetItunesDirectoryDepth(iTunesPath);
			int depth = pathParts.Length - iTunesDepth;

			if ((depth > 4) && pathParts[6].Equals("Music"))
			{
				// there is an extra intermediary directory, remove it
				List<string> list = new List<string>(pathParts);
				list.RemoveAt(7);

				pathParts = list.ToArray();
				newPath = string.Join("\\", pathParts);
			}

			return newPath;
		}
	}
}
