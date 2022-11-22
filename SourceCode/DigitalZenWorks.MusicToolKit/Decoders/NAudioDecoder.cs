/////////////////////////////////////////////////////////////////////////////
// <copyright file="NAudioDecoder.cs" company="Digital Zen Works">
// Copyright © 2019 - 2022 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MusicToolKit
{
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

		/// <summary>
		/// Generate Chromaprint signature.
		/// </summary>
		/// <returns>A Chromaprint signature.</returns>
		public unsafe string GenerateChromaPrint()
		{
			string fingerPrint = null;

			Assembly assembly = Assembly.GetEntryAssembly();
			string location = assembly.Location;
			string fullPath = System.IO.Path.GetDirectoryName(location);

			assembly = Assembly.GetExecutingAssembly();
			location = assembly.Location;
			fullPath = System.IO.Path.GetDirectoryName(location);

			ChromaprintContext context = NativeMethods.chromaprint_new(1);

			int result = NativeMethods.chromaprint_start(
				context, sampleRate, channels);

			if (result > 0)
			{
				using WaveStream reader = OpenWaveStream(file);

				try
				{
					double maxDuration = 120;
					double maxChunkDuration = 0;
					long streamSize = 0;
					double streamLimit = maxDuration * sampleRate;

					long chunkSize = 0;
					double chunkLimit = maxChunkDuration * sampleRate;

					long extraChunkLimit = 0;
					double overlap = 0.0;

					bool firstChunk = true;
					bool finished = false;

					int bufferSize = 230;
					int totalRead = 0;

					while (finished == false)
					{
						byte[] buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
						int actualRead = reader.Read(buffer, 0, bufferSize);

						bool streamDone = false;
						if (streamLimit > 0)
						{
							double remaining = streamLimit - streamSize;
							if (actualRead > remaining)
							{
								bufferSize = (int)remaining;
								streamDone = true;
							}
						}

						streamSize += actualRead;
						totalRead += actualRead;

						// TODO: Refactor into something easier
						if (actualRead == 0)
						{
							if (streamDone)
							{
								break;
							}
							else
							{
								continue;
							}
						}

						bool chunkDone = false;
						int firstPartSize = actualRead;

						if (chunkLimit > 0)
						{
							double remaining =
								chunkLimit + extraChunkLimit - chunkSize;

							if (firstPartSize > remaining)
							{
								firstPartSize = (int)remaining;
								chunkDone = true;
							}
						}

						int thisSize = firstPartSize * channels;
						thisSize = actualRead;

						fixed (byte* bytePointer = &buffer[0])
						{
							IntPtr bufferPointer = (IntPtr)bytePointer;
							result = NativeMethods.chromaprint_feed(
								context, (IntPtr)bytePointer, thisSize);
						}

						if (result > 0)
						{
							chunkSize += firstPartSize;

							if (chunkDone)
							{
								result = NativeMethods.chromaprint_finish(context);

								if (result == 0)
								{
									return null;
								}

								var innerChunkSize = chunkSize - extraChunkLimit;
								double innerChunkSizeD = innerChunkSize * 1.0;
								var chunkDuration = (innerChunkSizeD / sampleRate) + overlap;
								chunkDuration = chunkDuration + overlap;

								if (overlap > 0)
								{
									result =
										NativeMethods.chromaprint_clear_fingerprint(
											context);

									if (result == 0)
									{
										return null;
									}
								}
								else
								{
									result =
										NativeMethods.chromaprint_start(
											context, SampleRate, channels);

									if (result == 0)
									{
										return null;
									}
								}

								if (firstChunk)
								{
									extraChunkLimit = 0;
									firstChunk = false;
								}

								chunkSize = 0;
							}
						}

						int newFrameSize = actualRead - firstPartSize;
						if (newFrameSize > 0)
						{
							thisSize = firstPartSize * channels;
							int offset = firstPartSize * channels;

							fixed (byte* bytePointer = buffer)
							{
								IntPtr bufferPointer = (IntPtr)bytePointer;
								bufferPointer += offset;

								result = NativeMethods.chromaprint_feed(
									context, bufferPointer, thisSize);
							}

							if (result == 0)
							{
								return null;
							}
						}

						chunkSize += newFrameSize;

						if (actualRead < bufferSize || streamDone == true)
						{
							finished = true;
						}

						bufferSize = 256;
					}

					result = NativeMethods.chromaprint_finish(context);
				}
				catch (Exception exception) when
					(exception is ArgumentException ||
					exception is ArgumentNullException ||
					exception is ArgumentOutOfRangeException ||
					exception is IOException ||
					exception is NotSupportedException ||
					exception is ObjectDisposedException)
				{
				}
			}

			IntPtr outBuffer;

			result = NativeMethods.chromaprint_get_fingerprint(
				context, out outBuffer);
			fingerPrint = Marshal.PtrToStringAnsi(outBuffer);
			return fingerPrint;
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
