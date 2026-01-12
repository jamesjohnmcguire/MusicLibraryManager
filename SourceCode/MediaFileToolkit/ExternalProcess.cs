/////////////////////////////////////////////////////////////////////////////
// <copyright file="ExternalProcess.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace MediaFileToolkit;

using Serilog;
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
	/// Executes an external application with the specified arguments.
	/// </summary>
	/// <param name="applicationName">The application name.</param>
	/// <param name="arguments">The arguments.</param>
	/// <returns>A value indicating success or not.</returns>
	public async Task<bool> Execute(
		string applicationName, string arguments)
	{
		bool result = false;

		ProcessStartInfo startInfo = new()
		{
			FileName = applicationName,
			Arguments = arguments,
			RedirectStandardOutput = true,
			RedirectStandardError = true,
			UseShellExecute = false,
			CreateNoWindow = true
		};

		using Process process = new() { StartInfo = startInfo };
		process.Start();
		await process.WaitForExitAsync().ConfigureAwait(false);

		if (process.ExitCode == 0)
		{
			result = true;
		}
		else
		{
			StreamReader errorOutput = process.StandardOutput;
			Output =
				await errorOutput.ReadToEndAsync().ConfigureAwait(false);
		}

		return result;
	}
}
