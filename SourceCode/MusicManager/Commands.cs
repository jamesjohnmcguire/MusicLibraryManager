/////////////////////////////////////////////////////////////////////////////
// <copyright file="Commands.cs" company="Digital Zen Works">
// Copyright © 2019 - 2024 Digital Zen Works. All Rights Reserved.
// </copyright>
/////////////////////////////////////////////////////////////////////////////

using DigitalZenWorks.CommandLine.Commands;
using System.Collections.Generic;

namespace DigitalZenWorks.Music.ToolKit.Application
{
	internal static class Commands
	{
		public static IList<Command> GetCommands()
		{
			IList<Command> commands = new List<Command>();

			Command help = new ("help");
			help.Description = "Show this information";
			commands.Add(help);

			IList<CommandOption> options = new List<CommandOption>();

			CommandOption configFile = new ("c", "config", true);
			options.Add(configFile);

			CommandOption location = new ("l", "location", true);
			options.Add(location);

			Command extractTags =
				new ("extract-tags", options, 0, "Extract Tags Information");
			commands.Add(extractTags);

			CommandOption rules = new ("r", "rules", true);
			options.Add(rules);

			CommandOption noUpdateTags = new ("n", "no-update-tags", false);
			options.Add(noUpdateTags);

			Command clean = new ("clean", options, 0, "Clean music files");
			commands.Add(clean);

			return commands;
		}
	}
}
