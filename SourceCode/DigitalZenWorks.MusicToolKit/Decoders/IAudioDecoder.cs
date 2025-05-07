/////////////////////////////////////////////////////////////////////////////
// <copyright file="IAudioDecoder.cs" company="Digital Zen Works">
// Copyright © 2019 - 2025 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MusicToolKit.Decoders
{
	using System;
	using NAudio.CoreAudioApi;

	/// <summary>
	/// Interface for audio decoders.
	/// </summary>
	public interface IAudioDecoder : IDecoder, IDisposable
	{
		/// <summary>
		/// Gets the format property.
		/// </summary>
		/// <value>The format property.</value>
		AudioProperties Format { get; }
	}
}
