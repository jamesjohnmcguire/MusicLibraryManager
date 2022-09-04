/////////////////////////////////////////////////////////////////////////////
// <copyright file="Paths.cs" company="Digital Zen Works">
// Copyright © 2019 - 2022 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalZenWorks.MusicToolKit
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
		/// <returns>The album part of the path.</returns>
		public static string GetAlbumFromPath(string path)
		{
			string album = null;

			if (!string.IsNullOrWhiteSpace(path))
			{
				int depth = GetDirectoryCount(path);

				if (depth > 2)
				{
					// Assuming file has path structure ending with
					// artist/album/song.
					string[] pathParts =
						path.Split(Path.DirectorySeparatorChar);

					int albumSegment = pathParts.Length - 2;
					album = pathParts[albumSegment];
				}
			}

			if (!string.IsNullOrWhiteSpace(album))
			{
				Dictionary<string, string> exceptions = new();
				exceptions.Add("10Cc", "10cc");

				album = GetTitleCase(album);

				foreach (KeyValuePair<string, string> exception in exceptions)
				{
					album = album.Replace(
						exception.Key,
						exception.Value,
						StringComparison.OrdinalIgnoreCase);
				}
			}

			return album;
		}

		/// <summary>
		/// Get artist from path method.
		/// </summary>
		/// <param name="path">The full path of the file.</param>
		/// <returns>The artist part of the path.</returns>
		public static string GetArtistFromPath(string path)
		{
			string artist = null;

			if (!string.IsNullOrWhiteSpace(path))
			{
				int depth = GetDirectoryCount(path);

				if (depth > 3)
				{
					// Assuming file has path structure ending with
					// artist/album/song.
					string[] pathParts =
						path.Split(Path.DirectorySeparatorChar);

					int artistSegment = pathParts.Length - 3;
					artist = pathParts[artistSegment];
				}
			}

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
					if (path.Contains(
						charactor, StringComparison.OrdinalIgnoreCase))
					{
						path = path.Replace(
							charactor.ToString(CultureInfo.InvariantCulture),
							string.Empty,
							StringComparison.OrdinalIgnoreCase);
					}
				}

				illegalCharactors = new char[] { ':', '/', '\\', '|' };

				foreach (char charactor in illegalCharactors)
				{
					if (path.Contains(
						charactor, StringComparison.OrdinalIgnoreCase))
					{
						path = path.Replace(
							charactor.ToString(CultureInfo.InvariantCulture),
							" - ",
							StringComparison.OrdinalIgnoreCase);
					}
				}

				path = path.Replace(
					"  ", " ", StringComparison.OrdinalIgnoreCase);
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

		private static int GetDirectoryCount(string fileName)
		{
			string[] parts = fileName.Split(Path.DirectorySeparatorChar);
			int depth = parts.Length;

			return depth;
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
