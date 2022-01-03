/////////////////////////////////////////////////////////////////////////////
// <copyright file="TagSet.cs" company="Digital Zen Works">
// Copyright © 2019 - 2022 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using TagLib;

namespace DigitalZenWorks.MusicUtility
{
	/// <summary>
	/// Tag set class.
	/// </summary>
	public class TagSet
	{
		/// <summary>
		/// Gets or sets the MusicBrainz release artist id.
		/// </summary>
		/// <value>The MusicBrainz release artist id.</value>
		public virtual string MusicBrainzReleaseArtistId { get; set; }

		/// <summary>
		/// Gets or sets the MusicBrainz track id.
		/// </summary>
		/// <value>The MusicBrainz track id.</value>
		public virtual string MusicBrainzTrackId { get; set; }

		/// <summary>
		/// Gets or sets the MusicBrainz disc id.
		/// </summary>
		/// <value>The MusicBrainz disc id.</value>
		public virtual string MusicBrainzDiscId { get; set; }

		/// <summary>
		/// Gets or sets the music ip id.
		/// </summary>
		/// <value>The music ip id.</value>
		public virtual string MusicIpId { get; set; }

		/// <summary>
		/// Gets or sets the Amazon id.
		/// </summary>
		/// <value>The Amazon id.</value>
		public virtual string AmazonId { get; set; }

		/// <summary>
		/// Gets or sets the MusicBrainz release status.
		/// </summary>
		/// <value>The MusicBrainz release status.</value>
		public virtual string MusicBrainzReleaseStatus { get; set; }

		/// <summary>
		/// Gets or sets the MusicBrainz release type.
		/// </summary>
		/// <value>The MusicBrainz release type.</value>
		public virtual string MusicBrainzReleaseType { get; set; }

		/// <summary>
		/// Gets or sets the MusicBrainz release country.
		/// </summary>
		/// <value>The MusicBrainz release country.</value>
		public virtual string MusicBrainzReleaseCountry { get; set; }

		/// <summary>
		/// Gets or sets the pictures.
		/// </summary>
		/// <value>The pictures.</value>
		public virtual IPicture[] Pictures { get; set; }

		/// <summary>
		/// Gets or sets the artists.
		/// </summary>
		/// <value>The artists.</value>
		public virtual string[] Artists { get; set; }

		/// <summary>
		/// Gets or sets the first artist.
		/// </summary>
		/// <value>The first artist.</value>
		public string FirstArtist { get; set; }

		/// <summary>
		/// Gets or sets the first album artist.
		/// </summary>
		/// <value>The first album artist.</value>
		public string FirstAlbumArtist { get; set; }

		/// <summary>
		/// Gets or sets the first album artist sort.
		/// </summary>
		/// <value>The first album artist sort.</value>
		public string FirstAlbumArtistSort { get; set; }

		/// <summary>
		/// Gets or sets the first performer.
		/// </summary>
		/// <value>The first performer.</value>
		public string FirstPerformer { get; set; }

		/// <summary>
		/// Gets or sets the first performer sort.
		/// </summary>
		/// <value>The first performer sort.</value>
		public string FirstPerformerSort { get; set; }

		/// <summary>
		/// Gets or sets the first composer sort.
		/// </summary>
		/// <value>The first composer sort.</value>
		public string FirstComposerSort { get; set; }

		/// <summary>
		/// Gets or sets the first composer.
		/// </summary>
		/// <value>The first composer.</value>
		public string FirstComposer { get; set; }

		/// <summary>
		/// Gets or sets the first genre.
		/// </summary>
		/// <value>The first genre.</value>
		public string FirstGenre { get; set; }

		/// <summary>
		/// Gets or sets the joined artists.
		/// </summary>
		/// <value>The joined artists.</value>
		public string JoinedArtists { get; set; }

		/// <summary>
		/// Gets or sets the joined album artists.
		/// </summary>
		/// <value>The joined album artists.</value>
		public string JoinedAlbumArtists { get; set; }

		/// <summary>
		/// Gets or sets the joined performers.
		/// </summary>
		/// <value>The joined performers.</value>
		public string JoinedPerformers { get; set; }

		/// <summary>
		/// Gets or sets the joined performers sort.
		/// </summary>
		/// <value>The joined performers sort.</value>
		public string JoinedPerformersSort { get; set; }

		/// <summary>
		/// Gets or sets the joined composers.
		/// </summary>
		/// <value>The joined composers.</value>
		public string JoinedComposers { get; set; }

		/// <summary>
		/// Gets or sets the MusicBrainz release id.
		/// </summary>
		/// <value>The MusicBrainz release id.</value>
		public virtual string MusicBrainzReleaseId { get; set; }

		/// <summary>
		/// Gets or sets the chain.
		/// </summary>
		/// <value>The chain.</value>
		public virtual string MusicBrainzArtistId { get; set; }

		/// <summary>
		/// Gets or sets the copyright.
		/// </summary>
		/// <value>The copyright.</value>
		public virtual string Copyright { get; set; }

		/// <summary>
		/// Gets or sets the conductor.
		/// </summary>
		/// <value>The conductor.</value>
		public virtual string Conductor { get; set; }

		/// <summary>
		/// Gets or sets the tag types.
		/// </summary>
		/// <value>The tag types.</value>
		public TagTypes TagTypes { get; set; }

		/// <summary>
		/// Gets or sets the title.
		/// </summary>
		/// <value>The title.</value>
		public virtual string Title { get; set; }

		/// <summary>
		/// Gets or sets the title sort.
		/// </summary>
		/// <value>The title sort.</value>
		public virtual string TitleSort { get; set; }

		/// <summary>
		/// Gets or sets the performers.
		/// </summary>
		/// <value>The performers.</value>
		public virtual string[] Performers { get; set; }

		/// <summary>
		/// Gets or sets the performers sort.
		/// </summary>
		/// <value>The performers sort.</value>
		public virtual string[] PerformersSort { get; set; }

		/// <summary>
		/// Gets or sets the album artists.
		/// </summary>
		/// <value>The album artists.</value>
		public virtual string[] AlbumArtists { get; set; }

		/// <summary>
		/// Gets or sets the album artists sort.
		/// </summary>
		/// <value>The album artists sort.</value>
		public virtual string[] AlbumArtistsSort { get; set; }

		/// <summary>
		/// Gets or sets the composers.
		/// </summary>
		/// <value>The composers.</value>
		public virtual string[] Composers { get; set; }

		/// <summary>
		/// Gets or sets the joined geners.
		/// </summary>
		/// <value>The joined geners.</value>
		public string JoinedGenres { get; set; }

		/// <summary>
		/// Gets or sets the album.
		/// </summary>
		/// <value>The album.</value>
		public virtual string Album { get; set; }

		/// <summary>
		/// Gets or sets the composers sort.
		/// </summary>
		/// <value>The composers sort.</value>
		public virtual string[] ComposersSort { get; set; }

		/// <summary>
		/// Gets or sets the comment.
		/// </summary>
		/// <value>The comment.</value>
		public virtual string Comment { get; set; }

		/// <summary>
		/// Gets or sets the genres.
		/// </summary>
		/// <value>The genres.</value>
		public virtual string[] Genres { get; set; }

		/// <summary>
		/// Gets or sets the year.
		/// </summary>
		/// <value>The year.</value>
		public virtual uint Year { get; set; }

		/// <summary>
		/// Gets or sets the track.
		/// </summary>
		/// <value>The track.</value>
		public virtual uint Track { get; set; }

		/// <summary>
		/// Gets or sets the track count.
		/// </summary>
		/// <value>The track count.</value>
		public virtual uint TrackCount { get; set; }

		/// <summary>
		/// Gets or sets the disc.
		/// </summary>
		/// <value>The disc.</value>
		public virtual uint Disc { get; set; }

		/// <summary>
		/// Gets or sets the disc count.
		/// </summary>
		/// <value>The disc count.</value>
		public virtual uint DiscCount { get; set; }

		/// <summary>
		/// Gets or sets the lyrics.
		/// </summary>
		/// <value>The lyrics.</value>
		public virtual string Lyrics { get; set; }

		/// <summary>
		/// Gets or sets the grouping.
		/// </summary>
		/// <value>The grouping.</value>
		public virtual string Grouping { get; set; }

		/// <summary>
		/// Gets or sets the beats per minute.
		/// </summary>
		/// <value>The beats per minute.</value>
		public virtual uint BeatsPerMinute { get; set; }

		/// <summary>
		/// Gets or sets the album sort.
		/// </summary>
		/// <value>The album sort.</value>
		public virtual string AlbumSort { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether object is empty.
		/// </summary>
		/// <value>A value indicating whether object is empty.</value>
		public virtual bool IsEmpty { get; set; }
	}
}
