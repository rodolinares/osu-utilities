using System;
using System.IO;
using System.Linq;

namespace Rhodus
{
    class Program
    {
        private const string OsuBeatmapsPath = @"D:\osu!\Songs";
        private const string OutputPath = @"D:\Rodo\Documents\_Rodo\osu!Test";
        private static int _currentBeatmap;
        private static int _currentFile;
        private static readonly string[] Separator = { " - " };

        static void Main()
        {
            EditMusicFiles();
            Console.WriteLine("Finished! Press 'ENTER' key to exit.");
            Console.ReadLine();
        }

        private static void EditMusicFiles()
        {
            var files = Directory.GetFiles(OutputPath);
            Console.WriteLine($"{files.Length} were detected");
            Console.WriteLine("Attempting to edit songs metadata...");
            Console.Write($"Progress: {_currentFile} of {files.Length}\r");

            foreach (var file in files)
            {
                var fileName = Path.GetFileNameWithoutExtension(file);
                var data = fileName.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
                var artist = string.Join(" ", data[0].Split(' ').Skip(1));
                var title = data[1];
                var song = TagLib.File.Create(file);
                song.Tag.AlbumArtists = new[] { artist };
                song.Tag.Performers = new[] { artist };
                song.Tag.Title = title;
                song.Tag.Album = "osu!";
                song.Save();
                Console.Write($"Progress: {++_currentFile} of {files.Length}\r");
            }
        }

        private static void GetMusicFiles()
        {
            var beatmaps = Directory.GetDirectories(OsuBeatmapsPath);
            Console.WriteLine($"{beatmaps.Length} beatmaps were detected.");
            Console.WriteLine("Attempting to extract songs...");
            Console.Write($"Progress: {_currentBeatmap} of {beatmaps.Length}\r");

            foreach (var folder in beatmaps)
            {
                var files = Directory.GetFiles(folder, "*.mp3");

                if (files.Length == 0)
                {
                    Console.Write($"Progress: {++_currentBeatmap} of {beatmaps.Length}\r");
                    continue;
                }

                var biggest = files[0];
                var bSize = new FileInfo(biggest).Length;

                foreach (var file in files)
                {
                    var size = new FileInfo(file).Length;

                    if (size > bSize)
                    {
                        biggest = file;
                        bSize = size;
                    }
                }

                var fileName = $"{Path.GetFileName(folder)}.mp3";
                File.Copy(biggest, Path.Combine(OutputPath, fileName));
                Console.Write($"Progress: {++_currentBeatmap} of {beatmaps.Length}\r");
            }
        }
    }
}
