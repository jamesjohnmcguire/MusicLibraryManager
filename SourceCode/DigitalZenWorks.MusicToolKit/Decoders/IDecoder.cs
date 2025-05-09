﻿/////////////////////////////////////////////////////////////////////////////
// <copyright file="IDecoder.cs" company="Digital Zen Works">
// Copyright © 2019 - 2025 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MusicToolKit
{
	/// <summary>
	/// Interface for audio decoders.
	/// </summary>
	public interface IDecoder
	{
		/// <summary>
		/// Gets the sample rate of the audio sent to the audio signature.
		/// </summary>
		/// <remarks>
		/// May be different from the source audio sample rate, if the
		/// decoder does resampling.
		/// </remarks>
		/// <value>The sample rate of the audio sent to the audio
		/// signature.</value>
		int SampleRate { get; }

		/// <summary>
		/// Gets the channel count of the audio sent to the audio signature.
		/// </summary>
		/// <remarks>
		/// May be different from the source audio channel count.
		/// </remarks>
		/// <value>The channel count of the audio sent to the audio
		/// signature.</value>
		int Channels { get; }

		/// <summary>
		/// Decode audio file.
		/// </summary>
		/// <param name="consumer">The <see cref="IAudioConsumer"/> that consumes the decoded audio.</param>
		/// <param name="maxLength">The number of seconds to decode.</param>
		/// <returns>Returns true, if decoding was successful.</returns>
		bool Decode(IAudioConsumer consumer, int maxLength);
	}
}
