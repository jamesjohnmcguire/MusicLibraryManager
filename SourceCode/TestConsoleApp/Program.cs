﻿/////////////////////////////////////////////////////////////////////////////
// <copyright file="Program.cs" company="Digital Zen Works">
// Copyright © 2023 - 2025 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using DigitalZenWorks.MusicToolKit;

[assembly: CLSCompliant(true)]

namespace Music
{
	/// <summary>
	/// Dbx to pst program class.
	/// </summary>
	internal static class Program
	{
		/// <summary>
		/// The program's main entry point.
		/// </summary>
		/// <param name="arguments">The arguments given to the program.</param>
		/// <returns>A value indicating success or not.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Globalization",
			"CA1303:Do not pass literals as localized parameters",
			Justification = "It is just a test application.")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Style",
			"IDE0060:Remove unused parameter",
			Justification = "It is just a test application.")]
		public static int Main(string[] arguments)
		{
			int result = 0;

			Console.WriteLine("Testing");

			string appDataFolder = Environment.GetFolderPath(
				Environment.SpecialFolder.ApplicationData);
			string relativePath = Path.Combine(
				"DigitalZenWorks", "MusicManager", "Sakura.mp4");
			string testFilePath = Path.Combine(appDataFolder, relativePath);

			string audioSignature =
				AudioSignature.GetAudioSignature(testFilePath);

			return result;
		}
	}
}
