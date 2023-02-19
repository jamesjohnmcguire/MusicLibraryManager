/////////////////////////////////////////////////////////////////////////////
// <copyright file="FingerPrinter.cs" company="Digital Zen Works">
// Copyright © 2019 - 2023 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace DigitalZenWorks.MusicToolKit
{
	/// <summary>
	/// Represents an audio finger print signature.
	/// </summary>
	public static class FingerPrinter
	{
		/// <summary>
		/// Get finger print signature.
		/// </summary>
		/// <param name="filePath">The file path of the audio file.</param>
		/// <returns>The finger print.</returns>
		public static string FingerPrint(string filePath)
		{
			IntPtr data = NativeMethods.FingerPrint(filePath);
			string fingerPrint = Marshal.PtrToStringAnsi(data);

			NativeMethods.FreeFingerPrint(data);

			return fingerPrint;
		}
	}
}
