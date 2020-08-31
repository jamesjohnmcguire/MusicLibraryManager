using TagLib;

namespace MusicUtility
{
	public class TagSet
	{
		public virtual string MusicBrainzReleaseArtistId { get; set; }

		public virtual string MusicBrainzTrackId { get; set; }

		public virtual string MusicBrainzDiscId { get; set; }

		public virtual string MusicIpId { get; set; }

		public virtual string AmazonId { get; set; }

		public virtual string MusicBrainzReleaseStatus { get; set; }

		public virtual string MusicBrainzReleaseType { get; set; }

		public virtual string MusicBrainzReleaseCountry { get; set; }

#pragma warning disable CA1819 // Properties should not return arrays
		public virtual IPicture[] Pictures { get; set; }

		public virtual string[] Artists { get; set; }
#pragma warning restore CA1819 // Properties should not return arrays

		public string FirstArtist { get; }

		public string FirstAlbumArtist { get; }

		public string FirstAlbumArtistSort { get; }

		public string FirstPerformer { get; }

		public string FirstPerformerSort { get; }

		public string FirstComposerSort { get; }

		public string FirstComposer { get; }

		public string FirstGenre { get; }

		public string JoinedArtists { get; }

		public string JoinedAlbumArtists { get; }

		public string JoinedPerformers { get; }

		public string JoinedPerformersSort { get; }

		public string JoinedComposers { get; }

		public virtual string MusicBrainzReleaseId { get; set; }

		public virtual string MusicBrainzArtistId { get; set; }

		public virtual string Copyright { get; set; }

		public virtual string Conductor { get; set; }

		public TagTypes TagTypes { get; }

		public virtual string Title { get; set; }

		public virtual string TitleSort { get; set; }

		public virtual string[] Performers { get; set; }

		public virtual string[] PerformersSort { get; set; }

		public virtual string[] AlbumArtists { get; set; }

		public virtual string[] AlbumArtistsSort { get; set; }

		public virtual string[] Composers { get; set; }

		public string JoinedGenres { get; }

		public virtual string Album { get; set; }

		public virtual string[] ComposersSort { get; set; }

		public virtual string Comment { get; set; }

		public virtual string[] Genres { get; set; }

		public virtual uint Year { get; set; }

		public virtual uint Track { get; set; }

		public virtual uint TrackCount { get; set; }

		public virtual uint Disc { get; set; }

		public virtual uint DiscCount { get; set; }

		public virtual string Lyrics { get; set; }

		public virtual string Grouping { get; set; }

		public virtual uint BeatsPerMinute { get; set; }

		public virtual string AlbumSort { get; set; }

		public virtual bool IsEmpty { get; }
	}
}
