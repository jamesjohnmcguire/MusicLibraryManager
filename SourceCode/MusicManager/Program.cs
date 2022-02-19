/////////////////////////////////////////////////////////////////////////////
// <copyright file="Program.cs" company="Digital Zen Works">
// Copyright © 2019 - 2022 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Common.Logging;
using DigitalZenWorks.MusicToolKit;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using System;
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
		/// <param name="args">An array of arguments passed to
		/// the program.</param>
		public static void Main(string[] args)
		{
			try
			{
				LogInitialization();

				Log.Info("Starting Music Manager");

				string rulesData = null;
				Rules rules = null;

				if ((args != null) && (args.Length > 0))
				{
					rulesData = GetRulesData(args[0]);
				}

				if (!string.IsNullOrWhiteSpace(rulesData))
				{
					rules = new (rulesData);
				}

				using DigitalZenWorks.MusicToolKit.MusicManager musicUtility = new (rules);

				musicUtility.UpdateLibrarySkeleton();
				musicUtility.CleanMusicLibrary();
			}
			catch (Exception exception)
			{
				Console.WriteLine("Exception: " + exception.Message);

				throw;
			}
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
