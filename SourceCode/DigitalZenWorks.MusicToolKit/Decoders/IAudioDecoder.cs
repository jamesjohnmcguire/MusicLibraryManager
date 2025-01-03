/////////////////////////////////////////////////////////////////////////////
// <copyright file="IAudioDecoder.cs" company="Digital Zen Works">
// Copyright © 2019 - 2025 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using NAudio.CoreAudioApi;
using System;

namespace DigitalZenWorks.MusicToolKit
{
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
