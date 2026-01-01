/////////////////////////////////////////////////////////////////////////////
// <copyright file="AudioSignature.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.MusicToolKit
{
	using System;
	using System.Runtime.InteropServices;

	/// <summary>
	/// Represents an audio signature.
	/// </summary>
	public static class AudioSignature
	{
		/// <summary>
		/// Get audio signature.
		/// </summary>
		/// <param name="filePath">The file path of the audio file.</param>
		/// <returns>The audio signature.</returns>
		public static string GetAudioSignature(string filePath)
		{
			IntPtr data = NativeMethods.GetAudioSignature(filePath);
			string audioSignature = Marshal.PtrToStringAnsi(data);

			NativeMethods.FreeAudioSignature(data);

			return audioSignature;
		}
	}
}
