/////////////////////////////////////////////////////////////////////////////
// <copyright file="AudioConsumer.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MusicToolKit.Decoders;

/// <summary>
/// Represents an audio consumer.
/// </summary>
public class AudioConsumer : IAudioConsumer
{
	/// <summary>
	/// Consume audio data.
	/// </summary>
	/// <param name="input">The audio data.</param>
	/// <param name="length">The number of samples to consume.</param>
	public void Consume(short[] input, int length)
	{
	}
}
