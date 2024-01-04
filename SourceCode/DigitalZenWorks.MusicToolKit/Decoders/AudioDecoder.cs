/////////////////////////////////////////////////////////////////////////////
// <copyright file="AudioDecoder.cs" company="Digital Zen Works">
// Copyright © 2019 - 2024 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System;

namespace DigitalZenWorks.MusicToolKit
{
	/// <summary>
	/// Abstract base class for audio decoders.
	/// </summary>
	public abstract class AudioDecoder : IAudioDecoder
	{
		/// <summary>
		/// Buffer size field.
		/// </summary>
		protected static readonly int BUFFERSIZE = 2 * 192000;

		/// <summary>
		/// Finalizes an instance of the <see cref="AudioDecoder"/> class.
		/// Destructor.
		/// </summary>
		~AudioDecoder() => Dispose(false);

		/// <summary>
		/// Gets or sets the sample rate.
		/// </summary>
		/// <value>The sample rate.</value>
		public int SampleRate { get; set; }

		/// <summary>
		/// Gets or sets the channels property.
		/// </summary>
		/// <value>The channels property.</value>
		public int Channels { get; set; }

		/// <summary>
		/// Gets or sets the format property.
		/// </summary>
		/// <value>The format property.</value>
		public AudioProperties Format { get; protected set; }

		/// <summary>
		/// The decode method.
		/// </summary>
		/// <param name="consumer">The consumer.</param>
		/// <param name="maxLength">The maximum length.</param>
		/// <returns>A valude indicating success or not.</returns>
		public abstract bool Decode(IAudioConsumer consumer, int maxLength);

		/// <summary>
		/// The dispose method.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// The dispose method.
		/// </summary>
		/// <param name="disposing">A value indicating disposing
		/// or not.</param>
		protected virtual void Dispose(bool disposing)
		{
		}
	}
}
