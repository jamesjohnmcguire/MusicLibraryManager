/////////////////////////////////////////////////////////////////////////////
// <copyright file="TagSet.cs" company="Digital Zen Works">
// Copyright © 2019 - 2021 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

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

		public string FirstArtist { get; set; }

		public string FirstAlbumArtist { get; set; }

		public string FirstAlbumArtistSort { get; set; }

		public string FirstPerformer { get; set; }

		public string FirstPerformerSort { get; set; }

		public string FirstComposerSort { get; set; }

		public string FirstComposer { get; set; }

		public string FirstGenre { get; set; }

		public string JoinedArtists { get; set; }

		public string JoinedAlbumArtists { get; set; }

		public string JoinedPerformers { get; set; }

		public string JoinedPerformersSort { get; set; }

		public string JoinedComposers { get; set; }

		public virtual string MusicBrainzReleaseId { get; set; }

		public virtual string MusicBrainzArtistId { get; set; }

		public virtual string Copyright { get; set; }

		public virtual string Conductor { get; set; }

		public TagTypes TagTypes { get; set; }

		public virtual string Title { get; set; }

		public virtual string TitleSort { get; set; }

		public virtual string[] Performers { get; set; }

		public virtual string[] PerformersSort { get; set; }

		public virtual string[] AlbumArtists { get; set; }

		public virtual string[] AlbumArtistsSort { get; set; }

		public virtual string[] Composers { get; set; }

		public string JoinedGenres { get; set; }

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

		public virtual bool IsEmpty { get; set; }
	}
}
