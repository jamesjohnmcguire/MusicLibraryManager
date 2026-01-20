/////////////////////////////////////////////////////////////////////////////
// <copyright file="MediaFormat.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace MediaFileToolkit;

using Newtonsoft.Json;

/// <summary>
/// Represents the format information of a media file.
/// </summary>
public class MediaFormat
{
	/// <summary>
	/// Gets or sets the bit rate property.
	/// </summary>
	[JsonProperty("bit_rate")]
	public string? BitRate { get; set; }

	/// <summary>
	/// Gets the bit rate numeric property.
	/// </summary>
	[JsonIgnore]
	public long? BitRateNumeric =>
		long.TryParse(BitRate, out var result) ? result : null;

	/// <summary>
	/// Gets or sets the duration property.
	/// </summary>
	public string? Duration { get; set; }

	/// <summary>
	/// Gets the duration numeric property.
	/// </summary>
	[JsonIgnore]
	public double? DurationNumeric =>
		double.TryParse(Duration, out var result) ? result : null;

	/// <summary>
	/// Gets or sets the filename property.
	/// </summary>
	public string? Filename { get; set; }

	/// <summary>
	/// Gets or sets the format long name property.
	/// </summary>
	[JsonProperty("format_long_name")]
	public string? FormatLongName { get; set; }

	/// <summary>
	/// Gets or sets the format name property.
	/// </summary>
	[JsonProperty("format_name")]
	public string? FormatName { get; set; }

	/// <summary>
	/// Gets or sets the size property.
	/// </summary>
	public string? Size { get; set; }
}
