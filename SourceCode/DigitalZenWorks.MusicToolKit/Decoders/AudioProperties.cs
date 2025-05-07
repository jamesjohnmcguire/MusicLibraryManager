/////////////////////////////////////////////////////////////////////////////
// <copyright file="AudioProperties.cs" company="Digital Zen Works">
// Copyright © 2019 - 2025 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MusicToolKit
{
	/// <summary>
	/// Represents a set of common audio file properties.
	/// </summary>
	public class AudioProperties
	{
		private readonly int channels;

		/// <summary>
		/// Initializes a new instance of the <see cref="AudioProperties"/> class.
		/// </summary>
		/// <param name="sampleRate">The sample rate.</param>
		/// <param name="bitsPerSample">The bits per sample.</param>
		/// <param name="channels">The channels.</param>
		/// <param name="totalTime">The total time.</param>
		public AudioProperties(
			int sampleRate, int bitsPerSample, int channels, int totalTime)
		{
			this.channels = channels;
		}

		/// <summary>
		/// Gets the channels property.
		/// </summary>
		/// <value>The channels property.</value>
		public int Channels
		{
			get { return channels; }
		}
	}
}
