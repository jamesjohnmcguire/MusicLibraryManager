using Common.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using iTunesLib;
using TagLib;

namespace MusicUtility
{
	public class MusicUtility
	{
		private iTunesApp iTunes = null;
		private static readonly ILog log = LogManager.GetLogger
			(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private IITLibraryPlaylist playList = null;
		private string iTunesDirectoryLocation = null;
		private static readonly ResourceManager stringTable =
			new ResourceManager("MusicUtility.Resources",
			Assembly.GetExecutingAssembly());
		private Tags tags = null;

		public string ITunesLibraryLocation
		{
			get
			{
				return iTunesDirectoryLocation;
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
			iTunesDirectoryLocation = iTunesXmlFile.ITunesFolderLocation;
		}

		public int CleanMusicLibrary()
		{
			CleanFiles(iTunesDirectoryLocation);

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
				log.Error(CultureInfo.InvariantCulture, m => m(
					exception.ToString()));
			}

			return same;
		}

		private void CleanFile(FileInfo file)
		{
			try
			{
				log.Info(CultureInfo.InvariantCulture, m => m(
					"Checking: " + file.FullName));

				// get and update tags
				tags = new Tags(file.FullName);

				//file = CleanFileName(file, tags.Artist);

				// update directory and file names
				file = UpdateFile(file);

				// update iTunes
				IITTrackCollection tracks = UpdateItunes(file);
			}
			catch (Exception exception)
			{
				log.Error(CultureInfo.InvariantCulture, m => m(
					exception.ToString()));
			}
		}

		// NOT USED
		private FileInfo CleanFileName(FileInfo file, string artist)
		{
			string regex = @" \[.*?\]";
			string fileName = CleanTrackNumbersOutOfFileName(file);

			if (!fileName.Equals(file.Name))
			{
				string filePath = Path.Combine(file.DirectoryName, fileName);
				file = new FileInfo(filePath);
			}

			if (Regex.IsMatch(file.Name, regex))
			{
				string filePath = Regex.Replace(file.Name, regex, @"");
				file = new FileInfo(filePath);
			}

			if ((!string.IsNullOrWhiteSpace(artist)) &&
				(file.Name.Contains(artist + " - ")))
			{
				string filePath = file.Name.Replace(artist + " - ", "");
				file = new FileInfo(filePath);
			}

			return file;
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
					//UpdateArtistLocation(path);

					DirectoryInfo directory = new DirectoryInfo(path);

					FileInfo[] files = directory.GetFiles();

					foreach (FileInfo file in files)
					{
						if (includes.Contains(file.Extension.ToLower()))
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

					if ((files.Length == 0) && (directories.Length == 0))
					{
						Directory.Delete(path, false);
					}
				}
			}
			catch (Exception exception)
			{
				log.Error(CultureInfo.InvariantCulture, m => m(
					exception.ToString()));
			}
			finally
			{
			}
		}

		// NOT USED
		private string CleanTrackNumbersOutOfFileName(FileInfo file)
		{
			string newName = file.Name;

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
					log.Info(CultureInfo.InvariantCulture, m => m(message));

					newName = filePath;
					break;
				}
			}

			return newName;
		}

		private static void CreateDirectoryIfNotExists(string path)
		{
			DirectoryInfo directory = new DirectoryInfo(path);

			if (!directory.Exists)
			{
				directory.Create();
			}
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

		private string GetAlbumFromPath(string path)
		{
			string artist = GetPathPart(path, 8);

			return artist;
		}

		private string GetArtistFromPath(string path)
		{
			string artist = GetPathPart(path, 7);

			return artist;
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
			int depth = iTunesPathParts.Length - 1;

			while (false == locationOk)
			{
				pathParts[depth] = "Music" + tries.ToString();

				List<string> newList = new List<string>(pathParts);
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

				destinationPath =
					newPath + "\\" + pathParts[pathParts.Length - 1];

				if (!System.IO.File.Exists(destinationPath))
				{
					locationOk = true;
				}
			}

			return destinationPath;
		}

		private string GetPathPart(string path, int index)
		{
			string part = string.Empty;

			string cleanPath = RemoveIntermediaryPath(path);

			string[] pathParts =
				cleanPath.Split(Path.DirectorySeparatorChar);
			string[] iTunesPathParts =
				ITunesLibraryLocation.Split(Path.DirectorySeparatorChar);
			int depth = pathParts.Length - iTunesPathParts.Length;

			part = pathParts[index];

			return part;
		}

		private static string GetPathPartFromTag(string tag, string path)
		{
			if (!string.IsNullOrWhiteSpace(tag))
			{
				path = tag;
				char[] illegalCharactors = new char[]
					{ '<', '>', '"', '?', '*', '\'' };

				foreach(char charactor in illegalCharactors)
				{
					if (path.Contains(charactor))
					{
						path = path.Replace(charactor.ToString(), "");
					}
				}

				illegalCharactors = new char[] { ':', '/', '\\', '|' };

				foreach (char charactor in illegalCharactors)
				{
					if (path.Contains(charactor))
					{
						path = path.Replace(charactor.ToString(), " - ");
					}
				}

				path = path.Replace("  ", " ");
			}

			return path;
		}

		private string GetTitleFromPath(string path)
		{
			string artist = GetPathPart(path, 8);

			return artist;
		}

		// NOT USED
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

		private string RemoveIntermediaryPath(string path)
		{
			string newPath = path;

			string[] pathParts =
				path.Split(Path.DirectorySeparatorChar);
			string[] iTunesPathParts =
				ITunesLibraryLocation.Split(Path.DirectorySeparatorChar);
			int depth = pathParts.Length - iTunesPathParts.Length;

			if ((depth > 2) && pathParts[6].Equals("Music"))
			{
				// there is an extra intermediary directory, remove it
				List<string> list = new List<string>(pathParts);
				list.RemoveAt(7);

				pathParts = list.ToArray();
				newPath = string.Join("\\", pathParts);
			}

			return newPath;
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

			log.Info(CultureInfo.InvariantCulture, m => m(message));
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

		// NOT USED
		private string UpdateArtistLocation(string path)
		{
			string newPath = path;

			string[] pathParts =
				path.Split(Path.DirectorySeparatorChar);
			string[] iTunesPathParts =
				ITunesLibraryLocation.Split(Path.DirectorySeparatorChar);
			int depth = pathParts.Length - iTunesPathParts.Length;

			if ((depth >  2) && pathParts[6].Equals("Music"))
			{
				List<string> sourceList = new List<string>(pathParts);
				sourceList.RemoveAt(sourceList.Count - 1);
				string[] sourceParts = sourceList.ToArray();
				string sourcePath = string.Join("\\", sourceParts);	

				List<string> list = new List<string>(pathParts);
				list.RemoveAt(7);

				pathParts = list.ToArray();
				newPath = string.Join("\\", pathParts);

				list.RemoveAt(list.Count - 1);

				string[] destinationParts = list.ToArray();
				string destinationPath = string.Join("\\", destinationParts);

				DirectoryInfo directory = new DirectoryInfo(destinationPath);

				if (!directory.Exists)
				{
					directory.Create();
					//Directory.Move(sourcePath, destinationPath);
				}
			}

			return newPath;
		}

		// NOT USED
		private static string UpdateAlbumDirectory(string albumPath)
		{
			string newAlbumPath = albumPath;
			string regex = @" \[.*?\]";

			if (albumPath.EndsWith(" (Disc 2)"))
			{
				newAlbumPath = albumPath.Replace(" (Disc 2)", "");
			}

			if (Regex.IsMatch(newAlbumPath, regex))
			{
				newAlbumPath = Regex.Replace(newAlbumPath, regex, @"");
			}

			CreateDirectoryIfNotExists(newAlbumPath);

			return newAlbumPath;
		}

		private FileInfo UpdateFile(FileInfo file)
		{
			string filePath = file.FullName;

			string artist = GetArtistFromPath(file.FullName);
			string pathPart = GetPathPartFromTag(tags.Artist, artist);

			string path = Path.Combine(iTunesDirectoryLocation,
					"Music\\" + pathPart);
			CreateDirectoryIfNotExists(path);

			string album = GetAlbumFromPath(file.FullName);
			pathPart = GetPathPartFromTag(tags.Album, album);
			path = Path.Combine(path, pathPart);
			CreateDirectoryIfNotExists(path);

			string title = GetTitleFromPath(file.FullName);
			pathPart = GetPathPartFromTag(tags.Title, title);
			filePath = path + "\\" + pathPart + file.Extension;

			if (!filePath.Equals(file.FullName))
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

				file = new FileInfo(filePath);
			}

			return file;
		}

		private IITTrackCollection UpdateItunes(FileInfo file)
		{
			string searchName = Path.GetFileNameWithoutExtension(file.Name);

			IITTrackCollection tracks = playList.Search(searchName,
				ITPlaylistSearchField.ITPlaylistSearchFieldAll);

			if (null == tracks)
			{
				// not in collection yet, add it
				IITOperationStatus status =
					iTunes.LibraryPlaylist.AddFile(file.FullName);
			}
			else
			{
				// tracks is a list of potential matches
				bool found = false;

				foreach (IITTrack track in tracks)
				{
					bool same = AreFileAndTrackTheSame(file.FullName, track);

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

				if ((string.IsNullOrWhiteSpace(fileTrack.Location)) ||
					(!filePath.Equals(fileTrack.Location)))
				{
					try
					{
						fileTrack.Location = filePath;
					}
					catch(Exception exception)
					{
						// TODO:  If you get here, find out why the exception,
						// the actual type of exception, then find out if the
						// below code makes any sense
						log.Error(CultureInfo.InvariantCulture, m => m(
							exception.ToString()));

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

				// log.Info(CultureInfo.InvariantCulture, m => m(message));
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
								log.Error(CultureInfo.InvariantCulture, m => m(
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
