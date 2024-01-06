/////////////////////////////////////////////////////////////////////////////
// <copyright file="NAudioDecoder.cs" company="Digital Zen Works">
// Copyright © 2019 - 2024 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////
using NAudio.Wave;
using System;
using System.Buffers;
using System.IO;

namespace DigitalZenWorks.MusicToolKit
{
	/// <summary>
	/// Decode using the NAudio library.
	/// </summary>
	public class NAudioDecoder : AudioDecoder
	{
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
				remaining = maxLength * this.Format.Channels * SampleRate;

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
			string extension = Path.GetExtension(file);
			extension = extension.ToUpperInvariant();
			WaveStream waveStream = extension switch
			{
				".MP3" => new Mp3FileReader(file),
				".WAV" => new WaveFileReader(file),
				_ => new MediaFoundationReader(file),
			};
			return waveStream;
		}

		private bool Initialize()
		{
			using WaveStream reader = OpenWaveStream(file);

			var format = reader.WaveFormat;

			SampleRate = format.SampleRate;
			Channels = format.Channels;

			this.Format = new AudioProperties(
				format.SampleRate,
				format.BitsPerSample,
				format.Channels,
				(int)reader.TotalTime.TotalSeconds);

			return format.BitsPerSample != 16;
		}
	}
}
