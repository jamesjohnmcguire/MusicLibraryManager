/////////////////////////////////////////////////////////////////////////////
// <copyright file="AudioProperties.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MusicToolKit.Decoders;

/// <summary>
/// Represents a set of common audio file properties.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="AudioProperties"/> class.
/// </remarks>
/// <param name="sampleRate">The sample rate.</param>
/// <param name="bitsPerSample">The bits per sample.</param>
/// <param name="channels">The channels.</param>
/// <param name="totalTime">The total time.</param>
public class AudioProperties(
	int sampleRate, int bitsPerSample, int channels, int totalTime)
{
	private readonly int channels = channels;

	/// <summary>
	/// Gets the channels property.
	/// </summary>
	/// <value>The channels property.</value>
	public int Channels
	{
		get { return channels; }
	}
}
