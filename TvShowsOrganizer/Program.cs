using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;

namespace TvShowsOrganizer
{
    class Program
    {
        static void Main()
        {
            var shows = GetTvShows(ConfigurationManager.AppSettings["InputFolder"]);
            foreach (var tvShowFile in shows)
            {
                Console.WriteLine("Moving {0}", tvShowFile.File);
                MoveFile(tvShowFile.File, ConfigurationManager.AppSettings["OutputFolder"], tvShowFile.Title, tvShowFile.Season);
                Console.WriteLine("");
                Console.WriteLine("");
            }
            Console.WriteLine("End");
        }

        private static readonly IEnumerable<Regex> ShowNameEpisodeAndSeasonRegularExpressions = new []
        {
            new Regex(@"(.*?)\.S(\d{1,2})E(\d{2})\.(.*)", RegexOptions.IgnoreCase),
            new Regex(@"(.*?)\.(\d{1,2})(\d{2})\.(.*)", RegexOptions.IgnoreCase)
        };

        private static TvShowFile HandleFile(string file)
        {
            var relativePathFile = Path.GetFileName(file);
            if (string.IsNullOrEmpty(relativePathFile))
            {
                return null;
            }
            foreach (var regex in ShowNameEpisodeAndSeasonRegularExpressions)
            {
                var matches = regex.Match(relativePathFile);
                if (!matches.Success)
                {
                    continue;
                }
                string tvShowTitle = matches.Groups[1].ToString().Trim();
                tvShowTitle = CapitalizeWords(tvShowTitle.Replace(".", " ").ToLowerInvariant());
                int season;
                if (!int.TryParse(matches.Groups[2].Value, out season))
                {
                    season = 0;
                }
                return new TvShowFile
                {
                    File = file,
                    Title = tvShowTitle,
                    Season = season
                };
            }
            return null;
        }
        
        private static IEnumerable<TvShowFile> GetTvShows(string inputFolder)
        {
            /* test subfolders for recursivity */
            var folders = Directory.GetDirectories(inputFolder);
            foreach (var folder in folders)
            {
                foreach (var regex in ShowNameEpisodeAndSeasonRegularExpressions)
                {
                    var directoryName = Path.GetFileName(folder);
                    if (string.IsNullOrEmpty(directoryName))
                    {
                        break;
                    }
                    var matches = regex.Match(directoryName);
                    if (!matches.Success)
                    {
                        continue;
                    }
                    foreach (var tvShowFile in GetTvShows(folder))
                    {
                        if (tvShowFile != null)
                        {
                            yield return tvShowFile;
                        }
                    }
                }
            }

            var files = Directory.GetFiles(inputFolder);
            foreach (var file in files)
            {
                var show = HandleFile(file);
                if (show != null)
                {
                    yield return show;
                }
            }
        }

        private static string CapitalizeWords(string words)
        {
            var result = Regex.Replace(words, @"\b(\w)", m => m.Value.ToUpper());
            return Regex.Replace(result, @"\s(of|in|by|and|a)\s", m => m.Value.ToLower(), RegexOptions.IgnoreCase);
        }

        private static void MoveFile(string file, string outputFolder, string tvShowTitle, int season)
        {
            Console.WriteLine("Checking if exists {0}", outputFolder);
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
                Console.WriteLine("Directory {0} created", outputFolder);
            }
            string tvShowFolder = Path.Combine(outputFolder, tvShowTitle);
            Console.WriteLine("Checking if exists {0}", tvShowFolder);
            if (!Directory.Exists(tvShowFolder))
            {
                Directory.CreateDirectory(tvShowFolder);
                Console.WriteLine("Directory {0} created", tvShowFolder);
            }
            string tvShowSeasonFolder = Path.Combine(tvShowFolder, string.Format("Season {0}", season));
            Console.WriteLine("Checking if exists {0}", tvShowSeasonFolder);
            if (!Directory.Exists(tvShowSeasonFolder))
            {
                Directory.CreateDirectory(tvShowSeasonFolder);
                Console.WriteLine("Directory {0} created", tvShowSeasonFolder);
            }
            try
            {
                var fileName = Path.GetFileName(file);
                if (string.IsNullOrEmpty(fileName))
                {
                    Console.WriteLine("Error moving {0}: Unable to determine filename", file);
                    return;
                }
                string finalPathFile = Path.Combine(tvShowSeasonFolder, fileName);
                Console.WriteLine("Moving file to {0}", finalPathFile);
                File.Move(file, finalPathFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error moving {0}: {1}", file, ex.Message);
            }
        }

        private class TvShowFile
        {
            public string Title { get; set; }
            public int Season { get; set; }
            public string File { get; set; }
        }
    }
}
