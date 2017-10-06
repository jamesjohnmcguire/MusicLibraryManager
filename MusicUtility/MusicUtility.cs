﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using iTunesLib;
using TagLib;

namespace MusicUtility
{
	public class MusicUtility
	{
		private iTunesApp iTunes = null;
		private IITLibraryPlaylist playList = null;
		private string iTunesFolderLocation = null;

		public string ITunesLibraryLocation
		{
			get
			{
				return null;
			}
		}
		public MusicUtility()
		{
			//create a reference to iTunes
			//iTunesAppClass iTunes = new iTunesAppClass();
			iTunes = new iTunesLib.iTunesApp();
			playList = iTunes.LibraryPlaylist;

			ItunesXmlFile iTunesXmlFile =
				new ItunesXmlFile(iTunes.LibraryXMLPath);
			iTunesFolderLocation = iTunesXmlFile.ITunesFolderLocation;
		}

		public int CleanMusicLibrary()
		{
			CleanFiles(iTunesFolderLocation);

			// dispose
			System.Runtime.InteropServices.Marshal.ReleaseComObject(iTunes);
			iTunes = null;
			GC.Collect();

			return 0;
		}

		private IITTrack AddFileFromLocation(ref IITTrack thisITTrack, string thisLocation)
		{
			string name = thisITTrack.Name;
			string artist = thisITTrack.Artist;
			string album = thisITTrack.Album;
			int playedCount = thisITTrack.PlayedCount;
			int rating = thisITTrack.Rating;
			string genre = thisITTrack.Genre;
			iTunes.LibraryPlaylist.AddFile(thisLocation);
			int count = iTunes.LibraryPlaylist.Tracks.Count;
			IITTrack item = iTunes.LibraryPlaylist.Tracks[count];
			if (!item.Name.Equals(name))
			{
				return null;
			}
			try
			{
				item.Artist = artist;
			}
			catch (Exception exception)
			{
			}
			try
			{
				item.Album = album;
			}
			catch (Exception exception1)
			{
			}
			try
			{
				item.PlayedCount = playedCount;
			}
			catch (Exception exception2)
			{
			}
			try
			{
				item.Rating = rating;
			}
			catch (Exception exception3)
			{
			}
			try
			{
				item.Genre = genre;
			}
			catch (Exception exception4)
			{
			}

			thisITTrack.Delete();
			return item;
		}

		private bool AreFileAndTrackTheSame(string file, IITTrack track)
		{
			bool same = false;

			try
			{
				Tags tags = new Tags(file);

				string album1 = track.Album;
				string album2 = tags.Album;
				string artist1 = track.Artist;
				string artist2 = tags.Artist;
				string title1 = track.Name;
				string title2 = tags.Title;
				int year1 = track.Year;
				int year2 = (int)tags.Year;

				if ((album1.Equals(album2,
					StringComparison.OrdinalIgnoreCase)) && (artist1.Equals(
					artist2, StringComparison.OrdinalIgnoreCase)) &&
					(title1.Equals(title2,
					StringComparison.OrdinalIgnoreCase)) &&
					(year1 == year2))
				{
					same = true;
				}
			}
			catch(Exception exception)
			{
				Console.WriteLine("Exception: " + exception.Message);
				//log.Error(CultureInfo.InvariantCulture, m => m(
				//	stringTable.GetString("EXCEPTION") + ex.Message));
			}

			return same;
		}

		private void CleanFile(FileInfo file)
		{
			try
			{
				Console.WriteLine("Checking: " + file.FullName);

				string filePath = CleanTrackNumbersOutOfFileName(file);

				// filename might have changed
				file = new FileInfo(filePath);

				IITTrackCollection tracks =
					GetTracksFromFileName(file.Name, filePath);

				if (null == tracks)
				{
					// not in collection yet, add it
					string filePath = MoveFileBasedOnTags(file);

					IITOperationStatus status =
						iTunes.LibraryPlaylist.AddFile(filePath);
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine("Exception: " + exception.Message);
				//log.Error(CultureInfo.InvariantCulture, m => m(
				//	stringTable.GetString("EXCEPTION") + ex.Message));
			}
		}

		private void CleanFiles(string path)
		{
			try
			{
				string[] excludes = { ".crd", ".cue", ".doc", ".flac", ".gif",
					".gz", ".htm", ".ini", ".jpeg", ".jpg", ".lit", ".log",
					".m3u", ".nfo", ".opf", ".pdf", ".plist", ".png", ".psp",
					".sav", ".sfv", ".txt", ".url", ".xls", ".zip" };
				string[] includes = { ".aifc", ".m4a", ".mp3", ".wav",
					".wma" };

				if (Directory.Exists(path))
				{
					DirectoryInfo dir = new DirectoryInfo(path);

					FileInfo[] files = dir.GetFiles();

					foreach (FileInfo file in files)
					{
						if (includes.Contains(file.Extension.ToLower()))
						{
							CleanFile(file);
						}
					}

					string[] directories = Directory.GetDirectories(path);

					foreach (string directory in directories)
					{
						CleanFiles(directory);
					}
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine("Exception: " + exception.Message);
				//log.Error(CultureInfo.InvariantCulture, m => m(
				//	stringTable.GetString("EXCEPTION") + ex.Message));
			}
			finally
			{
			}
		}

		private string CleanTrackNumbersOutOfFileName(FileInfo file)
		{
			string newName = null;
			string[] regexes =
				new string[] { @"^\d+-\d+ ", @"^\d+ - ", @"^\d+ " };

			foreach (string regex in regexes)
			{
				if (Regex.IsMatch(file.Name, regex))
				{
					string oldName = file.Name;

					//name =
					//	Regex.Replace(name, @"(?<prefix>\d+)", @"${prefix}");
					string name = Regex.Replace(file.Name, regex, @"");

					string filePath = Path.Combine(file.DirectoryName, name);

					System.IO.File.Move(file.FullName, filePath);

					string message = string.Format("updated: {0} with: {1}",
						oldName, name);
					Console.WriteLine(message);

					newName = filePath;
					break;
				}
			}

			return newName;
		}

		private void FindDeadTracks()
		{
			//get a reference to the collection of all tracks
			IITTrackCollection tracks = iTunes.LibraryPlaylist.Tracks;

			int trackCount = tracks.Count;
			int numberChecked = 0;
			int numberDeadFound = 0;

			//setup the progress control
			//this.SetupProgress(trackCount);

			for (int i = trackCount; i > 0; i--)
			{
				//if (!this._shouldStop)
				{
					IITTrack track = tracks[i];
					numberChecked++;
					//this.IncrementProgress();
					//this.UpdateLabel("Checking track # " + numberChecked.ToString() + " - " + track.Name);

					if (track.Kind == ITTrackKind.ITTrackKindFile)
					{
						IITFileOrCDTrack fileTrack = (IITFileOrCDTrack)track;

						//if the file doesn't exist, we'll delete it from iTunes
						if (fileTrack.Location == String.Empty)
						{
							numberDeadFound++;
							//this.AddTrackToList(fileTrack);

							//if (this.checkBoxRemove.Checked)
							{
								fileTrack.Delete();
							}
						}
						else if (!System.IO.File.Exists(fileTrack.Location))
						{
							numberDeadFound++;
							//this.AddTrackToList(fileTrack);

							//if (this.checkBoxRemove.Checked)
							{
								fileTrack.Delete();
							}
						}
					}
				}
			}

			//this.UpdateLabel("Checked " + numberChecked.ToString() + " tracks and " + numberDeadFound.ToString() + " dead tracks found.");
			//this.SetupProgress(1);
		}

		private DataTable GetTracks(IITPlaylist playlist)
		{
			DataTable tracksTable = new DataTable();

			tracksTable.Rows.Clear();

			IITTrackCollection tracks = playlist.Tracks;
			int numTracks = tracks.Count;

			foreach(IITTrack track in tracks)
			{
				DataRow row = tracksTable.NewRow();

				// get the track from the current track list
				row["artist"] = track.Artist;
				row["song name"] = track.Name;
				row["album"] = track.Album;
				row["genre"] = track.Genre;

				// if track is a file, then get the file 
				// location on the drive. 
				if (track.Kind == ITTrackKind.ITTrackKindFile)
				{
					IITFileOrCDTrack file = (IITFileOrCDTrack)track;
					if (file.Location != null)
					{
						FileInfo fi = new FileInfo(file.Location);
						if (fi.Exists)
						{
							row["FileLocation"] = file.Location;
						}
						else
							row["FileLocation"] =
									 "not found " + file.Location;
					}
				}
				tracksTable.Rows.Add(row);
			}

			//lbl_numsongs.Text =
			//	"number of songs: " + dataTable1.Rows.Count.ToString() +
			//	", total file size: " +
			//	(totalfilesize / 1024.00 / 1024.00).ToString("#,### mb");

			return tracksTable;
		}

		private IITTrackCollection GetTracksFromFileName(string fileName,
			string filePath)
		{
			string searchName = Path.GetFileNameWithoutExtension(fileName);

			IITTrackCollection tracks = playList.Search(searchName,
				ITPlaylistSearchField.ITPlaylistSearchFieldAll);

			if (null != tracks)
			{
				bool found = false;

				foreach (IITTrack track in tracks)
				{
					bool same = AreFileAndTrackTheSame(filePath, track);

					if (true == same)
					{
						// TODO update based on tags
						FileInfo file = new FileInfo(filePath);
						filePath = MoveFileBasedOnTags(file);

						found =
							UpdateLocation(track, filePath);
						//	File.Move(file.FullName, file.FullPath);
						//	string message = string.Format("updated: {0} with: {1}",
						//		oldName, name);
						//	Console.WriteLine(message);
					}
				}

				if (false == found)
				{
					// TODO update based on tags
					FileInfo file = new FileInfo(filePath);
					filePath = MoveFileBasedOnTags(file);
					iTunes.LibraryPlaylist.AddFile(filePath);
				}
			}

			return tracks;
		}

		private string MoveFileBasedOnTags(FileInfo file)
		{
			string filePath = file.FullName;
			Tags tags = new Tags(file.FullName);

			if (!string.IsNullOrWhiteSpace(tags.Artist))
			{
				string path = Path.Combine(iTunesFolderLocation,
					"Music\\" + tags.Artist);
				DirectoryInfo directory = new DirectoryInfo(path);

				if (!directory.Exists)
				{
					directory.Create();
				}

				if (!string.IsNullOrWhiteSpace(tags.Album))
				{
					path = Path.Combine(path, tags.Album);
					directory = new DirectoryInfo(path);

					if (!directory.Exists)
					{
						directory.Create();
					}
				}

				if (!string.IsNullOrWhiteSpace(tags.Title))
				{
					filePath =
						path + "\\" + tags.Title + file.Extension;
				}

				System.IO.File.Move(file.FullName, filePath);
			}

			return filePath;
		}

		private void RemoveDeadTracks()
		{
			IITLibraryPlaylist mainLibrary = iTunes.LibraryPlaylist;
			IITTrackCollection tracks = mainLibrary.Tracks;
			IITFileOrCDTrack currTrack;

			int numTracks = tracks.Count;
			int deletedTracks = 0;

			while (numTracks != 0)
			{
				// only work with files
				currTrack = tracks[numTracks] as IITFileOrCDTrack;

				// is this a file track?
				if (currTrack != null && currTrack.Kind == ITTrackKind.ITTrackKindFile)
				{
					// yes, does it have an empty location?
					if (currTrack.Location == null)
					{
						// yes, delete it
						currTrack.Delete();
						deletedTracks++;
					}
				}

				// progress to the next tack
				numTracks--;
			}

			// report to the user the results
			string message = string.Format("Removed { 0} track{ 1}.",
				deletedTracks, deletedTracks == 1 ? "" : "s");

			Console.WriteLine(message);
		}

		private void RemoveDuplicates()
		{
			//get a reference to the collection of all tracks
			IITTrackCollection tracks = iTunes.LibraryPlaylist.Tracks;

			int trackCount = tracks.Count;
			int numberChecked = 0;
			int numberDuplicateFound = 0;
			Dictionary<string, IITTrack> trackCollection = new Dictionary<string, IITTrack>();
			ArrayList tracksToRemove = new ArrayList();

			//setup the progress control
			//this.SetupProgress(trackCount);

			for (int i = trackCount; i > 0; i--)
			{
				if (tracks[i].Kind == ITTrackKind.ITTrackKindFile)
				{
					//if (!this._shouldStop)
					{
						numberChecked++;
						//this.IncrementProgress();
						//this.UpdateLabel("Checking track # " + numberChecked.ToString() + " - " + tracks[i].Name);
						string trackKey = tracks[i].Name + tracks[i].Artist + tracks[i].Album;

						if (!trackCollection.ContainsKey(trackKey))
						{
							trackCollection.Add(trackKey, tracks[i]);
						}
						else
						{
							if (trackCollection[trackKey].Album != tracks[i].Album || trackCollection[trackKey].Artist != tracks[i].Artist)
							{
								trackCollection.Add(trackKey, tracks[i]);
							}
							else if (trackCollection[trackKey].BitRate > tracks[i].BitRate)
							{
								IITFileOrCDTrack fileTrack = (IITFileOrCDTrack)tracks[i];
								numberDuplicateFound++;
								tracksToRemove.Add(tracks[i]);
							}
							else
							{
								IITFileOrCDTrack fileTrack = (IITFileOrCDTrack)tracks[i];
								trackCollection[trackKey] = fileTrack;
								numberDuplicateFound++;
								tracksToRemove.Add(tracks[i]);
							}
						}
					}
				}
			}

			//this.SetupProgress(tracksToRemove.Count);

			for (int i = 0; i < tracksToRemove.Count; i++)
			{
				IITFileOrCDTrack track = (IITFileOrCDTrack)tracksToRemove[i];
				//this.UpdateLabel("Removing " + track.Name);
				//this.IncrementProgress();
				//this.AddTrackToList((IITFileOrCDTrack)tracksToRemove[i]);

				//if (this.checkBoxRemove.Checked)
				{
					track.Delete();
				}
			}

			//this.UpdateLabel("Checked " + numberChecked.ToString() + " tracks and " + numberDuplicateFound.ToString() + " duplicate tracks found.");
			//this.SetupProgress(1);
		}

		private bool UpdateLocation(IITTrack track, string filePath)
		{
			bool result = false;

			if (track.Kind == ITTrackKind.ITTrackKindFile)
			{
				IITFileOrCDTrack fileTrack = (IITFileOrCDTrack)track;

				if ((string.IsNullOrWhiteSpace(fileTrack.Location)) ||
					(!filePath.Equals(fileTrack.Location)))
				{
					try
					{
						fileTrack.Location = filePath;
					}
					catch(Exception exception)
					{
						Console.WriteLine("Exception: " + exception.Message);
						//log.Error(CultureInfo.InvariantCulture, m => m(
						//	stringTable.GetString("EXCEPTION") + ex.Message));

						//addFileFromLocation(ref track, filePath);
						result = UpdateTrackFromLocation(track, filePath);
					}
				}
				else
				{
					result = true;
				}
				//else if (!string.IsNullOrWhiteSpace(fileTrack.Location))
				//{
				//	string location = "C:\\Users\\JamesMc\\Data\\External\\Entertainment\\Music\\Music\\The Jackson 5\\Soul Source Jackson 5 Remixes\\05 ABC (Love Stream Mix By Kayoko Ki).m4a";
				//	fileTrack.Location = location;

				//	Console.WriteLine("Location: " + fileTrack.Location);
				//}
			}

			return result;
		}

		private bool UpdateTrackFromLocation(IITTrack track,
			string musicFilePath)
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

			//IITEncoder encoder = iTunes.CurrentEncoder;
			//status = iTunes.ConvertFile2(musicFilePath);

			IITTrackCollection tracks = playList.Search(name,
				ITPlaylistSearchField.ITPlaylistSearchFieldAll);

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
								Console.WriteLine("Exception: " +
									exception.Message);
								//log.Error(CultureInfo.InvariantCulture, m => m(
								//	stringTable.GetString("EXCEPTION") + ex.Message));
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
						//if ((!string.IsNullOrWhiteSpace(name)) && (name.Equals(item.Name)))
						//{
						//	return null;
						//}

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
