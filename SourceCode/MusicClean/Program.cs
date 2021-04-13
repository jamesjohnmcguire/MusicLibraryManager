/////////////////////////////////////////////////////////////////////////////
// <copyright file="Program.cs" company="Digital Zen Works">
// Copyright © 2019 - 2021 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using Common.Logging;
using MusicUtility;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using System;
using System.IO;
using System.Reflection;

namespace MusicClean
{
	public static class Program
	{
		private const string LogFilePath = "MusicMan.log";
		private const string OutputTemplate =
			"[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] " +
			"{Message:lj}{NewLine}{Exception}";

		private static readonly ILog Log = LogManager.GetLogger(
			System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

		public static void Main(string[] args)
		{
			try
			{
				StartUp();

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

				using MusicManager musicUtility = new (rules);

				musicUtility.UpdateLibrarySkeleton();
				musicUtility.CleanMusicLibrary();
			}
			catch (Exception exception)
			{
				Console.WriteLine("Exception: " + exception.Message);
			}
		}

		private static string GetRulesData(string fileName)
		{
			string data = File.ReadAllText(fileName);

			return data;
		}

		private static void StartUp()
		{
			LoggerConfiguration configuration = new ();
			LoggerSinkConfiguration sinkConfiguration = configuration.WriteTo;
			sinkConfiguration.Console(LogEventLevel.Verbose, OutputTemplate);
			sinkConfiguration.File(
				LogFilePath, LogEventLevel.Verbose, OutputTemplate);
			Serilog.Log.Logger = configuration.CreateLogger();

			LogManager.Adapter =
				new Common.Logging.Serilog.SerilogFactoryAdapter();
		}
	}
}
