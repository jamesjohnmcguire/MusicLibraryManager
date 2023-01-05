﻿/////////////////////////////////////////////////////////////////////////////
// <copyright file="Paths.cs" company="Digital Zen Works">
// Copyright © 2019 - 2023 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

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
			string album = GetPartFromPath(path, 2);

			return album;
		}

		/// <summary>
		/// Get artist from path method.
		/// </summary>
		/// <param name="path">The full path of the file.</param>
		/// <returns>The artist part of the path.</returns>
		public static string GetArtistFromPath(string path)
		{
			string artist = GetPartFromPath(path, 3);

			return artist;
		}

		/// <summary>
		/// Get the artist path from the file path method.
		/// </summary>
		/// <param name="path">The full path of the file.</param>
		/// <returns>The artist part of the path.</returns>
		public static string GetArtistPathFromFilePath(string path)
		{
			string artistPath = null;

			string artist = GetArtistFromPath(path);

			if (!string.IsNullOrWhiteSpace(path))
			{
				for (int index = 0; index < 3; index++)
				{
					path = Path.GetDirectoryName(path);
				}

				artistPath = path + @"\" + artist;
			}

			return artistPath;
		}

		/// <summary>
		/// Get the base path from the file path method.
		/// </summary>
		/// <remarks>This assumes the file path ends with the format of:
		/// Artist\Album\Song.ext.</remarks>
		/// <param name="path">The full path of the file.</param>
		/// <returns>The base part of the path.</returns>
		public static string GetBasePathFromFilePath(string path)
		{
			string basePath = null;

			if (!string.IsNullOrWhiteSpace(path))
			{
				int depth = GetDirectoryCount(path);

				if (depth > 3)
				{
					basePath = path;

					for (int index = 0; index < 3; index++)
					{
						basePath = Path.GetDirectoryName(basePath);
					}
				}
			}

			return basePath;
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
		/// <param name="path">The full path of the file.</param>
		/// <returns>The path part.</returns>
		public static string RemoveIllegalPathCharacters(string path)
		{
			if (!string.IsNullOrWhiteSpace(path))
			{
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
			}

			return path;
		}

		/// <summary>
		/// Get title from path method.
		/// </summary>
		/// <param name="path">The full path of the file.</param>
		/// <returns>The title part of the path.</returns>
		public static string GetTitleFromPath(string path)
		{
			string title = null;

			if (!string.IsNullOrWhiteSpace(path))
			{
				int depth = GetDirectoryCount(path);

				if (depth > 0)
				{
					// Assuming the last part of the path structure has the
					// song title.
					string[] pathParts =
						path.Split(Path.DirectorySeparatorChar);

					int titleSegment = pathParts.Length - 1;
					title = pathParts[titleSegment];
					title = Path.GetFileNameWithoutExtension(title);
				}
			}

			title = TagRules.GetTitleCase(title);

			return title;
		}

		private static int GetDirectoryCount(string fileName)
		{
			string[] parts = fileName.Split(Path.DirectorySeparatorChar);
			int depth = parts.Length;

			return depth;
		}

		private static string GetPartFromPath(string path, int partDepth)
		{
			string part = null;

			if (!string.IsNullOrWhiteSpace(path))
			{
				int depth = GetDirectoryCount(path);

				if (depth > partDepth)
				{
					// Assuming file has path structure ending with
					// artist/album/song.
					string[] pathParts =
						path.Split(Path.DirectorySeparatorChar);

					int segment = pathParts.Length - partDepth;
					part = pathParts[segment];
				}
			}

			return part;
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
				List<string> list = new (pathParts);
				list.RemoveAt(7);

				pathParts = list.ToArray();
				newPath = string.Join("\\", pathParts);
			}

			return newPath;
		}
	}
}
