/////////////////////////////////////////////////////////////////////////////
// <copyright file="AudioDecoder.cs" company="Digital Zen Works">
// Copyright © 2019 - 2023 Digital Zen Works. All Rights Reserved.
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
		/// The sample rate.
		/// </summary>
		protected int sampleRate;

		/// <summary>
		/// The channels property.
		/// </summary>
		protected int channels;

		/// <summary>
		/// Finalizes an instance of the <see cref="AudioDecoder"/> class.
		/// Destructor.
		/// </summary>
		~AudioDecoder() => Dispose(false);

		/// <summary>
		/// Gets the sample rate.
		/// </summary>
		/// <value>The sample rate.</value>
		public int SampleRate
		{
			get { return sampleRate; }
		}

		/// <summary>
		/// Gets the channels property.
		/// </summary>
		/// <value>The channels property.v</value>
		public int Channels
		{
			get { return channels; }
		}

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
		public virtual void Dispose()
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
