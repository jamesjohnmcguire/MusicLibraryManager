/////////////////////////////////////////////////////////////////////////////
// <copyright file="NativeMethods.cs" company="Digital Zen Works">
// Copyright © 2019 - 2025 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;
using System.Security;

[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]

namespace DigitalZenWorks.MusicToolKit
{
	/// <summary>
	/// The native method class.
	/// </summary>
	[SuppressUnmanagedCodeSecurity]
	internal static class NativeMethods
	{
		/// <summary>
		/// Get audio signature.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns>The audio signature.</returns>
		/// <remarks>Caller must free the returned pointer using
		/// FreeAudioSignature.</remarks>
		[DllImport(
			"AudioSignature",
			BestFitMapping = false,
			CallingConvention = CallingConvention.Cdecl,
			CharSet = CharSet.Ansi,
			EntryPoint = "GetAudioSignature")]
		public static extern IntPtr GetAudioSignature(string filePath);

		/// <summary>
		/// Free audio signature.
		/// </summary>
		/// <param name="data"></param>
		[DllImport(
			"AudioSignature",
			CallingConvention = CallingConvention.Cdecl,
			EntryPoint = "FreeAudioSignature")]
		public static extern void FreeAudioSignature(IntPtr data);
	}
}
