/////////////////////////////////////////////////////////////////////////////
// <copyright file="MediaInfo.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace MediaFileToolkit;

using Newtonsoft.Json;
using System.Collections.ObjectModel;

public class MediaInfo
{
	/// <summary>
	/// Gets or sets the format property.
	/// </summary>
	public MediaFormat? Format { get; set; }

	/// <summary>
	/// Gets or sets the programs property.
	/// </summary>
	public Collection<object> Programs { get; set; } = new();

	/// <summary>
	/// Gets or sets the stream groups property.
	/// </summary>
	[JsonProperty("stream_groups")]
	public Collection<object> StreamGroups { get; set; } = new();

	/// <summary>
	/// Gets or sets the streams property.
	/// </summary>
	public Collection<MediaStream> Streams { get; set; } = new();
}
