﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace TvShowsOrganizer
{
    class Program
    {
        static void Main(string[] args)
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

        private static readonly Regex ShowNameEpisodeAndSeasonRegularExpression = new Regex(@"(.*?)\.S(\d{1,2})E(\d{2})\.(.*)", RegexOptions.IgnoreCase);

        private static IEnumerable<TvShowFile> GetTvShows(string inputFolder)
        {
            var files = Directory.GetFiles(inputFolder);
            var shows = new List<TvShowFile>();
            foreach (var file in files)
            {
                var relativePathFile = Path.GetFileName(file);
                var matches = ShowNameEpisodeAndSeasonRegularExpression.Match(relativePathFile);
                if (matches.Success)
                {
                    string tvShowTitle = matches.Groups[1].ToString().Trim();
                    tvShowTitle.Replace(".", " ");
                    int season;
                    if(!int.TryParse(matches.Groups[2].Value, out season))
                    {
                        season = 0;
                    }
                    var show = new TvShowFile
                        {
                            File = file,
                            Title = tvShowTitle,
                            Season = season
                        };
                    shows.Add(show);
                }
            }

            return shows;
        }

        private static void MoveFile(string file, string outputFolder, string tvShowTitle, int season)
        {
            Console.WriteLine("Checking if exists {0}", outputFolder);
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            string tvShowFolder = Path.Combine(outputFolder, tvShowTitle);
            Console.WriteLine("Checking if exists {0}", tvShowFolder);
            if (!Directory.Exists(tvShowFolder))
            {
                Directory.CreateDirectory(tvShowFolder);
            }
            string tvShowSeasonFolder = Path.Combine(tvShowFolder, string.Format("Season {0}", season));
            Console.WriteLine("Checking if exists {0}", tvShowSeasonFolder);
            if (!Directory.Exists(tvShowSeasonFolder))
            {
                Directory.CreateDirectory(tvShowSeasonFolder);
            }
            try
            {
                string finalPathFile = Path.Combine(tvShowSeasonFolder, Path.GetFileName(file));
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