/////////////////////////////////////////////////////////////////////////////
// <copyright file="NativeMethods.cs" company="Digital Zen Works">
// Copyright © 2019 - 2023 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;

[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]

namespace DigitalZenWorks.MusicToolKit
{
	public class NativeMethods
	{
		/// <summary>
		/// Clear the current fingerprint, but allow more data to be processed.
		/// </summary>
		/// <param name="ctx">Chromaprint context pointer.</param>
		/// <returns>0 on error, 1 on success.</returns>
		/// <remarks>
		/// This is useful if you are processing a long stream and want to many
		/// smaller fingerprints, instead of waiting for the entire stream to be
		/// processed.
		/// </remarks>
		[DllImport("chromaprint", EntryPoint = "chromaprint_clear_fingerprint", CallingConvention = CallingConvention.Cdecl)]
		public static extern int chromaprint_clear_fingerprint(ChromaprintContext ctx);

		/// <summary>
		/// Free memory allocated by any function from the Chromaprint API.
		/// </summary>
		/// <param name="ptr">pointer to be deallocated.</param>
		[DllImport("chromaprint", EntryPoint = "chromaprint_dealloc", CallingConvention = CallingConvention.Cdecl)]
		public static extern void chromaprint_dealloc(IntPtr ptr);

		/// <summary>
		/// Uncompress and optionally base64-decode an encoded fingerprint.
		/// </summary>
		/// <param name="encoded_fp">pointer to an encoded fingerprint.</param>
		/// <param name="encoded_size">size of the encoded fingerprint in bytes.</param>
		/// <param name="fp">[out] pointer to a pointer, where the decoded raw fingerprint (array of 32-bit integers) will be stored.</param>
		/// <param name="size">[out] Number of items in the returned raw fingerprint.</param>
		/// <param name="algorithm">[out] Chromaprint algorithm version which was used to generate the raw fingerprint.</param>
		/// <param name="base64">Whether the encoded_fp parameter contains binary data or base64-encoded ASCII data.</param>
		/// <returns>0 on error, 1 on success.</returns>
		/// <remarks>
		/// The caller is responsible for freeing the returned pointer using chromaprint_dealloc().
		/// </remarks>
		[DllImport("chromaprint", EntryPoint = "chromaprint_decode_fingerprint", CallingConvention = CallingConvention.Cdecl)]
		public static extern int chromaprint_decode_fingerprint(IntPtr encoded_fp, int encoded_size, out IntPtr fp, out int size, out int algorithm, int base64);

		/// <summary>
		/// Compress and optionally base64-encode a raw fingerprint.
		/// </summary>
		/// <param name="fp">pointer to an array of 32-bit integers representing the raw fingerprint to be encoded.</param>
		/// <param name="size">number of items in the raw fingerprint.</param>
		/// <param name="algorithm">Chromaprint algorithm version which was used to generate the raw fingerprint.</param>
		/// <param name="encoded_fp">[out] pointer to a pointer, where the encoded fingerprint will be stored.</param>
		/// <param name="encoded_size">[out] size of the encoded fingerprint in bytes.</param>
		/// <param name="base64">Whether to return binary data or base64-encoded ASCII data.</param>
		/// <returns>0 on error, 1 on success.</returns>
		/// <remarks>
		/// The caller is responsible for freeing the returned pointer using chromaprint_dealloc().
		/// </remarks>
		[DllImport("chromaprint", EntryPoint = "chromaprint_encode_fingerprint", CallingConvention = CallingConvention.Cdecl)]
		public static extern int chromaprint_encode_fingerprint(IntPtr fp, int size, int algorithm, out IntPtr encoded_fp, out int encoded_size, int base64);
		/// <summary>
		/// Send audio data to the fingerprint calculator.
		/// </summary>
		/// <param name="ctx">Chromaprint context pointer.</param>
		/// <param name="data">raw audio data, should point to an array of 16-bit signed integers in native byte-order.</param>
		/// <param name="size">size of the data buffer (in samples).</param>
		/// <returns>0 on error, 1 on success.</returns>
		[DllImport("chromaprint", EntryPoint = "chromaprint_feed", CallingConvention = CallingConvention.Cdecl)]
		public static extern int chromaprint_feed(ChromaprintContext ctx, IntPtr data, int size);

		/// <summary>
		/// Process any remaining buffered audio data.
		/// </summary>
		/// <param name="ctx">Chromaprint context pointer.</param>
		/// <returns>0 on error, 1 on success.</returns>
		[DllImport("chromaprint", EntryPoint = "chromaprint_finish", CallingConvention = CallingConvention.Cdecl)]
		public static extern int chromaprint_finish(ChromaprintContext ctx);

		/// <summary>
		/// Deallocate the Chromaprint context.
		/// </summary>
		/// <param name="ctx">Chromaprint context pointer.</param>
		/// <remarks>
		/// Note that when Chromaprint is compiled with FFTW, this function is
		/// not reentrant and you need to call it only from one thread at a time.
		/// This is not a problem when using FFmpeg or vDSP.
		/// </remarks>
		[DllImport("chromaprint", EntryPoint = "chromaprint_free", CallingConvention = CallingConvention.Cdecl)]
		public static extern void chromaprint_free(ChromaprintContext ctx);

		/// <summary>
		/// Return the fingerprint algorithm this context is configured to use.
		/// </summary>
		/// <param name="ctx">Chromaprint context pointer.</param>
		/// <returns>current algorithm version.</returns>
		[DllImport("chromaprint", EntryPoint = "chromaprint_get_algorithm", CallingConvention = CallingConvention.Cdecl)]
		public static extern int chromaprint_get_algorithm(ChromaprintContext ctx);

		/// <summary>
		/// Get the duration of internal buffers that the fingerprinting algorithm uses.
		/// </summary>
		/// <param name="ctx">Chromaprint context pointer.</param>
		/// <returns>duration in samples.</returns>
		[DllImport("chromaprint", EntryPoint = "chromaprint_get_delay", CallingConvention = CallingConvention.Cdecl)]
		public static extern int chromaprint_get_delay(ChromaprintContext ctx);

		/// <summary>
		/// Get the duration of internal buffers that the fingerprinting algorithm uses.
		/// </summary>
		/// <param name="ctx">Chromaprint context pointer.</param>
		/// <returns>duration in milliseconds.</returns>
		[DllImport("chromaprint", EntryPoint = "chromaprint_get_delay_ms", CallingConvention = CallingConvention.Cdecl)]
		public static extern int chromaprint_get_delay_ms(ChromaprintContext ctx);

		/// <summary>
		/// Return the calculated fingerprint as a compressed string.
		/// </summary>
		/// <param name="ctx">Chromaprint context pointer.</param>
		/// <param name="fingerprint">pointer to a pointer, where a pointer to the allocated array will be stored.</param>
		/// <returns>0 on error, 1 on success.</returns>
		/// <remarks>
		/// The caller is responsible for freeing the returned pointer using chromaprint_dealloc().
		/// </remarks>
		[DllImport("chromaprint", EntryPoint = "chromaprint_get_fingerprint", CallingConvention = CallingConvention.Cdecl)]
		public static extern int chromaprint_get_fingerprint(ChromaprintContext ctx, out IntPtr fingerprint);

		/// <summary>
		/// Return 32-bit hash of the calculated fingerprint.
		/// </summary>
		/// <param name="ctx">Chromaprint context pointer.</param>
		/// <param name="hash">pointer to a 32-bit integer where the hash will be stored.</param>
		/// <returns>0 on error, 1 on success.</returns>
		/// <remarks>
		/// See chromaprint_hash_fingerprint() for details on how to use the hash.
		/// </remarks>
		[DllImport("chromaprint", EntryPoint = "chromaprint_get_fingerprint_hash", CallingConvention = CallingConvention.Cdecl)]
		public static extern int chromaprint_get_fingerprint_hash(ChromaprintContext ctx, out int hash);

		/// <summary>
		/// Get the duration of one item in the raw fingerprint in samples.
		/// </summary>
		/// <param name="ctx">Chromaprint context pointer.</param>
		/// <returns>duration in samples.</returns>
		[DllImport("chromaprint", EntryPoint = "chromaprint_get_item_duration", CallingConvention = CallingConvention.Cdecl)]
		public static extern int chromaprint_get_item_duration(ChromaprintContext ctx);

		/// <summary>
		/// Get the duration of one item in the raw fingerprint in milliseconds.
		/// </summary>
		/// <param name="ctx">Chromaprint context pointer.</param>
		/// <returns>duration in milliseconds.</returns>
		[DllImport("chromaprint", EntryPoint = "chromaprint_get_item_duration_ms", CallingConvention = CallingConvention.Cdecl)]
		public static extern int chromaprint_get_item_duration_ms(ChromaprintContext ctx);

		/// <summary>
		/// Get the number of channels that is internally used for fingerprinting.
		/// </summary>
		/// <param name="ctx">Chromaprint context pointer.</param>
		/// <returns>number of channels.</returns>
		/// <remarks>
		/// You normally don't need this. Just set the audio's actual number of channels
		/// when calling chromaprint_start() and everything will work. This is only used for
		/// certain optimized cases to control the audio source.
		/// </remarks>
		[DllImport("chromaprint", EntryPoint = "chromaprint_get_num_channels", CallingConvention = CallingConvention.Cdecl)]
		public static extern int chromaprint_get_num_channels(ChromaprintContext ctx);

		/// <summary>
		/// Return the calculated fingerprint as an array of 32-bit integers.
		/// </summary>
		/// <param name="ctx">Chromaprint context pointer.</param>
		/// <param name="fingerprint">pointer to a pointer, where a pointer to the allocated array will be stored.</param>
		/// <param name="size">number of items in the returned raw fingerprint.</param>
		/// <returns>0 on error, 1 on success.</returns>
		/// <remarks>
		/// The caller is responsible for freeing the returned pointer using chromaprint_dealloc().
		/// </remarks>
		[DllImport("chromaprint", EntryPoint = "chromaprint_get_raw_fingerprint", CallingConvention = CallingConvention.Cdecl)]
		public static extern int chromaprint_get_raw_fingerprint(ChromaprintContext ctx, out IntPtr fingerprint, out int size);

		/// <summary>
		/// Return the length of the current raw fingerprint.
		/// </summary>
		/// <param name="ctx">Chromaprint context pointer.</param>
		/// <param name="size">number of items in the current raw fingerprint.</param>
		/// <returns>0 on error, 1 on success.</returns>
		[DllImport("chromaprint", EntryPoint = "chromaprint_get_raw_fingerprint_size", CallingConvention = CallingConvention.Cdecl)]
		public static extern int chromaprint_get_raw_fingerprint_size(ChromaprintContext ctx, out int size);

		/// <summary>
		/// Get the sampling rate that is internally used for fingerprinting.
		/// </summary>
		/// <param name="ctx">Chromaprint context pointer.</param>
		/// <returns>sampling rate.</returns>
		/// <remarks>
		/// You normally don't need this. Just set the audio's actual number of channels
		/// when calling chromaprint_start() and everything will work. This is only used for
		/// certain optimized cases to control the audio source.
		/// </remarks>
		[DllImport("chromaprint", EntryPoint = "chromaprint_get_sample_rate", CallingConvention = CallingConvention.Cdecl)]
		public static extern int chromaprint_get_sample_rate(ChromaprintContext ctx);

		/// <summary>
		/// Return the version number of Chromaprint.
		/// </summary>
		/// <returns>The Chromaprint version.</returns>
		[DllImport(
			"chromaprint",
			EntryPoint = "chromaprint_get_version",
			CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr chromaprint_get_version();

		/// <summary>
		/// Generate a single 32-bit hash for a raw fingerprint.
		/// </summary>
		/// <param name="fp">pointer to an array of 32-bit integers representing the raw fingerprint to be hashed.</param>
		/// <param name="size">number of items in the raw fingerprint.</param>
		/// <param name="hash">[out] pointer to a 32-bit integer where the hash will be stored.</param>
		/// <returns>0 on error, 1 on success.</returns>
		/// <remarks>
		/// If two fingerprints are similar, their hashes generated by this function
		/// will also be similar. If they are significantly different, their hashes
		/// will most likely be significantly different as well, but you can't rely
		/// on that.
		///
		/// You compare two hashes by counting the bits in which they differ. Normally
		/// that would be something like POPCNT(hash1 XOR hash2), which returns a
		/// number between 0 and 32. Anthing above 15 means the hashes are
		/// completely different.
		/// </remarks>
		[DllImport("chromaprint", EntryPoint = "chromaprint_hash_fingerprint", CallingConvention = CallingConvention.Cdecl)]
		public static extern int chromaprint_hash_fingerprint(IntPtr fp, int size, out int hash);

		/// <summary>
		/// Allocate and initialize the Chromaprint context.
		/// </summary>
		/// <param name="algorithm">the fingerprint algorithm version you want to use, or CHROMAPRINT_ALGORITHM_DEFAULT for the default algorithm.</param>
		/// <returns>Chromaprint context pointer.</returns>
		/// <remarks>
		/// Note that when Chromaprint is compiled with FFTW, this function is
		/// not reentrant and you need to call it only from one thread at a time.
		/// This is not a problem when using FFmpeg or vDSP.
		/// </remarks>
		[DllImport(
			"chromaprint.dll",
			EntryPoint = "chromaprint_new",
			CallingConvention = CallingConvention.Cdecl)]
		public static extern ChromaprintContext chromaprint_new(int algorithm);

		/// <summary>
		/// Restart the computation of a fingerprint with a new audio stream.
		/// </summary>
		/// <param name="ctx">Chromaprint context pointer.</param>
		/// <param name="sample_rate">sample_rate sample rate of the audio stream (in Hz).</param>
		/// <param name="num_channels">num_channels numbers of channels in the audio stream (1 or 2).</param>
		/// <returns>0 on error, 1 on success.</returns>
		[DllImport("chromaprint", EntryPoint = "chromaprint_start", CallingConvention = CallingConvention.Cdecl)]
		public static extern int chromaprint_start(ChromaprintContext ctx, int sample_rate, int num_channels);

		[DllImport(
			"FingerPrinter",
			EntryPoint = "FingerPrint",
			CallingConvention = CallingConvention.Cdecl,
			CharSet = CharSet.Unicode)]
		public static extern IntPtr FingerPrint(string filePath);
	}
}
