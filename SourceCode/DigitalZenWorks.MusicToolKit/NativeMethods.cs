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
	internal class NativeMethods
	{
		[DllImport(
			"FingerPrinter",
			BestFitMapping = false,
			CallingConvention = CallingConvention.Cdecl,
			CharSet = CharSet.Ansi,
			EntryPoint = "FingerPrint",
			ThrowOnUnmappableChar = true)]
		public static extern IntPtr FingerPrint(string filePath);

		[DllImport(
			"FingerPrinter",
			EntryPoint = "FreeFingerPrint",
			CallingConvention = CallingConvention.Cdecl)]
		public static extern void FreeFingerPrint(IntPtr data);
	}
}
