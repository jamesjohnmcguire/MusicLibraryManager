using System;
using System.CommandLine;
using System.Diagnostics;
using System.IO;

namespace AudioConverter
{
    internal class Program
    {
        static async Task<int> Main(string[] args)
        {
            // Setup command line options
            var rootCommand = new RootCommand("Audio File Batch Converter");

            var inputOption = new Option<DirectoryInfo>(
                "--input",
                "Input directory containing WMA files")
            {
                IsRequired = true
            };

            var formatOption = new Option<string>(
                "--format",
                () => "m4a",
                "Output format (m4a, flac, mp3)");

            var qualityOption = new Option<string>(
                "--quality",
                () => "high",
                "Output quality (high, medium, low)");

            var recursiveOption = new Option<bool>(
                "--recursive",
                () => false,
                "Search for files recursively");

            rootCommand.AddOption(inputOption);
            rootCommand.AddOption(formatOption);
            rootCommand.AddOption(qualityOption);
            rootCommand.AddOption(recursiveOption);

            rootCommand.SetHandler(async (inputDir, format, quality, recursive) =>
            {
                var converter = new AudioConverter(inputDir, format, quality, recursive);
                await converter.ConvertFiles();
            },
            inputOption, formatOption, qualityOption, recursiveOption);

            return await rootCommand.InvokeAsync(args);
        }
    }
}
