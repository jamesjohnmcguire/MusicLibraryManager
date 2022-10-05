/////////////////////////////////////////////////////////////////////////////
// <copyright file="ITunesManager.cs" company="Digital Zen Works">
// Copyright © 2019 - 2022 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Common.Logging;
using iTunesLib;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

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
		/// Are file and track the same method.
		/// </summary>
		/// <param name="filePath">The file path to check.</param>
		/// <param name="track">The iTunes track to check.</param>
		/// <returns>A value indicating whether they are the same
		/// or not.</returns>
		public static bool AreFileAndTrackTheSame(
			string filePath, IITTrack track)
		{
			bool same = false;

			if (track != null)
			{
				if (track.Kind == ITTrackKind.ITTrackKindFile)
				{
					IITFileOrCDTrack fileTrack = (IITFileOrCDTrack)track;

					if (filePath == null && fileTrack.Location == null)
					{
						same = true;
					}
					else if (filePath != null &&
						filePath.Equals(
							fileTrack.Location,
							StringComparison.OrdinalIgnoreCase))
					{
						same = true;
					}
				}
			}

			return same;
		}

		/// <summary>
		/// Is valid iTunes location method.
		/// </summary>
		/// <remarks>This method will check if the location property within
		/// iTunes point to a valid file.</remarks>
		/// <param name="track">The track to check.</param>
		/// <returns>True if the iTunes location is valid,
		/// otherwise false.</returns>
		public static bool IsValidItunesLocation(IITTrack track)
		{
			bool isValid = false;

			if (track != null && track.Kind == ITTrackKind.ITTrackKindFile)
			{
				IITFileOrCDTrack fileTrack = (IITFileOrCDTrack)track;

				if (!string.IsNullOrWhiteSpace(fileTrack.Location) ||
					File.Exists(fileTrack.Location))
				{
					isValid = true;
				}
			}

			return isValid;
		}

		/// <summary>
		/// Delete dead tracks method.
		/// </summary>
		public void DeleteDeadTracks()
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

		/// <summary>
		/// Update file in iTunes.
		/// </summary>
		/// <param name="file">The file to update.</param>
		/// <returns>Indicates whether the actual iTunes library was updated
		/// or not.</returns>
		public bool UpdateItunes(FileInfo file)
		{
			bool updated = false;

			if (file != null)
			{
				string searchName =
					Path.GetFileNameWithoutExtension(file.Name);

				IITTrackCollection tracks = playList.Search(
					searchName,
					ITPlaylistSearchField.ITPlaylistSearchFieldAll);

				if (null == tracks)
				{
					// not in collection yet, add it
					iTunes.LibraryPlaylist.AddFile(file.FullName);
					updated = true;
				}
				else
				{
					// tracks is a list of potential matches
					bool found = false;

					foreach (IITTrack track in tracks)
					{
						// Check the file paths.
						bool same =
							AreFileAndTrackTheSame(file.FullName, track);

						if (true == same)
						{
							found = true;
							break;
						}
					}

					if (false == found)
					{
						// Check to see if there is an existing track with an
						// invalid location to update.
						foreach (IITTrack track in tracks)
						{
							updated =
								UpdateItunesLocation(track, file.FullName);

							if (updated == true)
							{
								// Only update one, to avoid duplicates.
								break;
							}
						}

						if (updated == false)
						{
							// not in collection yet, add it
							iTunes.LibraryPlaylist.AddFile(file.FullName);
							updated = true;
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

			if (track != null)
			{
				if (track.Kind == ITTrackKind.ITTrackKindFile)
				{
					IITFileOrCDTrack fileTrack = (IITFileOrCDTrack)track;

					bool isValid = IsValidItunesLocation(track);

					// only update in iTunes, if the noted file doesn't exist.
					if (isValid == false)
					{
						if (File.Exists(filePath))
						{
							try
							{
								fileTrack.Location = filePath;
								updated = true;
							}
							catch (Exception exception)
							{
								// TODO: If you get here, the actual type of
								// exception, find out why the exception
								// occured, then find out if the below code
								// makes any sense
								Log.Error(CultureInfo.InvariantCulture, m => m(
									exception.ToString()));

								updated = UpdateTrackFromLocation(track, filePath);
							}
						}
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
				// dispose managed resources
				System.Runtime.InteropServices.Marshal.ReleaseComObject(iTunes);
				iTunes = null;
				GC.Collect();
			}
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
					bool same = AreFileAndTrackTheSame(musicFilePath, track);

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
