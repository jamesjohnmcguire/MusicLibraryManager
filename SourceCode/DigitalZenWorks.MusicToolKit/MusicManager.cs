/////////////////////////////////////////////////////////////////////////////
// <copyright file="MusicManager.cs" company="Digital Zen Works">
// Copyright © 2019 - 2022 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Common.Logging;
using iTunesLib;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Runtime.Versioning;
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

		private readonly IITLibraryPlaylist playList;
		private readonly string iTunesDirectoryLocation;
		private readonly string iTunesLibraryXMLPath;
		private readonly string librarySkeletonDirectoryLocation;

		private iTunesApp iTunes;
		private Rules rules;
		private MediaFileTags tags;

		/// <summary>
		/// Initializes a new instance of the <see cref="MusicManager"/> class.
		/// </summary>
		public MusicManager()
		{
			try
			{
				// Create a reference to iTunes
				iTunes = new iTunesLib.iTunesApp();
			}
			catch (System.Runtime.InteropServices.COMException exception)
			{
				Log.Warn(exception.ToString());
			}

			if (iTunes != null)
			{
				playList = iTunes.LibraryPlaylist;
				iTunesLibraryXMLPath = iTunes.LibraryXMLPath;

				ITunesXmlFile iTunesXmlFile = new (iTunesLibraryXMLPath);
				iTunesDirectoryLocation = iTunesXmlFile.ITunesFolderLocation;
			}

			string temp = iTunesDirectoryLocation.Trim('\\');
			librarySkeletonDirectoryLocation = temp + "Skeleton";

			GetDefaultRules();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MusicManager"/> class.
		/// </summary>
		/// <param name="rules">The rules to use.</param>
		public MusicManager(Rules rules)
			: this()
		{
			if ((rules != null) && (rules.RulesList != null) &&
				(rules.RulesList.Count > 0))
			{
				this.rules = rules;
			}
		}

		/// <summary>
		/// Gets the iTunes libary location.
		/// </summary>
		/// <value>The iTunes libary location.</value>
		public string ITunesLibraryLocation
		{
			get
			{
				return iTunesDirectoryLocation;
			}
		}

		/// <summary>
		/// Gets the iTunes library XML path.
		/// </summary>
		/// <value>The iTunes library XML path.</value>
		public string ITunesLibraryXMLPath
		{
			get { return iTunesLibraryXMLPath; }
		}

		/// <summary>
		/// Gets the rules.
		/// </summary>
		/// <value>The rules.</value>
		public Rules Rules { get { return rules; } }

		/// <summary>
		/// Clean music library method.
		/// </summary>
		/// <returns>A value indicating success or not.</returns>
		public int CleanMusicLibrary()
		{
			// Operate on the actual music files in the file system
			CleanFiles(iTunesDirectoryLocation);

			// Operate on the iTunes data store
			DeleteDeadTracks();

			// dispose
			System.Runtime.InteropServices.Marshal.ReleaseComObject(iTunes);
			iTunes = null;
			GC.Collect();

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
		/// <returns>A value indicating if the method was successful
		/// or not.</returns>
		public bool SaveTagsToJsonFile(
			FileInfo sourceFile, string destinationPath)
		{
			bool result = false;

			try
			{
				if (sourceFile != null)
				{
					string destinationFile =
						destinationPath + "\\" + sourceFile.Name + ".json";

					tags = new MediaFileTags(sourceFile.FullName, rules);

					TagSet tagSet = tags.TagSet;

					JsonSerializerSettings jsonSettings = new ();
					jsonSettings.NullValueHandling = NullValueHandling.Ignore;
					jsonSettings.ContractResolver =
						new OrderedContractResolver();

					string json = JsonConvert.SerializeObject(
						tagSet, Formatting.Indented, jsonSettings);

					File.WriteAllText(destinationFile, json);

					result = true;
				}
			}
			catch (Exception exception) when
				(exception is ArgumentException ||
				exception is TagLib.CorruptFileException ||
				exception is TagLib.UnsupportedFormatException)
			{
				Log.Error(CultureInfo.InvariantCulture, m => m(
					exception.ToString()));
			}

			return result;
		}

		/// <summary>
		/// Update library skeleton method.
		/// </summary>
		public void UpdateLibrarySkeleton()
		{
			UpdateLibrarySkeleton(
				iTunesDirectoryLocation, librarySkeletonDirectoryLocation);
		}

		/// <summary>
		/// Update library skeleton method.
		/// </summary>
		/// <param name="path">The path of the source files.</param>
		/// <param name="skeletonPath">The path of skeleton files.</param>
		public void UpdateLibrarySkeleton(string path, string skeletonPath)
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
					CreateDirectoryIfNotExists(skeletonPath);

					DirectoryInfo directory = new (path);

					FileInfo[] files = directory.GetFiles();

					foreach (FileInfo file in files)
					{
						if (includes.Contains(
							file.Extension.ToUpperInvariant()))
						{
							SaveTagsToJsonFile(file, skeletonPath);
						}
					}

					string[] directories = Directory.GetDirectories(path);

					foreach (string subDirectory in directories)
					{
						DirectoryInfo subDirectoryInfo = new (subDirectory);
						string nextSkeletonPath =
							skeletonPath + "\\" + subDirectoryInfo.Name;

						UpdateLibrarySkeleton(subDirectory, nextSkeletonPath);
					}

					// refresh
					directories = Directory.GetDirectories(path);
					files = directory.GetFiles();

					if ((files.Length == 0) && (directories.Length == 0) &&
						(!path.Contains(
							"Automatically Add to iTunes",
							StringComparison.OrdinalIgnoreCase)))
					{
						Directory.Delete(path, false);
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
				Log.Error(CultureInfo.InvariantCulture, m => m(
					exception.ToString()));
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
				if (tags != null)
				{
					tags.Dispose();
					tags = null;
				}
			}
		}

		private static string CreateAlbumPathFromTag(
			FileInfo file, string currentPath, string albumTag)
		{
			string album = Paths.GetAlbumFromPath(file.FullName);
			string pathPart = Paths.GetPathPartFromTag(albumTag, album);

			string pattern = @"\.{2,}";

			if (Regex.IsMatch(pathPart, pattern))
			{
				pathPart = Regex.Replace(pathPart, pattern, string.Empty);
			}

			pathPart = pathPart.Trim();
			string path = Path.Combine(currentPath, pathPart);
			CreateDirectoryIfNotExists(path);

			return path;
		}

		private static void CreateDirectoryIfNotExists(string path)
		{
			DirectoryInfo directory = new (path);

			if (!directory.Exists)
			{
				directory.Create();
			}
		}

		private Rules GetDefaultRules()
		{
			string contents = null;

			string resourceName = "DigitalZenWorks.MusicToolKit.DefaultRules.json";
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

		private bool AreFileAndTrackTheSame(IITTrack track)
		{
			bool same = false;

			try
			{
				string album1 = track.Album;
				string album2 = tags.Album;
				string artist1 = track.Artist;
				string artist2 = tags.Artist;
				string title1 = track.Name;
				string title2 = tags.Title;
				int year1 = track.Year;
				int year2 = (int)tags.Year;

				if (string.Equals(
					album1, album2, StringComparison.OrdinalIgnoreCase) &&
					string.Equals(
						artist1, artist2, StringComparison.OrdinalIgnoreCase) &&
					string.Equals(
						title1, title2, StringComparison.OrdinalIgnoreCase) &&
					(year1 == year2))
				{
					same = true;
				}
			}
			catch (Exception exception) when
				(exception is ArgumentException ||
				exception is ArgumentNullException)
			{
				Log.Error(CultureInfo.InvariantCulture, m => m(
					exception.ToString()));
			}

			return same;
		}

		private void CleanFile(FileInfo file)
		{
			try
			{
				string message = "Checking: " + file.FullName;
				Log.Info(CultureInfo.InvariantCulture, m => m(
					message));

				// get and update tags
				tags = new MediaFileTags(file.FullName, rules);
				tags.Update();

				// update directory and file names
				file = UpdateFile(file);

				// update iTunes
				IITTrackCollection tracks = UpdateItunes(file);
			}
			catch (Exception exception) when
				(exception is ArgumentNullException ||
				exception is TagLib.CorruptFileException ||
				exception is DirectoryNotFoundException ||
				exception is FileNotFoundException ||
				exception is IOException ||
				exception is NullReferenceException ||
				exception is IndexOutOfRangeException ||
				exception is InvalidOperationException ||
				exception is UnauthorizedAccessException ||
				exception is TagLib.UnsupportedFormatException)
			{
				Log.Error(CultureInfo.InvariantCulture, m => m(
					exception.ToString()));
			}
		}

		private void CleanFiles(string path)
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

					// refresh
					directories = Directory.GetDirectories(path);
					files = directory.GetFiles();

					if ((files.Length == 0) && (directories.Length == 0) &&
						(!path.Contains(
							"Automatically Add to iTunes",
							StringComparison.OrdinalIgnoreCase)))
					{
						Directory.Delete(path, false);
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
				exception is NullReferenceException ||
				exception is IOException ||
				exception is PathTooLongException ||
				exception is System.Security.SecurityException ||
				exception is TargetException ||
				exception is UnauthorizedAccessException ||
				exception is TagLib.UnsupportedFormatException)
			{
				Log.Error(CultureInfo.InvariantCulture, m => m(
					exception.ToString()));
			}
		}

		private string CreateArtistPathFromTag(FileInfo file, string artistTag)
		{
			string artist = Paths.GetArtistFromPath(file.FullName);
			string pathPart = Paths.GetPathPartFromTag(artistTag, artist);

			string pattern = @"\.{2,}";

			if (Regex.IsMatch(pathPart, pattern))
			{
				pathPart = Regex.Replace(pathPart, pattern, string.Empty);
			}

			pathPart = pathPart.Trim();

			string path = Path.Combine(
				iTunesDirectoryLocation, "Music\\" + pathPart);
			CreateDirectoryIfNotExists(path);

			return path;
		}

		private void DeleteDeadTracks()
		{
			IITLibraryPlaylist mainLibrary = iTunes.LibraryPlaylist;
			IITTrackCollection tracks = mainLibrary.Tracks;
			IITFileOrCDTrack fileTrack;

			int trackCount = tracks.Count;
			int numberChecked = 0;
			int numberDeadFound = 0;

			for (int index = 1; index <= trackCount; index++)
			{
				try
				{
					// only work with files
					fileTrack = tracks[index] as IITFileOrCDTrack;

					// is this a file track?
					if ((null != fileTrack) &&
						(fileTrack.Kind == ITTrackKind.ITTrackKindFile))
					{
						if (string.IsNullOrWhiteSpace(fileTrack.Location))
						{
							numberDeadFound++;

							fileTrack.Delete();
						}
						else if (!File.Exists(fileTrack.Location))
						{
							numberDeadFound++;
							fileTrack.Delete();
						}
					}

					numberChecked++;
				}
				catch (Exception exception) when
					(exception is ArgumentException ||
					exception is NullReferenceException)
				{
					Log.Error(CultureInfo.InvariantCulture, m => m(
						exception.ToString()));
				}
			}
		}

		private IList<IITTrack> FindDuplicates()
		{
			IITLibraryPlaylist mainLibrary = iTunes.LibraryPlaylist;
			IITTrackCollection tracks = mainLibrary.Tracks;
			Dictionary<string, IITTrack> trackCollection = new ();
			List<IITTrack> duplicateTracks = new ();
			IITFileOrCDTrack fileTrack;

			int trackCount = tracks.Count;
			int numberChecked = 0;
			int duplicatesFound = 0;

			for (int index = 0; index < trackCount; index++)
			{
				fileTrack = tracks[index] as IITFileOrCDTrack;

				// is this a file track?
				if ((null != fileTrack) &&
					(fileTrack.Kind == ITTrackKind.ITTrackKindFile))
				{
					numberChecked++;
					string trackKey =
						fileTrack.Name + fileTrack.Artist + fileTrack.Album;

					if (!trackCollection.ContainsKey(trackKey))
					{
						trackCollection.Add(trackKey, fileTrack);
					}
					else
					{
						if ((trackCollection[trackKey].Album !=
								fileTrack.Album) ||
							(trackCollection[trackKey].Artist !=
								fileTrack.Artist))
						{
							trackCollection.Add(trackKey, fileTrack);
						}
						else if (trackCollection[trackKey].BitRate >
							fileTrack.BitRate)
						{
							duplicatesFound++;
							duplicateTracks.Add(fileTrack);
						}
						else
						{
							trackCollection[trackKey] = fileTrack;
							duplicatesFound++;
							duplicateTracks.Add(fileTrack);
						}
					}
				}

				numberChecked++;
			}

			return duplicateTracks;
		}

		private string GetDulicateLocation(string path)
		{
			bool locationOk = false;
			int tries = 2;

			string destinationPath = path;
			string[] pathParts =
				path.Split(Path.DirectorySeparatorChar);

			string[] iTunesPathParts =
				ITunesLibraryLocation.Split(Path.DirectorySeparatorChar);

			while (false == locationOk)
			{
				int depth = iTunesPathParts.Length - 1;
				pathParts[depth] = "Music" +
					tries.ToString(CultureInfo.InvariantCulture);

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

				tries++;
			}

			return destinationPath;
		}

		private FileInfo UpdateFile(FileInfo file)
		{
			string path = CreateArtistPathFromTag(file, tags.Artist);

			path = CreateAlbumPathFromTag(file, path, tags.Album);

			string title = Paths.GetTitleFromPath(file.FullName);
			string pathPart = Paths.GetPathPartFromTag(tags.Title, title);

			string filePath = path + "\\" + pathPart + file.Extension;

			// windows will treat different cases as same file names,
			// so need to compensate
			if (!filePath.Equals(
				file.FullName, StringComparison.Ordinal))
			{
				if (!System.IO.File.Exists(filePath))
				{
					System.IO.File.Move(file.FullName, filePath);
				}
				else
				{
					// a file is already there, move into duplicates
					filePath = GetDulicateLocation(filePath);
					System.IO.File.Move(file.FullName, filePath);
				}
			}

			// might have difference in title case, even though, the OS will
			// treat different cases the same, let's try to keep to the proper
			// title case
			file = new FileInfo(filePath);

			return file;
		}

		private IITTrackCollection UpdateItunes(FileInfo file)
		{
			string searchName = Path.GetFileNameWithoutExtension(file.Name);

			IITTrackCollection tracks = playList.Search(
				searchName, ITPlaylistSearchField.ITPlaylistSearchFieldAll);

			if (null == tracks)
			{
				// not in collection yet, add it
				iTunes.LibraryPlaylist.AddFile(file.FullName);
			}
			else
			{
				// tracks is a list of potential matches
				bool found = false;

				foreach (IITTrack track in tracks)
				{
					bool same = AreFileAndTrackTheSame(track);

					if (true == same)
					{
						found = UpdateItunesLocation(track, file.FullName);
					}
				}

				if (false == found)
				{
					// not in collection yet, add it
					iTunes.LibraryPlaylist.AddFile(file.FullName);
				}
			}

			return tracks;
		}

		private bool UpdateItunesLocation(IITTrack track, string filePath)
		{
			bool result = false;

			if (track.Kind == ITTrackKind.ITTrackKindFile)
			{
				IITFileOrCDTrack fileTrack = (IITFileOrCDTrack)track;

				// only update in iTunes,
				// if the current actual file doesn't exist
				if (string.IsNullOrWhiteSpace(fileTrack.Location) ||
					((!filePath.Equals(
						fileTrack.Location,
						StringComparison.OrdinalIgnoreCase)) &&
					(!System.IO.File.Exists(fileTrack.Location))))
				{
					try
					{
						fileTrack.Location = filePath;
					}
					catch (Exception exception)
					{
						// TODO:  If you get here, find out why the exception,
						// the actual type of exception, then find out if the
						// below code makes any sense
						Log.Error(CultureInfo.InvariantCulture, m => m(
							exception.ToString()));

						result = UpdateTrackFromLocation(track, filePath);
					}
				}
				else
				{
					result = true;
				}
			}

			return result;
		}

		private bool UpdateTrackFromLocation(
			IITTrack track, string musicFilePath)
		{
			bool result = false;

			string album = track.Album;
			string artist = track.Artist;
			string comment = track.Comment;
			string composer = track.Composer;
			int discCount = track.DiscCount;
			int discNumber = track.DiscNumber;
			string genre = track.Genre;
			string grouping = track.Grouping;
			string name = track.Name;
			int playedCount = track.PlayedCount;
			DateTime playedDate = track.PlayedDate;
			int rating = track.Rating;
			int trackCount = track.TrackCount;
			int trackNumber = track.TrackNumber;
			int year = track.Year;

			int count = iTunes.LibraryPlaylist.Tracks.Count;
			IITTrack item = iTunes.LibraryPlaylist.Tracks[count];

			IITOperationStatus status =
				iTunes.LibraryPlaylist.AddFile(musicFilePath);

			IITTrackCollection tracks = playList.Search(
				name, ITPlaylistSearchField.ITPlaylistSearchFieldAll);

			if (null != tracks)
			{
				foreach (IITTrack foundTrack in tracks)
				{
					bool same = AreFileAndTrackTheSame(track);

					if (true == same)
					{
						if (foundTrack.Kind == ITTrackKind.ITTrackKindFile)
						{
							IITFileOrCDTrack fileTrack =
								(IITFileOrCDTrack)foundTrack;

							try
							{
								fileTrack.Location = musicFilePath;
							}
							catch (Exception exception)
							{
								Log.Error(CultureInfo.InvariantCulture, m => m(
									exception.ToString()));
							}

							foundTrack.Artist = artist;
							foundTrack.Album = album;
							foundTrack.PlayedCount = playedCount;
							foundTrack.Rating = rating;
							foundTrack.Comment = comment;
							foundTrack.Composer = composer;
							foundTrack.DiscCount = discCount;
							foundTrack.DiscNumber = discNumber;
							foundTrack.Genre = genre;
							foundTrack.Grouping = grouping;
							foundTrack.Name = name;
							foundTrack.PlayedCount = playedCount;
							foundTrack.PlayedDate = playedDate;
							foundTrack.Rating = rating;
							foundTrack.TrackCount = trackCount;
							foundTrack.TrackNumber = trackNumber;
							foundTrack.Year = year;
						}

						result = true;
						break;
					}
				}
			}

			int newCount = iTunes.LibraryPlaylist.Tracks.Count;

			if (newCount > count)
			{
				track.Delete();
			}

			return result;
		}
	}
}
