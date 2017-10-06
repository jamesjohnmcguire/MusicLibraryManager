using System;
using System.Collections.Generic;
using System.Linq;
using iTunesLib;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MusicUtility
{
	public class PlayListItem
	{
		public readonly Dictionary<string, object> _rawPlaylistDict;

		private IITPlaylist _iitPlaylist;

		private List<TrackItem> _trackItemList;

		private Dictionary<string, List<TrackItem>> _dupeGroups;

		private static ImageSource _imgPlaylistStatus_Green;

		private static ImageSource _imgPlaylistStatus_Red;

		public Dictionary<string, List<TrackItem>> DupeGroups
		{
			get
			{
				if (this._dupeGroups == null)
				{
//					this._dupeGroups = DADatabaseManager.GetDuplicateTrackGroups(this.TrackItemList);
				}
				return this._dupeGroups;
			}
		}

		//public ImageSource ImgPlaylistStatus
		//{
		//	get
		//	{
		//		if (PlayListItem._imgPlaylistStatus_Green == null)
		//		{
		//			PlayListItem._imgPlaylistStatus_Green = new BitmapImage(new Uri("/Dupe Away;component/images/greencirclestatus@2x.png", UriKind.RelativeOrAbsolute));
		//		}
		//		if (PlayListItem._imgPlaylistStatus_Red == null)
		//		{
		//			PlayListItem._imgPlaylistStatus_Red = new BitmapImage(new Uri("/Dupe Away;component/images/redcirclestatus@2x.png", UriKind.RelativeOrAbsolute));
		//		}
		//		if (this.DupeGroups.Count != 0)
		//		{
		//			return PlayListItem._imgPlaylistStatus_Red;
		//		}
		//		return PlayListItem._imgPlaylistStatus_Green;
		//	}
		//}

		public string Name
		{
			get;
			set;
		}

		public string PlaylistTitle
		{
			get
			{
				return (string)this._rawPlaylistDict["Name"];
			}
		}

		//public string TrackCountString
		//{
		//	get
		//	{
		//		return string.Format("{0} Tracks, {1} Dupes", this.TrackItemList.Count, this.DupeGroups.Sum<KeyValuePair<string, List<TrackItem>>>((KeyValuePair<string, List<TrackItem>> dg) => dg.Value.Count) - this.DupeGroups.Count);
		//	}
		//}

/*		public List<TrackItem> TrackItemList
		{
			get
			{
				if (this._trackItemList == null)
				{
					this._trackItemList = new List<TrackItem>();
					if (this._rawPlaylistDict.ContainsKey("Playlist Items"))
					{
						if (this._iitPlaylist == null)
						{
							this._iitPlaylist = DAITunesUtilities.GetIITPlaylistWithName(this.Name);
						}
						ArrayList item = (ArrayList)this._rawPlaylistDict["Playlist Items"];
						int num = 1;
						foreach (Dictionary<string, object> strs in item)
						{
							Dictionary<string, object> trackRawDictForTrackID = DADatabaseManager.getTrackRawDictForTrackID((string)strs["Track ID"]);
							TrackItem TrackItem = new TrackItem(trackRawDictForTrackID);
							int num1 = num;
							num = num1 + 1;
							TrackItem.PlaylistIndex = num1;
							TrackItem.TrackItemType = TrackItem.TrackItemType.iTunesPlaylistTrack;
							TrackItem._parentPlaylist = this;
							this._trackItemList.Add(TrackItem);
						}
					}
				}
				return this._trackItemList;
			}
		}

		static PlayListItem()
		{
		}

		public PlayListItem(string name, Dictionary<string, object> playList)
		{
			this.Name = name;
			this._rawPlaylistDict = playList;
		}

		public PlayListItem()
		{
		}

		public bool DeleteTrackReference(TrackItem tiToDelete)
		{
			bool flag = true;
			int playlistIndex = tiToDelete.PlaylistIndex;
			IITTrack item = this._iitPlaylist.Tracks[playlistIndex];
			if (item.Name != tiToDelete.Name)
			{
				flag = false;
			}
			else
			{
				int count = this._iitPlaylist.Tracks.Count;
				item.Delete();
				if (count - 1 != this._iitPlaylist.Tracks.Count)
				{
					return false;
				}
				foreach (TrackItem trackItemList in this.TrackItemList)
				{
					if (trackItemList.PlaylistIndex <= playlistIndex)
					{
						continue;
					}
					TrackItem TrackItem = trackItemList;
					TrackItem.PlaylistIndex = TrackItem.PlaylistIndex - 1;
				}
			}
			return flag;
		}

		public void LoadDupeGroups()
		{
			this._dupeGroups = DADatabaseManager.GetDuplicateTrackGroups(this.TrackItemList);
		}
		*/
	}
}
