using System;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: Program <path_to_audio_file>");
            return;
        }

        string filePath = args[0];

        var audioType = AudioFormatChecker.GetAudioType(filePath);
        Console.WriteLine($"The audio type of '{filePath}' is {audioType}");
    }
}