/////////////////////////////////////////////////////////////////////////////
// <copyright file="ExternalProcess.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace MediaFileToolkit;

using Serilog;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

/// <summary>
/// Represents a utility class for executing external processes.
/// </summary>
public class ExternalProcess
{
	/// <summary>
	/// Gets the output from the executed process.
	/// </summary>
	public string Output { get; private set; } = string.Empty;

	/// <summary>
	/// Gets the standard output from the executed process.
	/// </summary>
	public string StandardOutput { get; private set; } = string.Empty;

	/// <summary>
	/// Gets the error output from the executed process.
	/// </summary>
	public string StandardError { get; private set; } = string.Empty;

	/// <summary>
	/// Executes an external application with the specified arguments.
	/// </summary>
	/// <param name="applicationName">The application name.</param>
	/// <param name="arguments">The arguments.</param>
	/// <returns>A value indicating success or not.</returns>
	public bool Execute(
		string applicationName, string arguments)
	{
		bool result = false;

		using Process process = StartProcess(applicationName, arguments);

		// Drain both streams BEFORE waiting
		string standardOutput = process.StandardOutput.ReadToEnd();
		string errorOutput = process.StandardError.ReadToEnd();

		process.WaitForExit();

		SetOutput(applicationName, standardOutput, errorOutput);

		if (process.ExitCode == 0)
		{
			result = true;
		}

		return result;
	}

	/// <summary>
	/// Executes an external application with the specified arguments.
	/// </summary>
	/// <param name="applicationName">The application name.</param>
	/// <param name="arguments">The arguments.</param>
	/// <returns>A value indicating success or not.</returns>
	public async Task<bool> ExecuteAsync(
		string applicationName,
		string arguments)
	{
		bool result = false;

		using Process process = StartProcess(applicationName, arguments);

		// Drain both streams concurrently
		Task<string> stdOutTask = process.StandardOutput.ReadToEndAsync();
		Task<string> stdErrTask = process.StandardError.ReadToEndAsync();

		await process.WaitForExitAsync().ConfigureAwait(false);

		string standardOutput = await stdOutTask.ConfigureAwait(false);
		string errorOutput = await stdErrTask.ConfigureAwait(false);

		SetOutput(applicationName, standardOutput, errorOutput);

		if (process.ExitCode == 0)
		{
			result = true;
		}

		return result;
	}

	private static Process StartProcess(string applicationName, string arguments)
	{
		ProcessStartInfo startInfo = new()
		{
			FileName = applicationName,
			Arguments = arguments,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true
		};

		Process process = new() { StartInfo = startInfo };
		process.Start();

		return process;
	}

	private void SetOutput(
		string applicationName, string standardOutput, string errorOutput)
	{
		string applicationNameBase =
			Path.GetFileNameWithoutExtension(applicationName);
		bool isFfprobe = applicationNameBase.Equals(
			"ffprobe", StringComparison.OrdinalIgnoreCase);

		if (isFfprobe == true)
		{
			Output = standardOutput;
		}
		else
		{
			Output = errorOutput;
		}
	}
}
