/////////////////////////////////////////////////////////////////////////////
// <copyright file="ITunesManager.cs" company="Digital Zen Works">
// Copyright © 2019 - 2025 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Common.Logging;
using iTunesLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace DigitalZenWorks.MusicToolKit
{
	/// <summary>
	/// Manages iTunes operations class.
	/// </summary>
	public class ITunesManager : IDisposable
	{
		private static readonly ILog Log = LogManager.GetLogger(
			MethodBase.GetCurrentMethod().DeclaringType);

		private readonly IITLibraryPlaylist playList;
		private readonly string iTunesLibraryLocation;
		private readonly bool isItunesEnabled;
		private readonly string iTunesLibraryXMLPath;

		private iTunesApp iTunes;

		/// <summary>
		/// Initializes a new instance of the <see cref="ITunesManager"/> class.
		/// </summary>
		public ITunesManager()
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
				isItunesEnabled = true;

				playList = iTunes.LibraryPlaylist;
				iTunesLibraryXMLPath = iTunes.LibraryXMLPath;

				ITunesXmlFile iTunesXmlFile = new (iTunesLibraryXMLPath);
				iTunesLibraryLocation = iTunesXmlFile.ITunesFolderLocation;
			}
		}

		/// <summary>
		/// Gets a value indicating whether is iTunes enabled.
		/// </summary>
		/// <value>A value indicating whether is iTunes enabled
		/// or not.</value>
		public bool IsItunesEnabled { get { return isItunesEnabled; } }

		/// <summary>
		/// Gets the iTunes Application Com Reference.
		/// </summary>
		/// <value>The iTunes Application Com Reference.</value>
		public iTunesApp ItunesCom { get { return iTunes; } }

		/// <summary>
		/// Gets the iTunes library location.
		/// </summary>
		/// <value>The iTunes library location.</value>
		public string ItunesLibraryLocation
		{
			get { return iTunesLibraryLocation; }
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
		/// Does iTunes exist method.
		/// </summary>
		/// <returns>A value indicating whether the iTunes application exists
		/// or not.</returns>
		public static bool DoesItunesExist()
		{
			bool exists = File.Exists(@"C:\Program Files\iTunes\iTunes.exe");

			return exists;
		}

		/// <summary>
		/// Are file and track the same method.
		/// </summary>
		/// <param name="filePath">The file path to check.</param>
		/// <param name="track">The iTunes track to check.</param>
		/// <returns>A value indicating whether they are the same
		/// or not.</returns>
		public static bool DoFileAndTrackHaveSameValues(
			string filePath, IITTrack track)
		{
			bool same = false;

			if (File.Exists(filePath) && track != null)
			{
				try
				{
					if (track is IITFileOrCDTrack fileTrack &&
						File.Exists(fileTrack.Location))
					{
						using MediaFileTags tags = new (filePath);

						string album1 = fileTrack.Album;
						string album2 = tags.Album;
						string artist1 = fileTrack.Artist;
						string artist2 = tags.Artist;
						string title1 = fileTrack.Name;
						string title2 = tags.Title;
						int year1 = fileTrack.Year;
						int year2 = (int)tags.Year;

						if (album1.Equals(
							album2, StringComparison.OrdinalIgnoreCase) &&
							artist1.Equals(
							artist2, StringComparison.OrdinalIgnoreCase) &&
							title1.Equals(
							title2, StringComparison.OrdinalIgnoreCase) &&
							year1 == year2)
						{
							same = true;
						}
					}
				}
				catch (Exception exception) when
					(exception is ArgumentException ||
					exception is ArgumentNullException)
				{
					Log.Error(exception.ToString());
				}
			}

			return same;
		}

		/// <summary>
		/// Gets the iTunes supported file types.
		/// </summary>
		/// <returns>The iTunes supported file types.</returns>
		public static string[] GetSupportedFileTypes()
		{
			string[] supportedTpes =
			[
				".aa", ".aac", ".aiff", ".m4a", ".m4p", ".m4v", ".mov",
				".mp3", ".mp4", ".wav"
			];

			return supportedTpes;
		}

		/// <summary>
		/// Is valid iTunes track location method.
		/// </summary>
		/// <remarks>This method will check if the location property within
		/// iTunes point to a valid file.</remarks>
		/// <param name="track">The track to check.</param>
		/// <returns>True if the iTunes location is valid,
		/// otherwise false.</returns>
		public static bool IsValidTrackLocation(IITTrack track)
		{
			bool isValid = false;

			if (track != null && track is IITFileOrCDTrack fileTrack &&
				File.Exists(fileTrack.Location))
			{
				isValid = true;
			}

			return isValid;
		}

		/// <summary>
		/// Updates the track information.
		/// </summary>
		/// <param name="track">The track.</param>
		/// <param name="artist">The artist.</param>
		/// <param name="album">The album.</param>
		/// <param name="title">The title.</param>
		public static void UpdateTrackInformation(
			IITTrack track, string artist, string album, string title)
		{
			if (track != null)
			{
				track.Artist = artist;
				track.Album = album;
				track.Name = title;
			}
		}

		/// <summary>
		/// Adds the file.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns>True.</returns>
		public bool AddFile(string filePath)
		{
			// not in collection yet, add it
			string message =
				"Adding to iTunes Library: " + filePath;
			Log.Info(message);

			iTunes.LibraryPlaylist.AddFile(filePath);

			return true;
		}

		/// <summary>
		/// Delete empty tracks method.
		/// </summary>
		/// <returns>The number of empty tracks removed.</returns>
		public int DeleteEmptyTracks()
		{
			int emptyFound = 0;

			if (iTunes != null)
			{
				IITLibraryPlaylist mainLibrary = iTunes.LibraryPlaylist;
				IITTrackCollection tracks = mainLibrary.Tracks;
				IITFileOrCDTrack fileTrack;

				int trackCount = tracks.Count;

				for (int index = 1; index <= trackCount; index++)
				{
					// only work with tracks as files
					fileTrack = tracks[index] as IITFileOrCDTrack;

					bool emptyTrack = IsTrackLocationEmpty(fileTrack);

					if (emptyTrack == true)
					{
						string message =
							"Deleting Empty Track from iTunes library: " +
							fileTrack.Name;
						Log.Warn(message);

						fileTrack.Delete();

						emptyFound++;
					}
				}
			}

			return emptyFound;
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
		/// Find duplicates methods.
		/// </summary>
		/// <returns>A list of track duplicates.</returns>
		public IList<IITTrack> FindDuplicates()
		{
			IITLibraryPlaylist mainLibrary = iTunes.LibraryPlaylist;
			IITTrackCollection tracks = mainLibrary.Tracks;
			Dictionary<string, IITTrack> trackCollection = [];
			List<IITTrack> duplicateTracks = [];
			IITFileOrCDTrack fileTrack;

			int trackCount = tracks.Count;

			for (int index = 0; index < trackCount; index++)
			{
				fileTrack = tracks[index] as IITFileOrCDTrack;

				trackCollection = AddTrackKeyToCollection(
					trackCollection, ref duplicateTracks, fileTrack);
			}

			return duplicateTracks;
		}

		/// <summary>
		/// Is the file and track the same method.
		/// </summary>
		/// <param name="filePath">The file path to check.</param>
		/// <param name="track">The iTunes track to check.</param>
		/// <returns>A value indicating whether they are the same
		/// or not.</returns>
		public bool IsFileAndTrackSame(string filePath, IITTrack track)
		{
			bool same = false;

			if (track != null)
			{
				string songName =
					Path.GetFileNameWithoutExtension(filePath);

				// Normalize track name, as it may have been distorted
				// elsewhere.
				string trackName =
					TitleRules.ApplyTitleRules(track.Name, null);

				if (trackName.Equals(
					songName, StringComparison.OrdinalIgnoreCase))
				{
					if (track is IITFileOrCDTrack fileTrack)
					{
						same = IsFileAndTrackLocationSame(filePath, fileTrack);
					}
				}
				else
				{
					songName = songName.Replace(
						" -", ":", StringComparison.OrdinalIgnoreCase);

					if (trackName.Equals(
						songName, StringComparison.OrdinalIgnoreCase))
					{
						if (track is IITFileOrCDTrack fileTrack)
						{
							same = IsFileAndTrackLocationSame(filePath, fileTrack);
						}
					}
				}
			}

			return same;
		}

		/// <summary>
		/// Update file in iTunes library.
		/// </summary>
		/// <param name="file">The file to update.</param>
		/// <returns>Indicates whether the actual iTunes library was updated
		/// or not.</returns>
		public bool UpdateItunesLibrary(FileInfo file)
		{
			bool updated = false;

			if (file != null && file.Exists == true)
			{
				string extension = Path.GetExtension(file.FullName);
				string[] supportedTypes = GetSupportedFileTypes();

				if (supportedTypes.Contains(extension))
				{
					// tracks is a list of potential matches
					IITTrackCollection tracks = GetPossibleTracks(file.Name);

					if (null == tracks)
					{
						updated = AddFile(file.FullName);
					}
					else
					{
						IITTrack matchingTrack =
							GetMatchingTrack(tracks, file.FullName);

						if (matchingTrack != null)
						{
							updated = UpdateItunesLocation(
								matchingTrack, file.FullName);
						}
						else
						{
							updated = AddFile(file.FullName);
						}
					}
				}
			}

			return updated;
		}

		/// <summary>
		/// Update iTunes location method.
		/// </summary>
		/// <param name="track">The track to update.</param>
		/// <param name="filePath">The file path to update to.</param>
		/// <returns>A value indicating whether the track location was updated
		/// or not.</returns>
		public bool UpdateItunesLocation(IITTrack track, string filePath)
		{
			bool updated = false;

			if (track != null &&
				track is IITFileOrCDTrack fileTrack &&
				filePath != null)
			{
				// Deal with sym link hell.
				bool isValid = IsValidTrackLocation(track);
				bool isSymLinkSame = IsTrackPathSymLink(
					filePath, fileTrack.Location);

				if (isValid == false || isSymLinkSame == true)
				{
					if (!filePath.Equals(
						fileTrack.Location,
						StringComparison.OrdinalIgnoreCase))
					{
						updated = UpdateFileTrackLocation(fileTrack, filePath);
					}
				}
			}

			return updated;
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
				if (iTunes != null)
				{
					Marshal.ReleaseComObject(iTunes);
					iTunes = null;
					GC.Collect();
				}
			}
		}

		private static Dictionary<string, IITTrack> AddTrackKeyToCollection(
			Dictionary<string, IITTrack> trackCollection,
			ref List<IITTrack> duplicateTracks,
			IITFileOrCDTrack fileTrack)
		{
			if (fileTrack != null)
			{
				string trackKey =
					fileTrack.Name + fileTrack.Artist + fileTrack.Album;

				bool exists =
					trackCollection.TryGetValue(trackKey, out IITTrack value);

				if (exists == false)
				{
					trackCollection.Add(trackKey, fileTrack);
				}
				else
				{
					if ((value.Album != fileTrack.Album) ||
						(value.Artist != fileTrack.Artist))
					{
						trackCollection.Add(trackKey, fileTrack);
					}
					else if (trackCollection[trackKey].BitRate >
						fileTrack.BitRate)
					{
						duplicateTracks.Add(fileTrack);
					}
					else
					{
						trackCollection[trackKey] = fileTrack;
						duplicateTracks.Add(fileTrack);
					}
				}
			}

			return trackCollection;
		}

		private static bool IsTrackLocationEmpty(IITFileOrCDTrack fileTrack)
		{
			bool emptyTrack = false;

			try
			{
				if (fileTrack != null)
				{
					emptyTrack = !File.Exists(fileTrack.Location);
				}
			}
			catch (Exception exception) when
				(exception is ArgumentException ||
				exception is NullReferenceException)
			{
				Log.Error(exception.ToString());
			}

			return emptyTrack;
		}

		private static bool UpdateFileTrackLocation(
			IITFileOrCDTrack fileTrack, string filePath)
		{
			bool updated;

			try
			{
				string message =
					"Updating " + fileTrack.Name + " to " + filePath;
				Log.Info(message);

				fileTrack.Location = filePath;
				updated = true;
			}
			catch (Exception exception)
			{
				// TODO: Note the actual type of exception, find out why the
				// exception occured and if anything can be done about it.
				Log.Error(exception.ToString());
				throw;
			}

			return updated;
		}

		private string GetItunesLibraryFilePath(string sourceFile)
		{
			string filePath = null;

			string artistPath =
				Paths.GetArtistPathFromFilePath(sourceFile);

			if (!string.IsNullOrWhiteSpace(artistPath))
			{
				filePath = iTunesLibraryLocation + artistPath;
			}

			return filePath;
		}

		private IITTrack GetMatchingTrack(
			IITTrackCollection tracks, string fileName)
		{
			IITTrack matchingTrack = null;

			foreach (IITTrack track in tracks)
			{
				bool same = IsFileAndTrackSame(fileName, track);

				if (true == same)
				{
					matchingTrack = track;
					break;
				}
			}

			return matchingTrack;
		}

		private IITTrackCollection GetPossibleTracks(string name)
		{
			string searchName = Path.GetFileNameWithoutExtension(name);

#if NET6_0_OR_GREATER || NETSTANDARD2_1
			searchName = searchName.Replace(
				",", string.Empty, StringComparison.OrdinalIgnoreCase);
			searchName = searchName.Replace(
				".", string.Empty, StringComparison.OrdinalIgnoreCase);
			searchName = searchName.Replace(
				"-", string.Empty, StringComparison.OrdinalIgnoreCase);
			searchName = searchName.Replace(
				"'", " ", StringComparison.OrdinalIgnoreCase);
#else
			searchName = searchName.Replace(",", string.Empty);
			searchName = searchName.Replace(".", string.Empty);
			searchName = searchName.Replace("-", string.Empty);
			searchName = searchName.Replace("'", " ");
#endif

			IITTrackCollection tracks = playList.Search(
				searchName,
				ITPlaylistSearchField.ITPlaylistSearchFieldSongNames);

			return tracks;
		}

		/// <summary>
		/// Are file and track the same method.
		/// </summary>
		/// <param name="filePath">The file path to check.</param>
		/// <param name="fileTrack">The iTunes file track to check.</param>
		/// <returns>A value indicating whether they are the same
		/// or not.</returns>
		private bool IsFileAndTrackLocationSame(
			string filePath, IITFileOrCDTrack fileTrack)
		{
			bool same = false;

			if (filePath == null && fileTrack.Location == null)
			{
				same = true;
			}
			else if (filePath != null)
			{
				if (filePath.Equals(
					fileTrack.Location, StringComparison.OrdinalIgnoreCase))
				{
					same = true;
				}
				else
				{
					// Check for sym link hell.
					same = IsTrackPathSymLink(
						filePath, fileTrack.Location);
				}
			}

			return same;
		}

		private bool IsFileWithinLibraryPath(string filePath)
		{
			bool isWithIn = false;

			if (!string.IsNullOrWhiteSpace(filePath))
			{
				if (filePath.StartsWith(
					iTunesLibraryLocation, StringComparison.OrdinalIgnoreCase))
				{
					isWithIn = true;
				}
			}

			return isWithIn;
		}

		private bool IsTrackPathSymLink(string filePath, string trackLocation)
		{
			bool isSymLink = false;

			bool exists = File.Exists(trackLocation);

			if (exists == true)
			{
				bool check = Paths.DoesPathContainItunesSymLink(trackLocation);

				if (check == true)
				{
					string library = ItunesLibraryLocation;
					string checkPath =
						Paths.ReplaceLibraryPath(trackLocation, library);

					if (filePath.Equals(
						checkPath, StringComparison.OrdinalIgnoreCase))
					{
						isSymLink = true;
					}
				}
			}

			return isSymLink;
		}

		private bool IsTrackWithinLibraryPath(IITTrack track)
		{
			bool isWithIn = false;

			if (track != null && track is IITFileOrCDTrack fileTrack)
			{
				string filePath = fileTrack.Location;

				isWithIn = IsFileWithinLibraryPath(filePath);
			}

			return isWithIn;
		}

		private string UpdateTrackSymLinkLocation(
			IITFileOrCDTrack fileTrack, string symLinkTarget)
		{
			string newLocation;

			bool inside = IsFileWithinLibraryPath(symLinkTarget);

			if (inside == true)
			{
				fileTrack.Location = newLocation = symLinkTarget;
			}
			else
			{
				string newTargetPath =
					GetItunesLibraryFilePath(symLinkTarget);
				File.Move(symLinkTarget, newTargetPath);

				fileTrack.Location = newLocation = newTargetPath;
			}

			return newLocation;
		}
	}
}
