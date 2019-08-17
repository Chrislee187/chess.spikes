using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using chess.games.db.api;
using chess.games.db.Entities;
using Microsoft.EntityFrameworkCore;
using PgnReader;

namespace chess.games.db.pgnimporter
{
    class Program
    {
        private static readonly ChessGamesDbContext DbContext = new ChessGamesDbContext();
        private static readonly PgnFileArchiver Archiver = new PgnFileArchiver();
        private static readonly PgnFileFinder Finder = new PgnFileFinder();

        static void Main(string[] args)
        {
            DbContext.UpdateDatabase();
            var scanPath = args.Any() ? args[0] : @".\";

            var pgnFiles = Finder.FindFiles(args[0]);

            ImportGames(pgnFiles, scanPath);
            
        }

        private static void ImportGames(string[] pgnFiles, string scanPath)
        {
            Console.WriteLine("Initialising repo and cache...");
            IGamesRepository repo = new GamesRepository(DbContext);
            Console.WriteLine($"Beginning import of {pgnFiles.Length} PGN files at: {DateTime.Now}");

            var fileCount = 0;

            pgnFiles.ToList().ForEach(file =>
            {
                fileCount++;
                try
                {
                    Console.WriteLine($"File #{fileCount}/{pgnFiles.Length} : {file}");

                    var pgnGames = PgnGame.ReadAllGamesFromFile(file).ToArray();

                    Console.WriteLine($"Checking {pgnGames.Count()} games for new entries...");
                    var sw = Stopwatch.StartNew();
                    var createdCount = repo.AddImportBatch(pgnGames);
                    sw.Stop();

                    Console.WriteLine(
                        $"  File complete, {createdCount} new games added to DB (file contained {pgnGames.Count() - createdCount} duplicates) , DB Total Games: {repo.TotalGames}");
                    Console.WriteLine(
                        $"  time taken: {sw.Elapsed}, games created per second: {createdCount/sw.Elapsed.Seconds}");

                    Archiver.ArchiveImportedFile(file, scanPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"ERROR: Importing file: {file}");

                    Console.WriteLine(e);
                    var failPath = Archiver.ArchiveFailedFile(file, scanPath);
                    Console.WriteLine($"Fail archived at: {failPath}");

                    if (e is SqlException) throw;
                }
            });
        }


    }
}
