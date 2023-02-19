﻿/////////////////////////////////////////////////////////////////////////////
// <copyright file="NAudioDecoder.cs" company="Digital Zen Works">
// Copyright © 2019 - 2023 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MusicToolKit
{
	using Common.Logging;
	using NAudio.Wave;
	using System;
	using System.Buffers;
	using System.IO;
	using System.Reflection;
	using System.Runtime.InteropServices;

	/// <summary>
	/// Decode using the NAudio library.
	/// </summary>
	public class NAudioDecoder : AudioDecoder
	{
		private static readonly ILog Log = LogManager.GetLogger(
			MethodBase.GetCurrentMethod().DeclaringType);

		private readonly string file;

		/// <summary>
		/// Initializes a new instance of the <see cref="NAudioDecoder"/> class.
		/// </summary>
		/// <param name="file">The file to decode.</param>
		public NAudioDecoder(string file)
		{
			this.file = file;

			Initialize();
		}

		/// <summary>
		/// The decode method.
		/// </summary>
		/// <param name="consumer">The consumer.</param>
		/// <param name="maxLength">The maximum length.</param>
		/// <returns>A value indicating success or not.</returns>
		public override bool Decode(IAudioConsumer consumer, int maxLength)
		{
			bool result = false;

			if (consumer != null)
			{
				using WaveStream reader = OpenWaveStream(file);

				if (reader.WaveFormat.BitsPerSample != 16)
				{
					return false;
				}

				int remaining, length, size;

				var buffer = ArrayPool<byte>.Shared.Rent(2 * BUFFERSIZE);
				var data = ArrayPool<short>.Shared.Rent(BUFFERSIZE);

				// Samples to read to get maxLength seconds of audio
				remaining = maxLength * this.Format.Channels * this.sampleRate;

				// Bytes to read
				length = 2 * Math.Min(remaining, BUFFERSIZE);

				while ((size = reader.Read(buffer, 0, length)) > 0)
				{
					Buffer.BlockCopy(buffer, 0, data, 0, size);

					consumer.Consume(data, size / 2);

					remaining -= size / 2;
					if (remaining <= 0)
					{
						break;
					}

					length = 2 * Math.Min(remaining, BUFFERSIZE);
				}

				ArrayPool<byte>.Shared.Return(buffer);
				ArrayPool<short>.Shared.Return(data);

				result = true;
			}

			return result;
		}

		private static WaveStream OpenWaveStream(string file)
		{
			WaveStream waveStream;

			string extension = Path.GetExtension(file);
			extension = extension.ToUpperInvariant();

			switch (extension)
			{
				case ".MP3":
					waveStream = new Mp3FileReader(file);
					break;
				case ".WAV":
					waveStream = new WaveFileReader(file);
					break;
				default:
					waveStream = new MediaFoundationReader(file);
					break;
			}

			return waveStream;
		}

		private bool Initialize()
		{
			using WaveStream reader = OpenWaveStream(file);

			var format = reader.WaveFormat;

			this.sampleRate = format.SampleRate;
			this.channels = format.Channels;

			this.Format = new AudioProperties(
				format.SampleRate,
				format.BitsPerSample,
				format.Channels,
				(int)reader.TotalTime.TotalSeconds);

			return format.BitsPerSample != 16;
		}
	}
}
