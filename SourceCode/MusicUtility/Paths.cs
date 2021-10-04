/////////////////////////////////////////////////////////////////////////////
// <copyright file="Paths.cs" company="Digital Zen Works">
// Copyright © 2019 - 2021 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicUtility
{
	/// <summary>
	/// Paths class.
	/// </summary>
	public static class Paths
	{
		/// <summary>
		/// Get album from path method.
		/// </summary>
		/// <param name="path">The full path of the file.</param>
		/// <param name="iTunesPath">The iTunes path of the path.</param>
		/// <returns>The album part of the path.</returns>
		public static string GetAlbumFromPath(string path, string iTunesPath)
		{
			Dictionary<string, string> exceptions = new ();
			exceptions.Add("10Cc", "10cc");

			string album = GetPathPart(path, iTunesPath, 2);
			album = GetTitleCase(album);

			foreach (KeyValuePair<string, string> exception in exceptions)
			{
				album = album.Replace(exception.Key, exception.Value);
			}

			return album;
		}

		/// <summary>
		/// Get artist from path method.
		/// </summary>
		/// <param name="path">The full path of the file.</param>
		/// <param name="iTunesPath">The iTunes path of the path.</param>
		/// <returns>The artist part of the path.</returns>
		public static string GetArtistFromPath(string path, string iTunesPath)
		{
			string artist = Paths.GetPathPart(path, iTunesPath, 1);

			return artist;
		}

		/// <summary>
		/// Get iTunes directory depth method.
		/// </summary>
		/// <param name="iTunesPath">The iTunes path.</param>
		/// <returns>The depth of the iTunes path.</returns>
		public static int GetItunesDirectoryDepth(string iTunesPath)
		{
			int depth = -1;

			if (!string.IsNullOrWhiteSpace(iTunesPath))
			{
				string[] iTunesPathParts =
					iTunesPath.Split(Path.DirectorySeparatorChar);
				depth = iTunesPathParts.Length;

				string lastPath = iTunesPathParts[depth - 1];
				if (string.IsNullOrEmpty(lastPath))
				{
					depth--;
				}
			}

			return depth;
		}

		/// <summary>
		/// Get path part method.
		/// </summary>
		/// <param name="path">The full path of the file.</param>
		/// <param name="iTunesPath">The iTunes path of the path.</param>
		/// <param name="index">The index of the path part.</param>
		/// <returns>The path part.</returns>
		public static string GetPathPart(
			string path, string iTunesPath, int index)
		{
			string part = string.Empty;

			if (!string.IsNullOrWhiteSpace(path))
			{
				string cleanPath = RemoveIntermediaryPath(path, iTunesPath);

				string[] pathParts =
					cleanPath.Split(Path.DirectorySeparatorChar);
				int iTunesDepth = GetItunesDirectoryDepth(iTunesPath);
				int position = iTunesDepth + index;

				if (pathParts.Length > position)
				{
					part = pathParts[position];
				}
			}

			return part;
		}

		/// <summary>
		/// Get path part from tag method.
		/// </summary>
		/// <param name="tag">The tag data to use.</param>
		/// <param name="path">The full path of the file.</param>
		/// <returns>The path part.</returns>
		public static string GetPathPartFromTag(string tag, string path)
		{
			if (!string.IsNullOrWhiteSpace(tag))
			{
				path = tag;
				char[] illegalCharactors = new char[]
				{
					'<', '>', '"', '?', '*', '\''
				};

				foreach (char charactor in illegalCharactors)
				{
					if (path.Contains(charactor))
					{
						path = path.Replace(
							charactor.ToString(CultureInfo.InvariantCulture),
							string.Empty);
					}
				}

				illegalCharactors = new char[] { ':', '/', '\\', '|' };

				foreach (char charactor in illegalCharactors)
				{
					if (path.Contains(charactor))
					{
						path = path.Replace(
							charactor.ToString(CultureInfo.InvariantCulture),
							" - ");
					}
				}

				path = path.Replace("  ", " ");
			}

			return path;
		}

		/// <summary>
		/// Get title from path method.
		/// </summary>
		/// <param name="path">The full path of the file.</param>
		/// <param name="iTunesPath">The iTunes path of the path.</param>
		/// <returns>The title part of the path.</returns>
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

			if ((depth > 4) && pathParts[6].Equals(
				"Music",
				StringComparison.OrdinalIgnoreCase))
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
