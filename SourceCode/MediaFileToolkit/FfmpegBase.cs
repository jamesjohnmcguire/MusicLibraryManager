/////////////////////////////////////////////////////////////////////////////
// <copyright file="FfmpegBase.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace MediaFileToolkit;

using System;
using System.ComponentModel;
using Serilog;

/// <summary>
/// The FFmpeg base class.
/// </summary>
[System.Diagnostics.CodeAnalysis.SuppressMessage(
	"Design",
	"CA1052:Static holder types should be Static or NotInheritable",
	Justification = "This is likely a temporary suppression.")]
public class FfmpegBase
{
	/// <summary>
	/// Checks if FFmpeg is installed and available in the system PATH.
	/// </summary>
	/// <returns>A value indicating success or not.</returns>
	public static bool CheckFfmpeg()
	{
		bool result = false;

		try
		{
			ExternalProcess process = new();

			result = process.Execute("ffmpeg", "-version");
		}
		catch (Exception exception) when
			(exception is Win32Exception ||
			exception is InvalidOperationException)
		{
			// ffmpeg not found or cannot be executed
			Log.Error("FFmpeg is not installed or not found in PATH.");
		}

		return result;
	}
}
