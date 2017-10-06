using System;
using System.Collections.Generic;
using iTunesLib;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;

namespace MusicUtility
{
	public class TrackItem : INotifyPropertyChanged
	{
		public int playlistReferenceCount;

		public IITPlaylist _iitPlaylist;

		public PlayListItem _parentPlaylist;

		public bool HasPlaylistDupe;

//		public TrackItem.TrackItemType TrackItemType;

//		private TrackItem.DA_ActionStatus _actionStatus;

		public IITTrack _iITrack;

		private bool? _isMissingTrackFile = null;

		private string _duplicateKeyString;

		private IITTrack _cachedIITTrack;

		//public TrackItem.DA_Abandoned_Track_Action _abandonedTrackAction
		//{
		//	get;
		//	set;
		//}
/*
		public TrackItem.DA_ActionStatus ActionStatus
		{
			get
			{
				return this._actionStatus;
			}
			set
			{
				this._actionStatus = value;
				this.OnPropertyChanged("imgPath");
				this.OnPropertyChanged("DisplayFontColor");
				if (this.TrackItemType == TrackItem.TrackItemType.AbandonedTrack)
				{
					TrackItem.DA_ActionStatus dAActionStatu = this._actionStatus;
					switch (dAActionStatu)
					{
						case TrackItem.DA_ActionStatus.unchanged:
							{
								this.ActionStatusNum = 0;
								break;
							}
						case TrackItem.DA_ActionStatus.willDelete:
							{
								this.ActionStatusNum = 1;
								break;
							}
						default:
							{
								if (dAActionStatu == TrackItem.DA_ActionStatus.willImport)
								{
									this.ActionStatusNum = 2;
									break;
								}
								else
								{
									if (dAActionStatu == TrackItem.DA_ActionStatus.keep)
									{
										goto case TrackItem.DA_ActionStatus.unchanged;
									}
									break;
								}
							}
					}
					this.OnPropertyChanged("ActionStatusNum");
				}
			}
		}

		public int ActionStatusNum
		{
			get;
			set;
		}

		public string Album
		{
			get;
			set;
		}

		public string AlbumArtist
		{
			get;
			set;
		}

		public int AlbumRating
		{
			get;
			set;
		}

		public string Artist
		{
			get;
			set;
		}

		public int BitRate
		{
			get;
			set;
		}

		public int BookmarkTime
		{
			get;
			set;
		}

		public int BPM
		{
			get;
			set;
		}

		public string Category
		{
			get;
			set;
		}

		public string Comment
		{
			get;
			set;
		}

		public bool Compilation
		{
			get;
			set;
		}

		public string Composer
		{
			get;
			set;
		}

		public DateTime DateAdded
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public int DiscCount
		{
			get;
			set;
		}

		public int DiscNumber
		{
			get;
			set;
		}

		public string DisplayFontColor
		{
			get
			{
				if (this.WillDelete)
				{
					return "Red";
				}
				if (this._actionStatus != TrackItem.DA_ActionStatus.willImport && !this.HasPlaylistDupe)
				{
					return "Black";
				}
				return "Blue";
			}
		}
		*/

		//public GroupItem DupeGroup
		//{
		//	get;
		//	set;
		//}

		//public string DuplicateKeyString
		//{
		//	get
		//	{
		//		if (string.IsNullOrEmpty(this._duplicateKeyString))
		//		{
		//			this._duplicateKeyString = DAPreferenceControl.DupeKayTemplateString.Replace("%artist", this.Artist).Replace("%title", this.Name).Replace("%album", this.Album).ToLower();
		//		}
		//		return this._duplicateKeyString;
		//	}
		//}

		public int Duration
		{
			get;
			set;
		}

		public bool Enabled
		{
			get;
			set;
		}

		public string EpisodeID
		{
			get;
			set;
		}

		public int EpisodeNumber
		{
			get;
			set;
		}

		public string EQ
		{
			get;
			set;
		}

		public bool ExcludeFromShuffle
		{
			get;
			set;
		}
/*
		public string FileSizeFormat
		{
			get
			{
				return DAUtilities.ConvertByteSizeToReadableString(this.Size);
			}
		}

		public int Finish
		{
			get;
			set;
		}

		public string Genre
		{
			get;
			set;
		}

		public string Grouping
		{
			get;
			set;
		}

		public string imgPath
		{
			get
			{
				string str = null;
				if (this.ActionStatus == TrackItem.DA_ActionStatus.unchanged)
				{
					if (this.TrackItemType != TrackItem.TrackItemType.AbandonedTrack)
					{
						str = (!this.IsAutoMarkedForDeletion ? "keepdisabledP@2x.png" : "delete@2x.png");
					}
					else
					{
						switch (this._abandonedTrackAction)
						{
							case TrackItem.DA_Abandoned_Track_Action.Ignore:
								{
									str = "ignoredisabledP@2x.png";
									break;
								}
							case TrackItem.DA_Abandoned_Track_Action.Delete:
								{
									str = "delete@2x.png";
									break;
								}
							case TrackItem.DA_Abandoned_Track_Action.Add:
								{
									str = "import@2x.png";
									break;
								}
						}
					}
				}
				else if (this.ActionStatus == TrackItem.DA_ActionStatus.willDelete)
				{
					str = "delete@2x.png";
				}
				else if (this.ActionStatus == TrackItem.DA_ActionStatus.deleted || this.ActionStatus == TrackItem.DA_ActionStatus.deletedFromPlaylist)
				{
					str = "deleted@2x.png";
				}
				else if (this.ActionStatus == TrackItem.DA_ActionStatus.deleting)
				{
					str = "deleting@2x.png";
				}
				else if (this.ActionStatus == TrackItem.DA_ActionStatus.willImport)
				{
					str = "import@2x.png";
				}
				else if (this.ActionStatus == TrackItem.DA_ActionStatus.importing)
				{
					str = "importing@2x.png";
				}
				else if (this.ActionStatus == TrackItem.DA_ActionStatus.imported)
				{
					str = "imported@2x.png";
				}
				else if (this.ActionStatus == TrackItem.DA_ActionStatus.keep)
				{
					str = "keepdisabledP@2x.png";
				}
				return string.Format("/Dupe Away;component/images/{0}", str);
			}
		}

		public int Index
		{
			get;
			set;
		}

		public bool IsAutoMarkedForDeletion
		{
			get;
			set;
		}

		public bool IsMissingTrackFile
		{
			get
			{
				bool flag;
				if (!this._isMissingTrackFile.HasValue)
				{
					if (this.TrackType == TrackItem.ITTrackType.Remote)
					{
						flag = false;
					}
					else
					{
						flag = (this.Location == string.Empty ? true : !File.Exists(this.Location));
					}
					this._isMissingTrackFile = new bool?(flag);
				}
				if (this.ActionStatus == TrackItem.DA_ActionStatus.deleted)
				{
					this._isMissingTrackFile = new bool?(false);
				}
				return this._isMissingTrackFile.Value;
			}
		}

		public ITTrackKind Kind
		{
			get;
			set;
		}

		public string KindAsString
		{
			get;
			set;
		}

		public string Kinds
		{
			get;
			set;
		}

		public string Location
		{
			get;
			set;
		}

		public string LongDescription
		{
			get;
			set;
		}

		public string Lyrics
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public bool PartOfGaplessAlbum
		{
			get;
			set;
		}

		public string PersistentID
		{
			get;
			set;
		}

		public int PlayedCount
		{
			get;
			set;
		}

		public DateTime PlayedDate
		{
			get;
			set;
		}

		public int PlaylistIndex
		{
			get;
			set;
		}

		public string playListName
		{
			get;
			set;
		}

		public int PlaylistRefCount
		{
			get
			{
				return this.playlistReferenceCount;
			}
		}

		public int PlayOrderIndex
		{
			get;
			set;
		}

		public bool Podcast
		{
			get;
			set;
		}

		public int Rating
		{
			get;
			set;
		}

		public DateTime ReleaseDate
		{
			get;
			set;
		}

		public bool RememberBookmark
		{
			get;
			set;
		}

		public int SampleRate
		{
			get;
			set;
		}

		public int SeasonNumber
		{
			get;
			set;
		}

		public string selectionType
		{
			get;
			set;
		}

		public string Show
		{
			get;
			set;
		}
		*/

		//public Visibility ShowICloudIcon
		//{
		//	get
		//	{
		//		if (this.TrackType == TrackItem.ITTrackType.Remote)
		//		{
		//			return Visibility.Visible;
		//		}
		//		return Visibility.Hidden;
		//	}
		//}

		public ulong Size
		{
			get;
			set;
		}

		public int Size64High
		{
			get;
			set;
		}

		public int Size64Low
		{
			get;
			set;
		}

		public int SkippedCount
		{
			get;
			set;
		}

		public DateTime SkippedDate
		{
			get;
			set;
		}

		public string SortAlbum
		{
			get;
			set;
		}

		public string SortAlbumArtist
		{
			get;
			set;
		}

		public string SortArtist
		{
			get;
			set;
		}

		public string SortComposer
		{
			get;
			set;
		}

		public string SortName
		{
			get;
			set;
		}

		public string SortShow
		{
			get;
			set;
		}

		public int sourceID
		{
			get;
			set;
		}

		public int Start
		{
			get;
			set;
		}

		public string Time
		{
			get;
			set;
		}

		public int TrackCount
		{
			get;
			set;
		}

		public int TrackDatabaseID
		{
			get;
			set;
		}

		public int trackID
		{
			get;
			set;
		}

		public int TrackNumber
		{
			get;
			set;
		}

		//public TrackItem.ITTrackType TrackType
		//{
		//	get;
		//	set;
		//}

		public bool Unplayed
		{
			get;
			set;
		}

		public ITVideoKind VideoKind
		{
			get;
			set;
		}

		public int VolumeAdjustment
		{
			get;
			set;
		}
		/*
				public bool WillDelete
				{
					get
					{
						if (this.ActionStatus == TrackItem.DA_ActionStatus.unchanged)
						{
							return this.IsAutoMarkedForDeletion;
						}
						return this.ActionStatus == TrackItem.DA_ActionStatus.willDelete;
					}
				}

				public int Year
				{
					get;
					set;
				}

				public TrackItem(string groupHeader)
				{
				}

				public TrackItem(IITFileOrCDTrack fileTrack)
				{
					this.TrackItemType = TrackItem.TrackItemType.iTunesLibraryTrack;
					this.ActionStatus = TrackItem.DA_ActionStatus.unchanged;
					this._abandonedTrackAction = TrackItem.DA_Abandoned_Track_Action.Ignore;
					this.IsAutoMarkedForDeletion = false;
					this._iITrack = fileTrack;
					this.Name = fileTrack.Name;
					this.SortName = fileTrack.SortName;
					this.Album = fileTrack.Album;
					this.AlbumArtist = fileTrack.AlbumArtist;
					this.AlbumRating = fileTrack.AlbumRating;
					this.Artist = fileTrack.Artist;
					this.BitRate = fileTrack.BitRate;
					this.BookmarkTime = fileTrack.BookmarkTime;
					this.BPM = fileTrack.BPM;
					this.Category = fileTrack.Category;
					this.Comment = fileTrack.Comment;
					this.Compilation = fileTrack.Compilation;
					this.Composer = fileTrack.Composer;
					this.DateAdded = fileTrack.DateAdded;
					this.Description = fileTrack.Description;
					this.Duration = fileTrack.Duration;
					this.Index = fileTrack.Index;
					this.Rating = fileTrack.Rating;
					this.ReleaseDate = fileTrack.ReleaseDate;
					this.Location = fileTrack.Location;
					this.trackID = fileTrack.trackID;
					this.PlayedCount = fileTrack.PlayedCount;
					this.Size = (ulong)fileTrack.Size;
					this.PlayedDate = fileTrack.PlayedDate;
					this.Kind = fileTrack.Kind;
					this.playListName = fileTrack.Playlist.Name;
					this._duplicateKeyString = this.DuplicateKeyString;
				}

				public TrackItem(IITTrack iiTrack)
				{
					this.ActionStatus = TrackItem.DA_ActionStatus.unchanged;
					this._abandonedTrackAction = TrackItem.DA_Abandoned_Track_Action.Ignore;
					this.IsAutoMarkedForDeletion = false;
					this.TrackItemType = TrackItem.TrackItemType.iTunesLibraryTrack;
					this._iITrack = iiTrack;
					this.Name = iiTrack.Name;
					this.Album = iiTrack.Album;
					this.Artist = iiTrack.Artist;
					this.BitRate = iiTrack.BitRate;
					this.BPM = iiTrack.BPM;
					this.Comment = iiTrack.Comment;
					this.Compilation = iiTrack.Compilation;
					this.Composer = iiTrack.Composer;
					this.DateAdded = iiTrack.DateAdded;
					this.Duration = iiTrack.Duration;
					this.Index = iiTrack.Index;
					this.Rating = iiTrack.Rating;
					this.trackID = iiTrack.trackID;
					this.PlayedCount = iiTrack.PlayedCount;
					this.Size = (ulong)iiTrack.Size;
					this.PlayedDate = iiTrack.PlayedDate;
					this.Kind = iiTrack.Kind;
					this.playListName = iiTrack.Playlist.Name;
					this._duplicateKeyString = this.DuplicateKeyString;
				}

				public TrackItem(Dictionary<string, object> fileTrack)
				{
					this.TrackItemType = TrackItem.TrackItemType.iTunesLibraryTrack;
					this.ActionStatus = TrackItem.DA_ActionStatus.unchanged;
					this._abandonedTrackAction = TrackItem.DA_Abandoned_Track_Action.Ignore;
					this.IsAutoMarkedForDeletion = false;
					this.Name = (fileTrack.ContainsKey("Name") ? (string)fileTrack["Name"] : string.Empty);
					this.SortName = (fileTrack.ContainsKey("SortName") ? (string)fileTrack["SortName"] : string.Empty);
					this.Album = (fileTrack.ContainsKey("Album") ? (string)fileTrack["Album"] : string.Empty);
					this.AlbumArtist = (fileTrack.ContainsKey("Album Artist") ? (string)fileTrack["Album Artist"] : string.Empty);
					this.Artist = (fileTrack.ContainsKey("Artist") ? (string)fileTrack["Artist"] : string.Empty);
					this.Category = (fileTrack.ContainsKey("Category") ? (string)fileTrack["Category"] : string.Empty);
					this.Comment = (fileTrack.ContainsKey("Comment") ? (string)fileTrack["Comment"] : string.Empty);
					this.Composer = (fileTrack.ContainsKey("Composer") ? (string)fileTrack["Composer"] : string.Empty);
					this.Description = (fileTrack.ContainsKey("Description") ? (string)fileTrack["Description"] : string.Empty);
					this.Location = (fileTrack.ContainsKey("Location") ? (string)fileTrack["Location"] : string.Empty);
					this.playListName = (fileTrack.ContainsKey("Playlist Name") ? (string)fileTrack["Playlist Name"] : string.Empty);
					this.AlbumRating = (fileTrack.ContainsKey("AlbumRating") ? Convert.ToInt32(fileTrack["AlbumRating"]) : 0);
					this.BitRate = (fileTrack.ContainsKey("Bit Rate") ? Convert.ToInt32(fileTrack["Bit Rate"]) : 0);
					this.BookmarkTime = (fileTrack.ContainsKey("BookmarkTime") ? Convert.ToInt32(fileTrack["BookmarkTime"]) : 0);
					this.BPM = (fileTrack.ContainsKey("BPM") ? Convert.ToInt32(fileTrack["BPM"]) : 0);
					this.Duration = (fileTrack.ContainsKey("Duration") ? Convert.ToInt32(fileTrack["Duration"]) : 0);
					this.Index = (fileTrack.ContainsKey("Index") ? Convert.ToInt32(fileTrack["Index"]) : 0);
					this.Rating = (fileTrack.ContainsKey("Rating") ? Convert.ToInt32(fileTrack["Rating"]) : 0);
					this.trackID = (fileTrack.ContainsKey("Track ID") ? Convert.ToInt32(fileTrack["Track ID"]) : 0);
					this.PlayedCount = (fileTrack.ContainsKey("Play Count") ? Convert.ToInt32(fileTrack["Play Count"]) : 0);
					this.PersistentID = (fileTrack.ContainsKey("Persistent ID") ? (string)fileTrack["Persistent ID"] : "0");
					string str = (fileTrack.ContainsKey("Track Type") ? (string)fileTrack["Track Type"] : string.Empty);
					if (str == "File")
					{
						this.TrackType = TrackItem.ITTrackType.File;
					}
					else if (str != "Remote")
					{
						this.TrackType = TrackItem.ITTrackType.Unknown;
					}
					else
					{
						this.TrackType = TrackItem.ITTrackType.Remote;
					}
					this.ReleaseDate = (fileTrack.ContainsKey("ReleaseDate") ? Convert.ToDateTime(fileTrack["ReleaseDate"]) : DateTime.Now);
					this.DateAdded = (fileTrack.ContainsKey("DateAdded") ? Convert.ToDateTime(fileTrack["Date Added"]) : DateTime.Now);
					this.PlayedDate = (fileTrack.ContainsKey("PlayDate") ? Convert.ToDateTime(fileTrack["Play Date UTC"]) : DateTime.Now);
					this.Size = (fileTrack.ContainsKey("Size") ? ulong.Parse((string)fileTrack["Size"]) : (ulong)((long)0));
					this.Kinds = (fileTrack.ContainsKey("Kind") ? (string)fileTrack["Kind"] : string.Empty);
					this._duplicateKeyString = this.DuplicateKeyString;
				}

				public TrackItem(FileInfo fi)
				{
					this.ActionStatus = TrackItem.DA_ActionStatus.unchanged;
					this.ActionStatusNum = 0;
					this._abandonedTrackAction = TrackItem.DA_Abandoned_Track_Action.Ignore;
					this.IsAutoMarkedForDeletion = false;
					this.TrackItemType = TrackItem.TrackItemType.AbandonedTrack;
					this.Name = fi.Name.Replace(fi.Extension, "");
					this.Location = fi.FullName;
					this.Size = (ulong)fi.Length;
				}

				public void DleleteAbandonedTrackFromLocation()
				{
					if (File.Exists(this.Location))
					{
						File.Delete(this.Location);
					}
				}

				public bool ExecuteAction()
				{
					bool flag = true;
					switch (this.TrackItemType)
					{
						case TrackItem.TrackItemType.iTunesLibraryTrack:
							{
								if (!this.WillDelete)
								{
									break;
								}
								try
								{
									IITTrack iTTrack = this.GetIITTrack();
									if (this.TrackType == TrackItem.ITTrackType.File)
									{
										this.ExecuteDeletionOfFile();
									}
									if (iTTrack == null)
									{
										this.ActionStatus = TrackItem.DA_ActionStatus.error;
										flag = false;
									}
									else
									{
										iTTrack.Delete();
										this.ActionStatus = TrackItem.DA_ActionStatus.deleted;
									}
									break;
								}
								catch (Exception exception)
								{
									Console.WriteLine(string.Concat("Failed to delete track from iTunes.  Error: ", exception.ToString()));
									flag = false;
									break;
								}
								break;
							}
						case TrackItem.TrackItemType.iTunesPlaylistTrack:
							{
								if (!this.WillDelete)
								{
									break;
								}
								flag = this._parentPlaylist.DeleteTrackReference(this);
								if (!flag)
								{
									this.ActionStatus = TrackItem.DA_ActionStatus.error;
									break;
								}
								else
								{
									this.ActionStatus = TrackItem.DA_ActionStatus.deleted;
									break;
								}
							}
						case TrackItem.TrackItemType.AbandonedTrack:
							{
								if (!this.WillDelete)
								{
									if (this.ActionStatus != TrackItem.DA_ActionStatus.willImport)
									{
										break;
									}
									this.ActionStatus = TrackItem.DA_ActionStatus.importing;
									try
									{
										IITOperationStatus variable = DAITunesUtilities.ITunesApplication.LibraryPlaylist.AddFile(this.Location);
										int num = 0;
										while (variable.InProgress)
										{
											int num1 = num;
											num = num1 + 1;
											if (num1 >= 10)
											{
												break;
											}
											Thread.Sleep(200);
										}
										if (variable.InProgress || variable.Tracks.Count <= 0)
										{
											flag = false;
										}
									}
									catch (Exception exception1)
									{
										Console.WriteLine("Error importing track {0} to iTunes:  {1}", this.Location, exception1.ToString());
										flag = false;
									}
									if (!flag)
									{
										this.ActionStatus = TrackItem.DA_ActionStatus.error;
										break;
									}
									else
									{
										this.ActionStatus = TrackItem.DA_ActionStatus.imported;
										break;
									}
								}
								else
								{
									flag = this.ExecuteDeletionOfFile();
									if (!flag)
									{
										break;
									}
									this.ActionStatus = TrackItem.DA_ActionStatus.deleted;
									break;
								}
							}
					}
					return flag;
				}

				private bool ExecuteDeletionOfFile()
				{
					bool flag = true;
					this.ActionStatus = TrackItem.DA_ActionStatus.deleting;
					try
					{
						File.Delete(this.Location);
					}
					catch (Exception exception)
					{
						Console.WriteLine("Deletion of track with path {0} failed: {1}", this.Location, exception.ToString());
						flag = false;
						this.ActionStatus = TrackItem.DA_ActionStatus.error;
					}
					return flag;
				}

				public IITTrack GetIITTrack()
				{
					if (this._cachedIITTrack == null)
					{
						int num = int.Parse(this.PersistentID.Substring(0, 8), NumberStyles.HexNumber);
						int num1 = int.Parse(this.PersistentID.Substring(8, 8), NumberStyles.HexNumber);
						this._cachedIITTrack = DAITunesUtilities.ITunesApplication.LibraryPlaylist.Tracks[num, num1];
					}
					return this._cachedIITTrack;
				}

				public bool ImportAbandonedTrackToItunes()
				{
					IITOperationStatus variable = DAITunesUtilities.ITunesApplication.LibraryPlaylist.AddFile(this.Location);
					IITTrackCollection tracks = variable.Tracks;
					for (int i = tracks.Count; i > 0; i--)
					{
						IITTrack item = tracks[i];
						if (item.Kind == ITTrackKind.ITTrackKindFile)
						{
							DADatabaseManager.AllFileTracks.Add(new TrackItem((IITFileOrCDTrack)item));
							return true;
						}
					}
					return false;
				}

				public bool IsDupeBasedUponTrackExtension(TrackItem ti)
				{
					if (this.TrackType == TrackItem.ITTrackType.Remote || ti.TrackType == TrackItem.ITTrackType.Remote)
					{
						return true;
					}
					return Path.GetExtension(this.Location).ToLower() == Path.GetExtension(ti.Location).ToLower();
				}

				public bool IsDuplicateTrack(TrackItem ti)
				{
					if (this.DuplicateKeyString == ti.DuplicateKeyString)
					{
						if (!Settings.Default.ChkFileSize)
						{
							return true;
						}
						if (Settings.Default.PrefExtension && this.IsDupeBasedUponTrackExtension(ti))
						{
							if (Settings.Default.prefFilterExactSize)
							{
								return this.Size == ti.Size;
							}
							return (double)((float)Math.Abs((float)((float)this.Size) - (float)((float)ti.Size)) / (float)((float)ti.Size)) < 0.05;
						}
					}
					return false;
				}

				public static bool IsDuplicateTrack(TrackItem t1, TrackItem t2)
				{
					if (!Settings.Default.ChkFileSize)
					{
						return true;
					}
					if (Settings.Default.prefFilterExactSize)
					{
						return t1.Size == t2.Size;
					}
					return (double)((float)Math.Abs((float)((float)t1.Size) - (float)((float)t2.Size)) / (float)((float)t2.Size)) < 0.05;
				}

				public bool LocationIsSameAsFileInfo(FileInfo fi)
				{
					if (string.IsNullOrWhiteSpace(this.Location))
					{
						return false;
					}
					return this.Location.Contains(fi.FullName);
				}
*/
				protected void OnPropertyChanged(string name)
				{
					PropertyChangedEventHandler propertyChangedEventHandler = this.PropertyChanged;
					if (propertyChangedEventHandler != null)
					{
						propertyChangedEventHandler(this, new PropertyChangedEventArgs(name));
					}
				}

				public event PropertyChangedEventHandler PropertyChanged;
/*
				public enum DA_Abandoned_Track_Action
				{
					Ignore,
					Delete,
					Add
				}

				public enum DA_ActionStatus
				{
					unchanged,
					willDelete,
					deleting,
					deleted,
					deletedFromPlaylist,
					willImport,
					importing,
					imported,
					error,
					keep
				}

				public enum TrackItemState
				{
					Default
				}

				public enum TrackItemType
				{
					iTunesLibraryTrack,
					iTunesPlaylistTrack,
					AbandonedTrack
				}

				public enum ITTrackType
				{
					File,
					Remote,
					Unknown
				}
		*/
	}
}
