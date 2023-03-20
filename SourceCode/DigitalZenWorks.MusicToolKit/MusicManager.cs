/////////////////////////////////////////////////////////////////////////////
// <copyright file="MusicManager.cs" company="Digital Zen Works">
// Copyright © 2019 - 2023 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Common.Logging;
using CSCore.XAudio2.X3DAudio;
using DigitalZenWorks.RulesLibrary;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using TagLib.Mpeg;

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
		/// <param name="artistPath">The artist path of the file.</param>
		/// <param name="albumTag">The album tag.</param>
		/// <returns>A new combined path.</returns>
		public static string CreateAlbumPathFromTag(
			string artistPath, string albumTag)
		{
			albumTag = Paths.RemoveIllegalPathCharacters(albumTag);
			albumTag = albumTag.Trim();
			albumTag = albumTag.TrimEnd('.');

			albumTag = albumTag.Replace(
				"  ", " ", StringComparison.OrdinalIgnoreCase);

			string path = Path.Combine(artistPath, albumTag);
			CreateDirectoryIfNotExists(path);

			return path;
		}

		/// <summary>
		/// Create artist path from tag.
		/// </summary>
		/// <param name="file">The given file.</param>
		/// <param name="artistTag">The artist tag.</param>
		/// <returns>A combined file path.</returns>
		public static string CreateArtistPathFromTag(
			FileInfo file, string artistTag)
		{
			string path = null;

			if (file != null)
			{
				string basePath = Paths.GetBasePathFromFilePath(file.FullName);

				artistTag = Paths.RemoveIllegalPathCharacters(artistTag);
				artistTag = artistTag.TrimEnd('.');

				artistTag = artistTag.Replace(
					"  ", " ", StringComparison.OrdinalIgnoreCase);

				string pattern = @"\.{2,}";

				if (Regex.IsMatch(artistTag, pattern))
				{
					artistTag =
						Regex.Replace(artistTag, pattern, string.Empty);
				}

				artistTag = artistTag.Trim();

				path = Path.Combine(
					basePath, artistTag);
				CreateDirectoryIfNotExists(path);
			}

			return path;
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

						CreateDirectoryIfNotExists(newPath);

						while (pathParts.Length > depth + 2)
						{
							depth++;
							newPath += "\\" + pathParts[depth];

							CreateDirectoryIfNotExists(newPath);
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
		/// Gets the file's hash.
		/// </summary>
		/// <param name="filePath">The path of the file.</param>
		/// <returns>The item's hash encoded in base 64.</returns>
		public static string GetFileHash(string filePath)
		{
			string hashBase64 = null;

			try
			{
				if (!string.IsNullOrWhiteSpace(filePath) &&
					System.IO.File.Exists(filePath))
				{
					using FileStream fileStream =
						System.IO.File.OpenRead(filePath);

					using SHA256 hasher = SHA256.Create();

					byte[] hashValue = hasher.ComputeHash(fileStream);
					hashBase64 = Convert.ToBase64String(hashValue);
				}
			}
			catch (System.Exception exception) when
				(exception is ArgumentException ||
				exception is ArgumentNullException ||
				exception is ArgumentOutOfRangeException ||
				exception is DirectoryNotFoundException ||
				exception is FileNotFoundException ||
				exception is InvalidCastException ||
				exception is IOException ||
				exception is NotSupportedException ||
				exception is OutOfMemoryException ||
				exception is PathTooLongException ||
				exception is UnauthorizedAccessException)
			{
				Log.Error(exception.ToString());
			}

			return hashBase64;
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
		/// Update files.
		/// </summary>
		/// <remarks>When adjusting the case in the file name parts, always
		/// need to be aware and compensate for, that Windows will treat file
		/// names with differnt cases as the same.</remarks>
		/// <param name="file">The file to update.</param>
		/// <returns>The updated file.</returns>
		public FileInfo UpdateFile(FileInfo file)
		{
			if (file != null)
			{
				using MediaFileTags tags = new (file.FullName, rules);

				string path = CreateArtistPathFromTag(file, tags.Artist);

				path = CreateAlbumPathFromTag(path, tags.Album);

				string title = Paths.RemoveIllegalPathCharacters(tags.Title);
				title = RemoveTrailingNumbers(title);

				title = title.Replace(
					"  ", " ", StringComparison.OrdinalIgnoreCase);

				string filePath = path + "\\" + title + file.Extension;

				if (!filePath.Equals(
					file.FullName, StringComparison.Ordinal))
				{
					if (!System.IO.File.Exists(filePath))
					{
						System.IO.File.Move(file.FullName, filePath);
					}
					else
					{
						if (filePath.Equals(
							file.FullName, StringComparison.OrdinalIgnoreCase))
						{
							// Windows special case - The file names differ
							// only by case, so need to compensate.  Simply
							// saving with the new name won't work - Windows
							// will just ignore the case change and keep the
							// original name.
							string temporaryFilePath = filePath + ".tmp";
							System.IO.File.Move(
								file.FullName, temporaryFilePath);
							System.IO.File.Move(temporaryFilePath, filePath);
						}
						else
						{
							// a file is already there, move into duplicates
							filePath = GetDuplicateLocation(filePath);
							System.IO.File.Move(file.FullName, filePath);
						}
					}
				}

				file = new FileInfo(filePath);
			}

			return file;
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
					CreateDirectoryIfNotExists(tagsOnlyPath);

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

		private static void CreateDirectoryIfNotExists(string path)
		{
			DirectoryInfo directory = new (path);

			if (!directory.Exists)
			{
				directory.Create();
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

		private static string RemoveTrailingNumbers(string text)
		{
			text = Regex.Replace(text, @"\s+\d+$", string.Empty);

			return text;
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
