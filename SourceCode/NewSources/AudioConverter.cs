using System;
using System.Diagnostics;

namespace AudioConverter
{
    public class AudioConverter
    {
        private readonly DirectoryInfo _inputDirectory;
        private readonly string _format;
        private readonly string _quality;
        private readonly bool _recursive;
        private readonly string _logFile = "audio_conversion.log";
        private int _successCount = 0;
        private int _totalFiles = 0;

        public AudioConverter(DirectoryInfo inputDirectory, string format, string quality, bool recursive)
        {
            _inputDirectory = inputDirectory;
            _format = format.ToLower();
            _quality = quality.ToLower();
            _recursive = recursive;

            // Validate format
            if (!new[] { "m4a", "flac", "mp3" }.Contains(_format))
            {
                throw new ArgumentException("Invalid format. Supported formats: m4a, flac, mp3");
            }

            // Validate quality
            if (!new[] { "high", "medium", "low" }.Contains(_quality))
            {
                throw new ArgumentException("Invalid quality. Supported qualities: high, medium, low");
            }
        }

        public async Task ConvertFiles()
        {
            if (!CheckFfmpeg())
            {
                LogError("FFmpeg is not installed. Please install FFmpeg to use this application.");
                return;
            }

            Log($"Starting conversion process at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");

            var searchOption = _recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
            var wmaFiles = _inputDirectory.GetFiles("*.wma", searchOption);
            _totalFiles = wmaFiles.Length;

            if (_totalFiles == 0)
            {
                LogError($"No WMA files found in {_inputDirectory.FullName}");
                return;
            }

            Log($"Found {_totalFiles} WMA files to process");

            foreach (var file in wmaFiles)
            {
                await ConvertFile(file);
                Log($"Progress: {_successCount}/{_totalFiles}");
            }

            Log($"Conversion completed: {_successCount}/{_totalFiles} files converted successfully");
            Log($"Process completed at {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}");
        }

        private async Task ConvertFile(FileInfo inputFile)
        {
            var outputPath = Path.ChangeExtension(inputFile.FullName, _format);

            if (File.Exists(outputPath))
            {
                Log($"Skipping {inputFile.Name} - output file already exists");
                return;
            }

            var arguments = GetFfmpegArguments(inputFile.FullName, outputPath);

            try
            {
                Log($"Converting: {inputFile.Name}");
                
                var startInfo = new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using var process = new Process { StartInfo = startInfo };
                process.Start();
                await process.WaitForExitAsync();

                if (process.ExitCode == 0)
                {
                    _successCount++;
                }
                else
                {
                    var error = await process.StandardError.ReadToEndAsync();
                    LogError($"Error converting {inputFile.Name}: {error}");
                }
            }
            catch (Exception ex)
            {
                LogError($"Exception while converting {inputFile.Name}: {ex.Message}");
            }
        }

        private string GetFfmpegArguments(string inputPath, string outputPath)
        {
            var qualitySettings = new Dictionary<string, Dictionary<string, string>>
            {
                ["m4a"] = new()
                {
                    ["high"] = "-c:a aac -b:a 256k",
                    ["medium"] = "-c:a aac -b:a 192k",
                    ["low"] = "-c:a aac -b:a 128k"
                },
                ["flac"] = new()
                {
                    ["high"] = "-c:a flac -compression_level 8",
                    ["medium"] = "-c:a flac -compression_level 5",
                    ["low"] = "-c:a flac -compression_level 3"
                },
                ["mp3"] = new()
                {
                    ["high"] = "-c:a libmp3lame -b:a 320k",
                    ["medium"] = "-c:a libmp3lame -b:a 192k",
                    ["low"] = "-c:a libmp3lame -b:a 128k"
                }
            };

            return $"-i \"{inputPath}\" -map_metadata 0 {qualitySettings[_format][_quality]} \"{outputPath}\"";
        }

        private bool CheckFfmpeg()
        {
            try
            {
                using var process = Process.Start(new ProcessStartInfo
                {
                    FileName = "ffmpeg",
                    Arguments = "-version",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                });
                return process != null;
            }
            catch
            {
                return false;
            }
        }

        private void Log(string message)
        {
            var logMessage = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - INFO - {message}";
            Console.WriteLine(message);
            File.AppendAllText(_logFile, logMessage + Environment.NewLine);
        }

        private void LogError(string message)
        {
            var logMessage = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - ERROR - {message}";
            Console.Error.WriteLine(message);
            File.AppendAllText(_logFile, logMessage + Environment.NewLine);
        }
    }
}