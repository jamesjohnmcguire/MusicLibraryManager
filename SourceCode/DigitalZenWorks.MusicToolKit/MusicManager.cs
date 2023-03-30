/////////////////////////////////////////////////////////////////////////////
// <copyright file="MusicManager.cs" company="Digital Zen Works">
// Copyright © 2019 - 2023 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Common.Logging;
using CSCore.XAudio2.X3DAudio;
using DigitalZenWorks.Common.Utilities;
using DigitalZenWorks.RulesLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

[assembly: CLSCompliant(false)]

namespace DigitalZenWorks.MusicToolKit
{
	/// <summary>
	/// Music manager class.
	/// </summary>
	public class MusicManager : IDisposable
	{
		private static readonly ILog Log = LogManager.GetLogger(
			MethodBase.GetCurrentMethod().DeclaringType);

		private readonly string libraryLocation;
		private readonly string libraryTagsOnlyDirectoryLocation;

		private ITunesManager iTunesManager;
		private Rules rules;

		/// <summary>
		/// Initializes a new instance of the <see cref="MusicManager"/> class.
		/// </summary>
		/// <param name="enableItunes">Indicates whether to instanciate
		/// the iTunes Application.</param>
		public MusicManager(bool enableItunes)
		{
			string applicationDataDirectory = @"\DigitalZenWorks\MusicManager";
			string baseDataDirectory = Environment.GetFolderPath(
				Environment.SpecialFolder.ApplicationData,
				Environment.SpecialFolderOption.Create);
			libraryLocation = baseDataDirectory + applicationDataDirectory;

			iTunesManager = new ITunesManager(enableItunes);

			if (iTunesManager.IsItunesEnabled == true)
			{
				libraryLocation = iTunesManager.ItunesLibraryLocation;
				libraryLocation = libraryLocation.Trim('\\');
			}

			libraryTagsOnlyDirectoryLocation = libraryLocation + " Tags Only";

			GetDefaultRules();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MusicManager"/> class.
		/// </summary>
		/// <param name="rules">The rules to use.</param>
		/// <param name="enableItunes">Indicates whether to instanciate
		/// the iTunes Application.</param>
		public MusicManager(Rules rules, bool enableItunes)
			: this(enableItunes)
		{
			if ((rules != null) && (rules.RulesList != null) &&
				(rules.RulesList.Count > 0))
			{
				this.rules = rules;
			}
		}

		/// <summary>
		/// Gets the library location.
		/// </summary>
		/// <value>The library location.</value>
		public string LibraryLocation
		{
			get { return libraryLocation; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether the update tags property.
		/// </summary>
		public bool UpdateTags { get; set; }

		/// <summary>
		/// Gets or sets the rules.
		/// </summary>
		/// <value>The rules.</value>
		public Rules Rules
		{
			get { return rules; }
			set { rules = value; }
		}

		/// <summary>
		/// Create album path from tag.
		/// </summary>
		/// <param name="album">The album tag.</param>
		/// <returns>A new combined path.</returns>
		public static string CleanAlbum(string album)
		{
			album = Paths.RemoveIllegalPathCharacters(album);
			album = album.Trim();
			album = album.TrimEnd('.');

			album =
				album.Replace("  ", " ", StringComparison.OrdinalIgnoreCase);

			return album;
		}

		/// <summary>
		/// Create artist path from tag.
		/// </summary>
		/// <param name="artist">The artist name.</param>
		/// <returns>The cleaned artist name.</returns>
		public static string CleanArtist(string artist)
		{
			if (!string.IsNullOrWhiteSpace(artist))
			{
				artist = Paths.RemoveIllegalPathCharacters(artist);
				artist = artist.TrimEnd('.');

				artist = artist.Replace(
					"  ", " ", StringComparison.OrdinalIgnoreCase);

				string pattern = @"\.{2,}";

				if (Regex.IsMatch(artist, pattern))
				{
					artist = Regex.Replace(artist, pattern, string.Empty);
				}

				artist = artist.Trim();
			}

			return artist;
		}

		/// <summary>
		/// Get duplicate location.
		/// </summary>
		/// <param name="path">The path of the duplicate item.</param>
		/// <returns>A new path for the duplicate item.</returns>
		public static string GetDuplicateLocation(string path)
		{
			string destinationPath = null;

			if (!string.IsNullOrWhiteSpace(path))
			{
				bool locationOk = false;
				int tries = 2;

				destinationPath = path;
				string[] pathParts =
					path.Split(Path.DirectorySeparatorChar);

				while (false == locationOk)
				{
					string basePath = Paths.GetBasePathFromFilePath(path);
					string newBasePath = basePath +
						tries.ToString(CultureInfo.InvariantCulture);
					FileInfo fileInfo = new (newBasePath);
					string baseName = fileInfo.Name;

					int depth = pathParts.Length - 4;

					if (depth > 4)
					{
						pathParts[depth] = baseName;

						List<string> newList = new (pathParts);
						while (newList.Count > depth + 1)
						{
							newList.RemoveAt(newList.Count - 1);
						}

						string[] newParts = newList.ToArray();
						string newPath = string.Join("\\", newParts);

						Directory.CreateDirectory(newPath);

						while (pathParts.Length > depth + 2)
						{
							depth++;
							newPath += "\\" + pathParts[depth];

							Directory.CreateDirectory(newPath);
						}

						destinationPath = newPath + "\\" + pathParts[^1];

						if (!System.IO.File.Exists(destinationPath))
						{
							locationOk = true;
						}
					}

					tries++;
				}
			}

			return destinationPath;
		}

		/// <summary>
		/// Normalize path.
		/// </summary>
		/// <param name="file">The file to check.</param>
		/// <returns>The normalized path.</returns>
		public static string NormalizePath(FileInfo file)
		{
			string filePath = null;

			if (file != null)
			{
				string basePath = Paths.GetBasePathFromFilePath(file.FullName);
				string artist = Paths.GetArtistFromPath(file.FullName);
				string album = Paths.GetAlbumFromPath(file.FullName);
				string title = Paths.GetTitleFromPath(file.FullName);

				if (file.Exists == true)
				{
					using MediaFileTags tags = new (file.FullName);

					artist = tags.Artist;
					album = tags.Album;
					title = tags.Title;
				}

				artist = CleanArtist(artist);
				album = CleanAlbum(album);

				title = TitleRules.ApplyTitleFileRules(title, artist, true);

				filePath = string.Format(
					CultureInfo.InvariantCulture,
					@"{1}{0}{2}{0}{3}{0}{4}{5}",
					Path.DirectorySeparatorChar,
					basePath,
					artist,
					album,
					title,
					file.Extension);
			}

			return filePath;
		}

		/// <summary>
		/// Update files.
		/// </summary>
		/// <remarks>When adjusting the case in the file name parts, always
		/// need to be aware and compensate for, that Windows will treat file
		/// names with differnt cases as the same.</remarks>
		/// <param name="file">The file to update.</param>
		/// <returns>The updated file.</returns>
		public static FileInfo UpdateFile(FileInfo file)
		{
			if (file != null)
			{
				string filePath = NormalizePath(file);

				// File path has changed
				if (!filePath.Equals(file.FullName, StringComparison.Ordinal))
				{
					// If no file existing with that name, just move it
					if (!System.IO.File.Exists(filePath))
					{
						string directory = Path.GetDirectoryName(filePath);
						Directory.CreateDirectory(directory);

						System.IO.File.Move(file.FullName, filePath);
					}
					else
					{
						string existingFile = filePath;

						// There is already a file there with that name...
						if (existingFile.Equals(
							file.FullName, StringComparison.OrdinalIgnoreCase))
						{
							// Windows special case - The file names differ
							// only by case, so need to compensate.  Simply
							// saving with the new name won't work - Windows
							// will just ignore the case change and keep the
							// original name.
							string temporaryFilePath = existingFile + ".tmp";
							File.Move(file.FullName, temporaryFilePath);
							File.Move(temporaryFilePath, existingFile);
						}
						else
						{
							bool areExactDuplicates =
								FileUtils.AreFilesTheSame(
									existingFile, file.FullName);

							if (areExactDuplicates == true)
							{
								File.Delete(file.FullName);
							}
							else
							{
								// move into duplicates
								filePath = GetDuplicateLocation(existingFile);
								File.Move(file.FullName, filePath);
							}
						}
					}
				}

				file = new FileInfo(filePath);
			}

			return file;
		}

		/// <summary>
		/// Clean music library method.
		/// </summary>
		/// <returns>A value indicating success or not.</returns>
		public int CleanMusicLibrary()
		{
			// Operate on the actual music files in the file system
			CleanFiles(libraryLocation);

			if (iTunesManager.IsItunesEnabled == true)
			{
				// Operate on the iTunes data store
				iTunesManager.DeleteDeadTracks();
			}

			return 0;
		}

		/// <summary>
		/// Dispose method.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Save tags to json file method.
		/// </summary>
		/// <param name="sourceFile">The source file.</param>
		/// <param name="destinationPath">The destination path.</param>
		/// <returns>The file path of the saved file.</returns>
		public string SaveTagsToJsonFile(
			FileInfo sourceFile, string destinationPath)
		{
			string destinationFile = null;

			try
			{
				if (sourceFile != null)
				{
					using MediaFileTags tags =
						new (sourceFile.FullName, rules);

					SortedDictionary<string, object> tagSet = tags.GetTags();

					JsonSerializerSettings jsonSettings = new ();
					jsonSettings.NullValueHandling = NullValueHandling.Ignore;
					jsonSettings.ContractResolver =
						new OrderedContractResolver();

					string json = JsonConvert.SerializeObject(
						tagSet, Formatting.Indented, jsonSettings);

					if (!string.IsNullOrWhiteSpace(json))
					{
						destinationFile =
							destinationPath + "\\" + sourceFile.Name + ".json";

						System.IO.File.WriteAllText(destinationFile, json);
					}

					Log.Info("Tags Saved to: " + destinationFile);
				}
			}
			catch (Exception exception) when
				(exception is ArgumentException ||
				exception is TagLib.CorruptFileException ||
				exception is TagLib.UnsupportedFormatException)
			{
				Log.Error(exception.ToString());
				Log.Error("File is: " + sourceFile);
			}

			return destinationFile;
		}

		/// <summary>
		/// Update library tags only method.
		/// </summary>
		public void UpdateLibraryTagsOnly()
		{
			UpdateLibraryTagsOnly(
				libraryLocation, libraryTagsOnlyDirectoryLocation);
		}

		/// <summary>
		/// Update library tags only method.
		/// </summary>
		/// <param name="path">The path of the source files.</param>
		/// <param name="tagsOnlyPath">The path of tags only files.</param>
		public void UpdateLibraryTagsOnly(string path, string tagsOnlyPath)
		{
			try
			{
				string[] excludes =
				{
					".crd", ".cue", ".doc", ".gif", ".gz", ".htm", ".ini",
					".jpeg", ".jpg", ".lit", ".log", ".m3u", ".nfo", ".opf",
					".pdf", ".plist", ".png", ".psp", ".sav", ".sfv", ".txt",
					".url", ".xls", ".zip"
				};

				string[] includes =
				{
					".AIFC", ".FLAC", ".M4A", ".MP3", ".WAV", ".WMA"
				};

				if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
				{
					Directory.CreateDirectory(tagsOnlyPath);

					DirectoryInfo directory = new (path);

					FileInfo[] files = directory.GetFiles();

					foreach (FileInfo file in files)
					{
						if (includes.Contains(
							file.Extension.ToUpperInvariant()))
						{
							SaveTagsToJsonFile(file, tagsOnlyPath);
						}
					}

					string[] directories = Directory.GetDirectories(path);

					foreach (string subDirectory in directories)
					{
						DirectoryInfo subDirectoryInfo = new (subDirectory);
						string nextTagsOnlyPath =
							tagsOnlyPath + "\\" + subDirectoryInfo.Name;

						UpdateLibraryTagsOnly(subDirectory, nextTagsOnlyPath);
					}

					if (!string.IsNullOrWhiteSpace(tagsOnlyPath))
					{
						DeleteEmptyDirectory(tagsOnlyPath);
					}
				}
			}
			catch (Exception exception) when
				(exception is ArgumentException ||
				exception is ArgumentNullException ||
				exception is TagLib.CorruptFileException ||
				exception is DirectoryNotFoundException ||
				exception is FileNotFoundException ||
				exception is IndexOutOfRangeException ||
				exception is InvalidOperationException ||
				exception is NotSupportedException ||
				exception is NullReferenceException ||
				exception is IOException ||
				exception is PathTooLongException ||
				exception is System.Security.SecurityException ||
				exception is TargetException ||
				exception is UnauthorizedAccessException ||
				exception is TagLib.UnsupportedFormatException)
			{
				Log.Error(exception.ToString());
			}
		}

		/// <summary>
		/// Dispose method.
		/// </summary>
		/// <param name="disposing">Indicates whether currently disposing
		/// or not.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				// dispose managed resources
				if (iTunesManager != null)
				{
					iTunesManager.Dispose();
					iTunesManager = null;
				}
			}
		}

		private static bool DeleteEmptyDirectory(string path)
		{
			bool deleted = false;

			DirectoryInfo directory = new (path);
			string[] directories = Directory.GetDirectories(path);
			FileInfo[] files = directory.GetFiles();

			if ((files.Length == 0) && (directories.Length == 0) &&
				(!path.Contains(
					"Automatically Add to iTunes",
					StringComparison.OrdinalIgnoreCase)))
			{
				Directory.Delete(path, false);
				deleted = true;
			}

			return deleted;
		}

		private Rules GetDefaultRules()
		{
			string contents = null;

			string resourceName =
				"DigitalZenWorks.MusicToolKit.DefaultRules.json";
			Assembly thisAssembly = Assembly.GetCallingAssembly();

			using (Stream templateObjectStream =
				thisAssembly.GetManifestResourceStream(resourceName))
			{
				if (templateObjectStream != null)
				{
					using StreamReader reader = new (templateObjectStream);
					contents = reader.ReadToEnd();
				}
			}

			rules = new Rules(contents);

			return rules;
		}

		private void CleanFile(FileInfo file)
		{
			try
			{
				string message = "Checking: " + file.FullName;
				Log.Info(message);

				if (UpdateTags == true)
				{
					// get and update tags
					using MediaFileTags tags = new (file.FullName, rules);
					tags.Clean();
				}

				// update directory and file names
				file = UpdateFile(file);

				if (iTunesManager.IsItunesEnabled == true)
				{
					iTunesManager.UpdateItunes(file);
				}
			}
			catch (Exception exception) when
				(exception is ArgumentException ||
				exception is ArgumentNullException ||
				exception is TagLib.CorruptFileException ||
				exception is DirectoryNotFoundException ||
				exception is FileNotFoundException ||
				exception is IOException ||
				exception is IndexOutOfRangeException ||
				exception is InvalidOperationException ||
				exception is NullReferenceException ||
				exception is PathTooLongException ||
				exception is System.Security.SecurityException ||
				exception is TargetException ||
				exception is UnauthorizedAccessException ||
				exception is TagLib.UnsupportedFormatException)
			{
				Log.Error(exception.ToString());
			}
		}

		private void CleanFiles(string path)
		{
			try
			{
				string[] includes =
				{
					".AIFC", ".FLAC", ".M4A", ".MP3", ".WAV", ".WMA"
				};

				if (Directory.Exists(path))
				{
					DirectoryInfo directory = new (path);

					FileInfo[] files = directory.GetFiles();

					foreach (FileInfo file in files)
					{
						if (includes.Contains(
							file.Extension.ToUpperInvariant()))
						{
							CleanFile(file);
						}
					}

					string[] directories = Directory.GetDirectories(path);

					foreach (string subDirectory in directories)
					{
						CleanFiles(subDirectory);
					}

					DeleteEmptyDirectory(path);
				}
			}
			catch (Exception exception) when
				(exception is ArgumentException ||
				exception is ArgumentNullException ||
				exception is DirectoryNotFoundException ||
				exception is FileNotFoundException ||
				exception is IndexOutOfRangeException ||
				exception is InvalidOperationException ||
				exception is NullReferenceException ||
				exception is IOException ||
				exception is PathTooLongException ||
				exception is System.Security.SecurityException ||
				exception is TargetException ||
				exception is UnauthorizedAccessException)
			{
				Log.Error(exception.ToString());
			}
		}
	}
}
