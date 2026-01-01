/////////////////////////////////////////////////////////////////////////////
// <copyright file="MusicManager.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

[assembly: System.CLSCompliant(false)]

namespace DigitalZenWorks.MusicToolKit
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.IO;
	using System.Linq;
	using System.Reflection;
	using System.Text.RegularExpressions;
	using DigitalZenWorks.Common.Utilities;
	using DigitalZenWorks.RulesLibrary;
	using global::Common.Logging;
	using Newtonsoft.Json;

	/// <summary>
	/// Music manager class.
	/// </summary>
	public class MusicManager : IDisposable
	{
		private static readonly ILog Log = LogManager.GetLogger(
			MethodBase.GetCurrentMethod().DeclaringType);

		private readonly string libraryTagsOnlyDirectoryLocation;

		// Total Possible List
		// "3GP", "8SVX", "AA", "AAC", "AAX", "ACT", "AIFF", "ALAC", "AMR",
		// "APE", "AU", "AWB", "CDA", "DSS", "DVF", "FLAC", "GSM", "IKLAX",
		// "IVS", "M4A", "M4B", "M4P", "MMF", "MOGG", "MOVPKG", "MP3", "MPC",
		// "MSV", "NMF", "OGA", "OGG", "OPUS", "RA", "RAW", "RF64", "RM",
		// "SLN", "TTA", "VOC", "VOX", "WAV", "WEBM", "WMA", "WV"
		private readonly string[] audioFileExtensions =
		[
			".AIFC", ".ALAC", ".FLAC", ".M4A", ".MP3", ".WAV", "WEBM", ".WMA"
		];

		private ITunesManager iTunesManager;
		private string libraryLocation;
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

			bool iTunesExist = ITunesManager.DoesItunesExist();

			if (iTunesExist == true && enableItunes == true)
			{
				iTunesManager = new ITunesManager();

				if (iTunesManager.IsItunesEnabled == true)
				{
					libraryLocation = iTunesManager.ItunesLibraryLocation;
					libraryLocation = libraryLocation.Trim('\\');
				}

				libraryTagsOnlyDirectoryLocation = libraryLocation + " Tags Only";
			}

			rules = GetDefaultRules();
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
		/// Gets or sets the library location.
		/// </summary>
		/// <value>The library location.</value>
		public string LibraryLocation
		{
			get => libraryLocation;
			set => libraryLocation = value;
		}

		/// <summary>
		/// Gets or sets a value indicating whether the update tags property.
		/// </summary>
		/// <value>A value indicating whether the update tags property.</value>
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
		/// Get default rules.
		/// </summary>
		/// <returns>A new rules object.</returns>
		public static Rules GetDefaultRules()
		{
			string contents = null;

			string resourceName =
				"DigitalZenWorks.MusicToolKit.DefaultRules.json";
			Assembly thisAssembly = Assembly.GetExecutingAssembly();

			using (Stream templateObjectStream =
				thisAssembly.GetManifestResourceStream(resourceName))
			{
				if (templateObjectStream != null)
				{
					using StreamReader reader = new (templateObjectStream);
					contents = reader.ReadToEnd();
				}
			}

			Rules rules = new (contents);

			return rules;
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

				while (locationOk == false)
				{
					string newBasePath = GetDuplicateLocationByNumber(
						path, tries);

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

						string[] newParts = [.. newList];
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
		/// Recollect duplicates.
		/// </summary>
		/// <param name="libraryPath">The library path.</param>
		public static void RecollectDuplicates(string libraryPath)
		{
			if (!string.IsNullOrWhiteSpace(libraryPath))
			{
				bool locationOk;
				int duplicateNumer = 2;

				do
				{
					string duplicatePath = GetDuplicateLocationByNumber(
						libraryPath, duplicateNumer);

					locationOk = Directory.Exists(duplicatePath);

					if (locationOk == true)
					{
						// recurse into directories

						// for each file
						// create file path to the original
						// check to see if exists
						// if not, move
					}
				}
				while (locationOk == true);
			}
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
				iTunesManager.DeleteEmptyTracks();
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
		/// Normalize path.
		/// </summary>
		/// <param name="filePath">The file path to check.</param>
		/// <param name="useDefaultLibraryPath">Indicates whether to use the
		/// default media library path or not.</param>
		/// <param name="useTags">Indicates whether use the file tags in
		/// determining the file location.</param>
		/// <returns>The normalized path.</returns>
		public string NormalizePath(
			string filePath,
			bool useDefaultLibraryPath = true,
			bool useTags = true)
		{
			if (!string.IsNullOrWhiteSpace(filePath))
			{
				string musicPath =
					LibraryLocation + Path.DirectorySeparatorChar + "Music";

				string basePath = Paths.GetBasePathFromFilePath(filePath);

				bool isStandard = IsStandardLibraryDirectory(filePath);

				bool pathPartsCheck =
					ArePathPartsIncluded(musicPath, filePath);

				if ((useDefaultLibraryPath == true && isStandard == false) ||
					pathPartsCheck == false)
				{
					basePath = musicPath;
				}

				string artist =
					GetArtistPathSegment(musicPath, filePath, null, null);
				string album =
					GetAlbumPathSegment(musicPath, filePath, artist, null);
				string title =
					GetTitlePathSegment(musicPath, filePath, artist, null);

				if (useTags == true)
				{
					try
					{
						using MediaFileTags tags = new (filePath);

						artist = GetArtistPathSegment(
							musicPath, filePath, null, tags);
						album = GetAlbumPathSegment(
							musicPath, filePath, artist, tags);
						title = GetTitlePathSegment(
							musicPath, filePath, artist, tags);
					}
					catch (Exception exception) when
						(exception is TagLib.CorruptFileException ||
						exception is TagLib.UnsupportedFormatException)
					{
						Log.Error(exception.ToString());
					}
				}

				int depth = Paths.GetDirectoryCount(filePath);
				int libraryPathDepth = Paths.GetDirectoryCount(basePath);
				depth -= libraryPathDepth;

				if (depth == 2)
				{
					// Missing album segment.
					Log.Warn("Missing Album Information: " + filePath);
					album = "Album Information Unavailable";
					artist = Paths.GetPartFromPath(basePath, filePath, 2);
				}

				string extension = Path.GetExtension(filePath);

				filePath = string.Format(
					CultureInfo.InvariantCulture,
					@"{1}{0}{2}{0}{3}{0}{4}{5}",
					Path.DirectorySeparatorChar,
					basePath,
					artist,
					album,
					title,
					extension);
			}

			return filePath;
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

					string jsonText = ConvertTagsToJsonText(tagSet);

					if (!string.IsNullOrWhiteSpace(jsonText))
					{
						destinationFile =
							destinationPath + "\\" + sourceFile.Name + ".json";

						System.IO.File.WriteAllText(destinationFile, jsonText);
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
		/// <param name="filePath">The file path to update.</param>
		/// <param name="useDefaultLibraryPath">Indicates whether to use the
		/// default media library path or not.</param>
		/// <returns>The updated file path.</returns>
		public string UpdateFile(
			string filePath, bool useDefaultLibraryPath = true)
		{
			if (!string.IsNullOrWhiteSpace(filePath))
			{
				string normalizedfilePath =
					NormalizePath(filePath, useDefaultLibraryPath);

				// File path has changed
				if (!normalizedfilePath.Equals(
					filePath, StringComparison.Ordinal))
				{
					// If no file existing with that name, just move it
					if (!System.IO.File.Exists(normalizedfilePath))
					{
						string directory =
							Path.GetDirectoryName(normalizedfilePath);
						Log.Info("Creating Directory: " + directory);
						Directory.CreateDirectory(directory);

						Log.Info("Moving File: " + normalizedfilePath);
						System.IO.File.Move(filePath, normalizedfilePath);
					}
					else
					{
						string existingFile = normalizedfilePath;

						// There is already a file there with that name...
						if (existingFile.Equals(
							filePath, StringComparison.OrdinalIgnoreCase))
						{
							// Windows special case - The file names differ
							// only by case, so need to compensate.  Simply
							// saving with the new name won't work - Windows
							// will just ignore the case change and keep the
							// original name.
							Log.Info("Renaming to Title Case: " +
								existingFile + " from: " + filePath);
							string temporaryFilePath = existingFile + ".tmp";
							File.Move(filePath, temporaryFilePath);
							File.Move(temporaryFilePath, existingFile);
						}
						else
						{
							bool areExactDuplicates =
								FileUtils.AreFilesTheSame(
									existingFile, filePath);

							if (areExactDuplicates == true)
							{
								Log.Info(
									"Deleting Exact Duplicate: " + filePath);
								File.Delete(filePath);
							}
							else
							{
								// move into duplicates
								Log.Info(
									"Moving duplicate file: " +
									normalizedfilePath);
								normalizedfilePath =
									GetDuplicateLocation(existingFile);
								File.Move(filePath, normalizedfilePath);
							}
						}
					}

					filePath = normalizedfilePath;
				}
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
		[Obsolete("UpdateFile(FileInfo) is deprecated, " +
			"please use UpdateFile(string) instead.")]
		public FileInfo UpdateFile(FileInfo file)
		{
			if (file != null)
			{
				string updatedFilePath = UpdateFile(file.FullName);
				file = new (updatedFilePath);
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
				[
					".crd", ".cue", ".doc", ".gif", ".gz", ".htm", ".ini",
					".jpeg", ".jpg", ".lit", ".log", ".m3u", ".nfo", ".opf",
					".pdf", ".plist", ".png", ".psp", ".sav", ".sfv", ".txt",
					".url", ".xls", ".zip"
				];

				if (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path))
				{
					Directory.CreateDirectory(tagsOnlyPath);

					DirectoryInfo directory = new (path);

					FileInfo[] files = directory.GetFiles();

					foreach (FileInfo file in files)
					{
						if (audioFileExtensions.Contains(
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

		private static bool ArePathPartsIncluded(
			string libraryPath, string filePath)
		{
			bool included = false;

			string artist = Paths.GetPartFromPath(libraryPath, filePath, 3);
			string album = Paths.GetPartFromPath(libraryPath, filePath, 2);

			if (!string.IsNullOrWhiteSpace(artist) &&
				!string.IsNullOrWhiteSpace(album))
			{
				included = true;
			}

			return included;
		}

		private static string ConvertTagsToJsonText(
			SortedDictionary<string, object> tagSet)
		{
			string jsonText = null;

			try
			{
				JsonSerializerSettings jsonSettings = new ();
				jsonSettings.NullValueHandling = NullValueHandling.Ignore;
				jsonSettings.ContractResolver =
					new OrderedContractResolver();

				jsonText = JsonConvert.SerializeObject(
					tagSet, Formatting.Indented, jsonSettings);
			}
			catch (Exception exception) when
				(exception is ArgumentException ||
				exception is JsonException)
			{
				Log.Error(exception.ToString());
			}

			return jsonText;
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

		private static string GetAlbumPathSegment(
			string musicPath,
			string filePath,
			string artist,
			MediaFileTags tags)
		{
			string album = Paths.GetAlbumFromPath(musicPath, filePath);

			if (tags != null)
			{
				if (!string.IsNullOrWhiteSpace(tags.Album))
				{
					album = tags.Album;
				}
			}

			string pathSegment = AlbumRules.CleanAlbumFilePath(album, artist);

			if (string.IsNullOrWhiteSpace(pathSegment))
			{
				pathSegment = "Album Information Unavailable";
			}

			return pathSegment;
		}

		private static string GetArtistPathSegment(
			string musicPath,
			string filePath,
			string album,
			MediaFileTags tags)
		{
			string artist = Paths.GetArtistFromPath(musicPath, filePath);

			if (tags != null)
			{
				if (!string.IsNullOrWhiteSpace(tags.Artist))
				{
					artist = tags.Artist;
				}
			}

			string pathSegment = ArtistRules.CleanArtistFilePath(
				artist, album, null);

			if (string.IsNullOrWhiteSpace(pathSegment))
			{
				pathSegment = "Unknown Artist";
			}

			return pathSegment;
		}

		private static string GetDuplicateLocationByNumber(
			string path, int number)
		{
			string locationPath = null;

			if (!string.IsNullOrWhiteSpace(path))
			{
				string basePath = Paths.GetBasePathFromFilePath(path);
				locationPath = basePath +
					number.ToString(CultureInfo.InvariantCulture);
			}

			return locationPath;
		}

		private static string GetTitlePathSegment(
			string musicPath,
			string filePath,
			string artist,
			MediaFileTags tags)
		{
			string title = Paths.GetTitleFromPath(musicPath, filePath);

			if (tags != null)
			{
				if (!string.IsNullOrWhiteSpace(tags.Title))
				{
					title = tags.Title;
				}
			}

			string pathSegment = TitleRules.ApplyTitleFileRules(title, artist);

			if (string.IsNullOrWhiteSpace(pathSegment))
			{
				pathSegment = "Title Information Unavailable";
			}

			return pathSegment;
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
					try
					{
						using MediaFileTags tags = new (file.FullName, rules);
						tags.Clean();
					}
					catch (Exception exception) when
						(exception is TagLib.CorruptFileException ||
						exception is TagLib.UnsupportedFormatException)
					{
						Log.Error(exception.ToString());
					}
				}

				// update directory and file names
				string updatedFilePath = UpdateFile(file.FullName);
				file = new (updatedFilePath);

				if (iTunesManager.IsItunesEnabled == true)
				{
					iTunesManager.UpdateItunesLibrary(file);
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
				bool exists = Directory.Exists(path);

				if (exists == true)
				{
					DirectoryInfo directory = new (path);

					FileInfo[] files = directory.GetFiles();

					foreach (FileInfo file in files)
					{
						if (audioFileExtensions.Contains(
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

		private bool IsStandardLibraryDirectory(string path)
		{
			bool isStandardLibraryDirectory = false;

			string musicPath =
				LibraryLocation + Path.DirectorySeparatorChar + "Music";

			if (!string.IsNullOrWhiteSpace(path))
			{
				if (path.Equals(
					LibraryLocation, StringComparison.OrdinalIgnoreCase) ||
					path.Equals(
					musicPath, StringComparison.OrdinalIgnoreCase))
				{
					isStandardLibraryDirectory = true;
				}
				else if (path.StartsWith(
					musicPath, StringComparison.OrdinalIgnoreCase))
				{
					int begin = musicPath.Length;
					string remaining = path[begin..];
					int end = remaining.IndexOf(
						Path.DirectorySeparatorChar,
						StringComparison.OrdinalIgnoreCase);

					if (end < 1)
					{
						isStandardLibraryDirectory = true;
					}
					else
					{
						string subSegment = path.Substring(begin, end);

						isStandardLibraryDirectory =
							Regex.IsMatch(subSegment, @"\d+$");
					}
				}
			}

			return isStandardLibraryDirectory;
		}
	}
}
