/////////////////////////////////////////////////////////////////////////////
// <copyright file="IAudioConsumer.cs" company="Digital Zen Works">
// Copyright © 2019 - 2025 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MusicToolKit.Decoders
{
	/// <summary>
	/// Consumer for 16bit audio data buffer.
	/// </summary>
	public interface IAudioConsumer
	{
		/// <summary>
		/// Consume audio data.
		/// </summary>
		/// <param name="input">The audio data.</param>
		/// <param name="length">The number of samples to consume.</param>
		void Consume(short[] input, int length);
	}
}
