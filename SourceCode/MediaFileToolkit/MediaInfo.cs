/////////////////////////////////////////////////////////////////////////////
// <copyright file="MediaInfo.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace MediaFileToolkit;

using Newtonsoft.Json;
using System.Collections.ObjectModel;

/// <summary>
/// The media information DTO class.
/// </summary>
public class MediaInfo
{
	/// <summary>
	/// Gets or sets the format property.
	/// </summary>
	public MediaFormat? Format { get; set; }

	/// <summary>
	/// Gets or sets the programs property.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Usage",
		"CA2227:Collection properties should be read only",
		Justification = "It's necessary to be set, as it is a DTO object.")]
	public Collection<object>? Programs { get; set; }

	/// <summary>
	/// Gets or sets the stream groups property.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Usage",
		"CA2227:Collection properties should be read only",
		Justification = "It's necessary to be set, as it is a DTO object.")]
	[JsonProperty("stream_groups")]
	public Collection<object>? StreamGroups { get; set; }

	/// <summary>
	/// Gets or sets the streams property.
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Usage",
		"CA2227:Collection properties should be read only",
		Justification = "It's necessary to be set, as it is a DTO object.")]
	public Collection<MediaStreamProperties>? Streams { get; set; }
}
