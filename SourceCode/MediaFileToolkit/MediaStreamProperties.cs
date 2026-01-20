/////////////////////////////////////////////////////////////////////////////
// <copyright file="MediaStreamProperties.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace MediaFileToolkit;

using Newtonsoft.Json;

/// <summary>
/// Represents a media stream with various properties such as codec type.
/// </summary>
public class MediaStreamProperties
{
	/// <summary>
	/// Gets or sets the average frame rate property.
	/// </summary>
	[JsonProperty("avg_frame_rate")]
	public string? AverageFrameRate { get; set; }

	/// <summary>
	/// Gets or sets the bits per raw sample property.
	/// </summary>
	[JsonProperty("bits_per_raw_sample")]
	public string? BitsPerRawSample { get; set; }

	/// <summary>
	/// Gets or sets the bits per sample property.
	/// </summary>
	[JsonProperty("bits_per_sample")]
	public int? BitsPerSample { get; set; }

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
	/// Gets or sets the channel layout property.
	/// </summary>
	[JsonProperty("channel_layout")]
	public string? ChannelLayout { get; set; }

	/// <summary>
	/// Gets or sets the channels property.
	/// </summary>
	[JsonProperty("channels")]
	public int? Channels { get; set; }

	/// <summary>
	/// Gets or sets the codec long name property.
	/// </summary>
	[JsonProperty("codec_long_name")]
	public string? CodecLongName { get; set; }

	/// <summary>
	/// Gets or sets the codec name property.
	/// </summary>
	[JsonProperty("codec_name")]
	public string? CodecName { get; set; }

	/// <summary>
	/// Gets or sets the codec tag property.
	/// </summary>
	[JsonProperty("codec_tag")]
	public string? CodecTag { get; set; }

	/// <summary>
	/// Gets or sets the codec tag string property.
	/// </summary>
	[JsonProperty("codec_tag_string")]
	public string? CodecTagString { get; set; }

	/// <summary>
	/// Gets or sets the codec_type property.
	/// </summary>
	[JsonProperty("codec_type")]
	public string? CodecType { get; set; }

	/// <summary>
	/// Gets or sets the display aspect ratio property.
	/// </summary>
	[JsonProperty("display_aspect_ratio")]
	public string? DisplayAspectRatio { get; set; }

	/// <summary>
	/// Gets or sets the disposition property.
	/// </summary>
	// Disposition flags
	[JsonProperty("disposition")]
	public StreamDisposition? Disposition { get; set; }

	/// <summary>
	/// Gets or sets the duration property.
	/// </summary>
	[JsonProperty("duration")]
	public string? Duration { get; set; }

	/// <summary>
	/// Gets the duration numeric property.
	/// </summary>
	[JsonIgnore]
	public double? DurationNumeric =>
		double.TryParse(Duration, out var result) ? result : null;

	/// <summary>
	/// Gets or sets the duration ts property.
	/// </summary>
	[JsonProperty("duration_ts")]
	public long? DurationTs { get; set; }

	/// <summary>
	/// Gets or sets the frame count property.
	/// </summary>
	[JsonProperty("nb_frames")]
	public string? FrameCount { get; set; }

	/// <summary>
	/// Gets or sets the frame rate property.
	/// </summary>
	[JsonProperty("r_frame_rate")]
	public string? FrameRate { get; set; }

	/// <summary>
	/// Gets the frame rate numeric property.
	/// </summary>
	[JsonIgnore]
	public double? FrameRateNumeric
	{
		get
		{
			if (string.IsNullOrEmpty(FrameRate))
			{
				return null;
			}

			var parts = FrameRate.Split('/');
			if (parts.Length == 2 &&
				double.TryParse(parts[0], out var numerator) &&
				double.TryParse(parts[1], out var denominator) &&
				denominator != 0)
			{
				return numerator / denominator;
			}

			return double.TryParse(FrameRate, out var result) ? result : null;
		}
	}

	/// <summary>
	/// Gets or sets the height property.
	/// </summary>
	[JsonProperty("height")]
	public int? Height { get; set; }

	/// <summary>
	/// Gets or sets the index property.
	/// </summary>
	// Stream metadata
	[JsonProperty("index")]
	public int Index { get; set; }

	/// <summary>
	/// Gets or sets the level property.
	/// </summary>
	[JsonProperty("level")]
	public int? Level { get; set; }

	/// <summary>
	/// Gets or sets the max bit rate property.
	/// </summary>
	[JsonProperty("max_bit_rate")]
	public string? MaxBitRate { get; set; }

	/// <summary>
	/// Gets or sets the pixel format property.
	/// </summary>
	[JsonProperty("pix_fmt")]
	public string? PixelFormat { get; set; }

	/// <summary>
	/// Gets or sets the profile property.
	/// </summary>
	[JsonProperty("profile")]
	public string? Profile { get; set; }

	/// <summary>
	/// Gets or sets the sample aspect ratio property.
	/// </summary>
	[JsonProperty("sample_aspect_ratio")]
	public string? SampleAspectRatio { get; set; }

	/// <summary>
	/// Gets or sets the sample format property.
	/// </summary>
	[JsonProperty("sample_fmt")]
	public string? SampleFormat { get; set; }

	/// <summary>
	/// Gets or sets the sample rate property.
	/// </summary>
	[JsonProperty("sample_rate")]
	public string? SampleRate { get; set; }

	/// <summary>
	/// Gets the sample rate numeric property.
	/// </summary>
	[JsonIgnore]
	public int? SampleRateNumeric =>
		int.TryParse(SampleRate, out var result) ? result : null;

	/// <summary>
	/// Gets or sets the start pts property.
	/// </summary>
	[JsonProperty("start_pts")]
	public long? StartPts { get; set; }

	/// <summary>
	/// Gets or sets the start time property.
	/// </summary>
	[JsonProperty("start_time")]
	public string? StartTime { get; set; }

	/// <summary>
	/// Gets or sets the tags property.
	/// </summary>
	[JsonProperty("tags")]
	public MediaStreamTags? Tags { get; set; }

	/// <summary>
	/// Gets or sets the time base property.
	/// </summary>
	[JsonProperty("time_base")]
	public string? TimeBase { get; set; }

	/// <summary>
	/// Gets or sets the width property.
	/// </summary>
	[JsonProperty("width")]
	public int? Width { get; set; }
}
