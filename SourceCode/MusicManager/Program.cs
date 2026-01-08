/////////////////////////////////////////////////////////////////////////////
// <copyright file="Program.cs" company="Digital Zen Works">
// Copyright © 2019 - 2026 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

[assembly: System.CLSCompliant(true)]

namespace DigitalZenWorks.Music.ToolKit.Application;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using DigitalZenWorks.CommandLine.Commands;
using DigitalZenWorks.RulesLibrary;
using global::Common.Logging;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;

/// <summary>
/// The main program class.
/// </summary>
internal static class Program
{
	private static readonly ILog Log = LogManager.GetLogger(
		System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	/// <param name="arguments">An array of arguments passed to
	/// the program.</param>
	public static void Main(string[] arguments)
	{
		try
		{
			LogInitialization();

			Log.Info("Starting Music Manager");

			IList<Command> commands = Commands.GetCommands();

			CommandLineInstance commandLine = new (commands, arguments);

			if (commandLine.ValidArguments == false)
			{
				Log.Error(commandLine.ErrorMessage);
			}
			else
			{
				Command command = commandLine.Command;

				switch (command.Name)
				{
					case "clean":
					{
						using MusicToolKit.MusicManager musicUtility =
							new (true);

						string configurationFile =
							Configuration.GetConfigurationFile(command);

						string location = GetLocation(command);

						if (!string.IsNullOrWhiteSpace(location))
						{
							musicUtility.LibraryLocation = location;
						}

						Rules rules = GetRulesData(command);

						if (rules != null)
						{
							musicUtility.Rules = rules;
						}

						bool noUpdateTags = command.DoesOptionExist(
							"n", "no-update-tags");
						musicUtility.UpdateTags = !noUpdateTags;

						musicUtility.CleanMusicLibrary();
					}

					break;
					case "extract-tags":
					{
						using MusicToolKit.MusicManager musicUtility =
							new (false);

						if (command.Parameters.Count > 0)
						{
							string location = command.Parameters[0];
							string tagsOnlyLocation =
								location + " Tags Only";

							musicUtility.UpdateLibraryTagsOnly(
								location, tagsOnlyLocation);
						}
						else
						{
							musicUtility.UpdateLibraryTagsOnly();
						}
					}

					break;
				}
			}
		}
		catch (Exception exception)
		{
			Log.Error(exception.Message);

			throw;
		}
	}

	private static string GetLocation(Command command)
	{
		string location = null;

		CommandOption optionFound = command.GetOption("l", "location");

		if (optionFound != null)
		{
			location = optionFound.Parameter;
		}

		return location;
	}

	private static Rules GetRulesData(Command command)
	{
		Rules rules = null;
		CommandOption optionFound =
			command.GetOption("r", "rules");

		if (optionFound != null)
		{
			string rulesFilePath = optionFound.Parameter;
			string rulesData = File.ReadAllText(rulesFilePath);

			rules = new (rulesData);
		}

		return rules;
	}

	private static void LogInitialization()
	{
		string applicationDataDirectory =
			Configuration.GetDefaultDataLocation();
		string logFilePath =
			Path.Combine(applicationDataDirectory, "MusicManager.log");

		const string outputTemplate =
			"[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] " +
			"{Message:lj}{NewLine}{Exception}";

		LoggerConfiguration configuration = new ();
		configuration = configuration.MinimumLevel.Verbose();

		LoggerSinkConfiguration sinkConfiguration = configuration.WriteTo;
		sinkConfiguration.Console(
			LogEventLevel.Verbose,
			outputTemplate,
			CultureInfo.InvariantCulture);
		sinkConfiguration.File(
			logFilePath,
			LogEventLevel.Verbose,
			outputTemplate,
			CultureInfo.InvariantCulture,
			flushToDiskInterval: TimeSpan.FromSeconds(1));
		Serilog.Log.Logger = configuration.CreateLogger();

		LogManager.Adapter =
			new global::Common.Logging.Serilog.SerilogFactoryAdapter();
	}
}
