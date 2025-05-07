/////////////////////////////////////////////////////////////////////////////
// <copyright file="Configuration.cs" company="Digital Zen Works">
// Copyright © 2019 - 2025 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

namespace DigitalZenWorks.Music.ToolKit.Application
{
	using System;
	using System.IO;
	using DigitalZenWorks.CommandLine.Commands;

	internal static class Configuration
	{
		public static string GetConfigurationFile(Command command)
		{
			string location;

			CommandOption optionFound = command.GetOption("c", "config");

			if (optionFound != null)
			{
				location = optionFound.Parameter;
			}
			else
			{
				location = GetDefaultConfigurationFile();
			}

			return location;
		}

		public static string GetDefaultConfigurationFile()
		{
			string configurationFile = null;

			string dataPath = GetDefaultDataLocation();

			// Will use existing directory or create it.
			Directory.CreateDirectory(dataPath);

			string configFile = dataPath + @"\MusicManager.json";

			if (File.Exists(configFile))
			{
				configurationFile = configFile;
			}

			return configurationFile;
		}

		public static string GetDefaultDataLocation()
		{
			string defaultDataLocation;

			string baseDataDirectory = Environment.GetFolderPath(
				Environment.SpecialFolder.ApplicationData,
				Environment.SpecialFolderOption.Create);

			string applicationDataDirectory = @"DigitalZenWorks\MusicManager";

			defaultDataLocation =
				Path.Combine(baseDataDirectory, applicationDataDirectory);

			return defaultDataLocation;
		}
	}
}
