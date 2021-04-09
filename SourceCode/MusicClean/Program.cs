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

				if (args != null)
				{
					rulesData = GetRulesData(args);
				}

				Rules rules = new (rulesData);

				using MusicManager musicUtility = new (rules);

				musicUtility.UpdateLibrarySkeleton();
				musicUtility.CleanMusicLibrary();
			}
			catch (Exception exception)
			{
				Console.WriteLine("Exception: " + exception.Message);
			}
		}

		private static string GetRulesData(string[] arguments)
		{
			string data;

			if (arguments.Length > 0)
			{
				string fileName = arguments[0];
				data = File.ReadAllText(fileName);
			}
			else
			{
				data = GetDefaultRules();
			}

			return data;
		}

		private static string GetDefaultRules()
		{
			string contents = null;

			string resourceName = "MusicUtility.DefaultRules.json";
			Assembly thisAssembly = Assembly.GetCallingAssembly();

			using (Stream templateObjectStream =
				thisAssembly.GetManifestResourceStream(resourceName))
			{
				if (templateObjectStream != null)
				{
					using StreamReader reader = new (templateObjectStream);
					contents = reader.ReadToEnd();
				}
			}

			return contents;
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
