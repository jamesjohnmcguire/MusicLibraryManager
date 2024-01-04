/////////////////////////////////////////////////////////////////////////////
// <copyright file="Configuration.cs" company="Digital Zen Works">
// Copyright © 2019 - 2024 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using System.IO;
using System;

namespace DigitalZenWorks.Music.ToolKit.Application
{
	internal static class Configuration
	{
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
