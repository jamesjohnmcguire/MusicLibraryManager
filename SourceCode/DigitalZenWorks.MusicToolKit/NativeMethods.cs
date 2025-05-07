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
	[SuppressUnmanagedCodeSecurity]
	internal static class NativeMethods
	{
		// Caller must free the returned pointer using FreeAudioSignature
		[DllImport(
			"AudioSignature",
			BestFitMapping = false,
			CallingConvention = CallingConvention.Cdecl,
			CharSet = CharSet.Ansi,
			EntryPoint = "GetAudioSignature")]
		public static extern IntPtr GetAudioSignature(string filePath);

		[DllImport(
			"AudioSignature",
			CallingConvention = CallingConvention.Cdecl,
			EntryPoint = "FreeAudioSignature")]
		public static extern void FreeAudioSignature(IntPtr data);
	}
}
