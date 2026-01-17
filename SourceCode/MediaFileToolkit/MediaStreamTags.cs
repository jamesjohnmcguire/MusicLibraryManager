/////////////////////////////////////////////////////////////////////////////
// <copyright file="MediaStreamTags.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace MediaFileToolkit;

using Newtonsoft.Json;

/// <summary>
/// Tags for a media stream.
/// </summary>
public class MediaStreamTags
{
	/// <summary>
	/// Gets or sets creation time property.
	/// </summary>
	[JsonProperty("creation_time")]
	public string? CreationTime { get; set; }

	/// <summary>
	/// Gets or sets handler name property.
	/// </summary>
	[JsonProperty("handler_name")]
	public string? HandlerName { get; set; }

	/// <summary>
	/// Gets or sets language property.
	/// </summary>
	[JsonProperty("language")]
	public string? Language { get; set; }

	/// <summary>
	/// Gets or sets title property.
	/// </summary>
	[JsonProperty("title")]
	public string? Title { get; set; }
}
