/////////////////////////////////////////////////////////////////////////////
// <copyright file="Paths.cs" company="Digital Zen Works">
// Copyright © 2019 - 2025 Digital Zen Works. All Rights Reserved.
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
		/// Checks if the path contain a sym link to itunes.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns>A value that indicates whether the path contains a
		/// sym link to iTunes or not.</returns>
		public static bool DoesPathContainItunesSymLink(string filePath)
		{
			bool iTunesSymLink = false;

			if (!string.IsNullOrWhiteSpace(filePath))
			{
				string basePath = GetLibraryFromPath(filePath);
				FileInfo basePathInfo = new (basePath);

#if NET6_0_OR_GREATER
				string target = basePathInfo.LinkTarget;
#else
				string target = null;

				bool reparsePoint = 
					basePathInfo.Attributes.HasFlag(
					FileAttributes.ReparsePoint);
#endif

				if (target != null)
				{
					iTunesSymLink = true;
				}
			}

			return iTunesSymLink;
		}

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
		/// Get album from path method.
		/// </summary>
		/// <param name="libraryPath">The full path of the
		/// base library.</param>
		/// <param name="path">The full path of the file.</param>
		/// <returns>The album part of the path.</returns>
		public static string GetAlbumFromPath(string libraryPath, string path)
		{
			string album = GetPartFromPath(libraryPath, path, 2);

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
		/// Get artist from path method.
		/// </summary>
		/// <param name="libraryPath">The full path of the
		/// base library.</param>
		/// <param name="path">The full path of the file.</param>
		/// <returns>The artist part of the path.</returns>
		public static string GetArtistFromPath(string libraryPath, string path)
		{
			string artist = GetPartFromPath(libraryPath, path, 3);

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
			string basePath = GetBaseSubPathFromFilePath(path, 3);

			return basePath;
		}

		/// <summary>
		/// Get the base path from the file path method.
		/// </summary>
		/// <remarks>This assumes the file path ends with the format of:
		/// Artist\Album\Song.ext.</remarks>
		/// <param name="path">The full path of the file.</param>
		/// <param name="depth">The depth of the path segments.</param>
		/// <returns>The base part of the path.</returns>
		public static string GetBaseSubPathFromFilePath(string path, int depth)
		{
			string basePath = null;

			if (!string.IsNullOrWhiteSpace(path))
			{
				int totalDepth = GetDirectoryCount(path);

				if (totalDepth > depth)
				{
					basePath = path;

					for (int index = 0; index < depth; index++)
					{
						basePath = Path.GetDirectoryName(basePath);
					}
				}
			}

			return basePath;
		}

		/// <summary>
		/// Get the base path from the file path method.
		/// </summary>
		/// <remarks>This assumes the file path ends with the format of:
		/// Artist\Album\Song.ext.</remarks>
		/// <param name="path">The full path of the file.</param>
		/// <param name="depth">The depth of the path segments.</param>
		/// <returns>The base part of the path.</returns>
		public static string GetBaseSubPathFromFilePathRightSide(
			string path, int depth)
		{
			string basePath = null;

			if (!string.IsNullOrWhiteSpace(path))
			{
				int totalDepth = GetDirectoryCount(path);

				if (totalDepth > depth)
				{
					int segments = totalDepth - depth;

					string[] pathParts =
						path.Split(Path.DirectorySeparatorChar);

					for (int index = segments; index < totalDepth; index++)
					{
						basePath += @"\" + pathParts[index];
					}
				}
				else
				{
					basePath = path;
				}
			}

			return basePath;
		}

		/// <summary>
		/// Get directory depth count.
		/// </summary>
		/// <param name="fileName">The file path to check.</param>
		/// <returns>The count of directory levels.</returns>
		public static int GetDirectoryCount(string fileName)
		{
			int depth = -1;

			if (!string.IsNullOrWhiteSpace(fileName))
			{
				string[] parts = fileName.Split(Path.DirectorySeparatorChar);
				depth = parts.Length;
			}

			return depth;
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
		/// Get library from path method.
		/// </summary>
		/// <param name="path">The full path of the file.</param>
		/// <returns>The library part of the path.</returns>
		public static string GetLibraryFromPath(string path)
		{
			string library = GetBaseSubPathFromFilePath(path, 4);

			return library;
		}

		/// <summary>
		/// Get path segment from path based on depth.
		/// </summary>
		/// <param name="libraryPath">The library path.</param>
		/// <param name="path">The path to check.</param>
		/// <param name="partDepth">The depth of the segment to check.</param>
		/// <returns>The path segment or null upon failure.</returns>
		public static string GetPartFromPath(
			string libraryPath, string path, int partDepth)
		{
			string part = null;

			if (!string.IsNullOrWhiteSpace(path))
			{
				int libraryPathDepth = GetDirectoryCount(libraryPath);

				int depth = GetDirectoryCount(path);
				depth -= libraryPathDepth;

				if (depth >= partDepth)
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

			return title;
		}

		/// <summary>
		/// Get title from path method.
		/// </summary>
		/// <param name="libraryPath">The full path of the
		/// base library.</param>
		/// <param name="path">The full path of the file.</param>
		/// <returns>The title part of the path.</returns>
		public static string GetTitleFromPath(string libraryPath, string path)
		{
			string title = GetPartFromPath(libraryPath, path, 1);
			title = Path.GetFileNameWithoutExtension(title);

			return title;
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
				char[] illegalCharactors =
				[
					'<', '>', '"', '?', '*'
				];

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

				illegalCharactors = [':', '/', '\\', '|'];

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
		/// Replaces the path with the specified library location.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <param name="libraryPath">The library path.</param>
		/// <returns>The updated path.</returns>
		public static string ReplaceLibraryPath(
			string filePath, string libraryPath)
		{
			if (!string.IsNullOrWhiteSpace(libraryPath))
			{
				libraryPath = libraryPath.TrimEnd(Path.DirectorySeparatorChar);
			}

			string rightSidePath = GetBaseSubPathFromFilePathRightSide(
				filePath, 4);
			string newPath = libraryPath + rightSidePath;

			return newPath;
		}

		private static string GetPartFromPath(string path, int partDepth)
		{
			string part = GetPartFromPath(null, path, partDepth);

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

				pathParts = [.. list];
				newPath = string.Join("\\", pathParts);
			}

			return newPath;
		}
	}
}
