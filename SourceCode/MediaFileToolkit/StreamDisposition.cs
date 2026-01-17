/////////////////////////////////////////////////////////////////////////////
// <copyright file="StreamDisposition.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace MediaFileToolkit;

using Newtonsoft.Json;

public class StreamDisposition
{
	/// <summary>
	/// Gets or sets attached picture property.
	/// </summary>
	[JsonProperty("attached_pic")]
	public int AttachedPicture { get; set; }

	/// <summary>
	/// Gets or sets clean effects property.
	/// </summary>
	[JsonProperty("clean_effects")]
	public int CleanEffects { get; set; }

	/// <summary>
	/// Gets or sets comment property.
	/// </summary>
	[JsonProperty("comment")]
	public int Comment { get; set; }

	/// <summary>
	/// Gets or sets default property.
	/// </summary>
	[JsonProperty("default")]
	public int Default { get; set; }

	/// <summary>
	/// Gets or sets dub property.
	/// </summary>
	[JsonProperty("dub")]
	public int Dub { get; set; }

	/// <summary>
	/// Gets or sets forced property.
	/// </summary>
	[JsonProperty("forced")]
	public int Forced { get; set; }

	/// <summary>
	/// Gets or sets hearing impaired property.
	/// </summary>
	[JsonProperty("hearing_impaired")]
	public int HearingImpaired { get; set; }

	/// <summary>
	/// Gets or sets karaoke property.
	/// </summary>
	[JsonProperty("karaoke")]
	public int Karaoke { get; set; }

	/// <summary>
	/// Gets or sets lyrics property.
	/// </summary>
	[JsonProperty("lyrics")]
	public int Lyrics { get; set; }

	/// <summary>
	/// Gets or sets original property.
	/// </summary>
	[JsonProperty("original")]
	public int Original { get; set; }

	/// <summary>
	/// Gets or sets visual impaired property.
	/// </summary>
	[JsonProperty("visual_impaired")]
	public int VisualImpaired { get; set; }
}
