/////////////////////////////////////////////////////////////////////////////
// <copyright file="NativeMethods.cs" company="Digital Zen Works">
// Copyright © 2019 - 2025 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;

[assembly: DefaultDllImportSearchPaths(DllImportSearchPath.SafeDirectories)]

namespace DigitalZenWorks.MusicToolKit
{
	internal static class NativeMethods
	{
		// Caller must free the returned pointer using FreeAudioSignature
		[DllImport(
			"AudioSignature",
			BestFitMapping = false,
			CallingConvention = CallingConvention.Cdecl,
			CharSet = CharSet.Ansi,
			EntryPoint = "GetAudioSignature",
			ThrowOnUnmappableChar = true)]
		public static extern IntPtr GetAudioSignature(string filePath);

		[DllImport(
			"AudioSignature",
			EntryPoint = "FreeAudioSignature",
			CallingConvention = CallingConvention.Cdecl)]
		public static extern void FreeAudioSignature(IntPtr data);
	}
}
