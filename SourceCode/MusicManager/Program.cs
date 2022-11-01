/////////////////////////////////////////////////////////////////////////////
// <copyright file="Program.cs" company="Digital Zen Works">
// Copyright © 2019 - 2022 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Common.Logging;
using DigitalZenWorks.CommandLine.Commands;
using DigitalZenWorks.RulesLibrary;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.IO;

[assembly: CLSCompliant(true)]

namespace MusicManager
{
	/// <summary>
	/// The main program class.
	/// </summary>
	public static class Program
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

				string rulesData = null;
				Rules rules = null;

				if ((arguments != null) && (arguments.Length > 0))
				{
					rulesData = GetRulesData(arguments[0]);
				}

				if (!string.IsNullOrWhiteSpace(rulesData))
				{
					rules = new (rulesData);
				}

				IList<Command> commands = GetCommands();

				CommandLineArguments commandLine = new (commands, arguments);

				if (commandLine.ValidArguments == false)
				{
					Log.Error(commandLine.ErrorMessage);
				}
				else
				{
					Command command = commandLine.Command;

					using DigitalZenWorks.MusicToolKit.MusicManager
						musicUtility = new (rules);

					switch (command.Name)
					{
						case "clean":
							musicUtility.CleanMusicLibrary();
							break;
						case "extract-tags":
							musicUtility.UpdateLibraryTagsOnly();
							break;
					}
				}
			}
			catch (Exception exception)
			{
				Console.WriteLine("Exception: " + exception.Message);

				throw;
			}
		}

		private static IList<Command> GetCommands()
		{
			IList<Command> commands = new List<Command>();

			Command help = new ("help");
			help.Description = "Show this information";
			commands.Add(help);

			Command clean = new ("clean");
			clean.Description = "Clean music files";
			commands.Add(clean);

			Command extractTags = new ("extract-tags");
			extractTags.Description = "Extract tag info";
			commands.Add(extractTags);

			return commands;
		}

		private static string GetRulesData(string fileName)
		{
			string data = File.ReadAllText(fileName);

			return data;
		}

		private static void LogInitialization()
		{
			string applicationDataDirectory = @"DigitalZenWorks\MusicManager";
			string baseDataDirectory = Environment.GetFolderPath(
				Environment.SpecialFolder.ApplicationData,
				Environment.SpecialFolderOption.Create) + @"\" +
				applicationDataDirectory;

			string logFilePath = baseDataDirectory + @"\MusicManager.log";
			const string outputTemplate =
				"[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] " +
				"{Message:lj}{NewLine}{Exception}";

			LoggerConfiguration configuration = new ();
			configuration = configuration.MinimumLevel.Verbose();

			LoggerSinkConfiguration sinkConfiguration = configuration.WriteTo;
			sinkConfiguration.Console(LogEventLevel.Verbose, outputTemplate);
			sinkConfiguration.File(
				logFilePath,
				LogEventLevel.Verbose,
				outputTemplate,
				flushToDiskInterval: TimeSpan.FromSeconds(1));
			Serilog.Log.Logger = configuration.CreateLogger();

			LogManager.Adapter =
				new Common.Logging.Serilog.SerilogFactoryAdapter();
		}
	}
}
