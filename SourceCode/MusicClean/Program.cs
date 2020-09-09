/////////////////////////////////////////////////////////////////////////////
// <copyright file="Program.cs" company="Digital Zen Works">
// Copyright © 2019 - 2020 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using MusicUtility;
using System;
using System.IO;
using System.Reflection;

namespace MusicClean
{
	public static class Program
	{
		public static void Main(string[] args)
		{
			try
			{
				string rulesData = null;

				if (args != null)
				{
					rulesData = GetRulesData(args);
				}

				Rules rules = new Rules(rulesData);

				using MusicManager musicUtility = new MusicManager(rules);

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
					using (StreamReader reader =
						new StreamReader(templateObjectStream))
					{
						contents = reader.ReadToEnd();
					}
				}
			}

			return contents;
		}
	}
}
