using System;
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
        private static readonly ChessGamesDbContext _dbContext = new ChessGamesDbContext();
        private static readonly PgnFileArchiver _archiver = new PgnFileArchiver();
        private static readonly PgnFileFinder _finder = new PgnFileFinder();

        static void Main(string[] args)
        {
            UpdateDatabase(_dbContext);
            var scanPath = args.Any() ? args[0] : @".\";

            var pgnFiles = _finder.FindFiles(args[0]);

            ImportGames(pgnFiles, scanPath);
        }

        private static void ImportGames(string[] pgnFiles, string scanPath)
        {
            var repo = new GamesRepository(_dbContext);
            Console.WriteLine($"Beginning import of {pgnFiles.Length} PGN files at: {DateTime.Now}");

            var fileCount = 0;

            pgnFiles.ToList().ForEach(file =>
            {
                fileCount++;

                var sw = Stopwatch.StartNew();
                Console.WriteLine($"File #{fileCount}/{pgnFiles.Length} : {file}");

                try
                {
                    var pgnGames = PgnGame.ReadAllGamesFromFile(file).ToList();
                    var gameCount = 0;
                    var createdCount = 0;
                    foreach (var pgnGame in pgnGames)
                    {
                        gameCount++;

                        var created = repo.GetOrCreate(pgnGame).created;
                        if (created) createdCount++;
                        Console.Write(Progress(gameCount, pgnGames.Count()));
                    }

                    sw.Stop();
                    Console.WriteLine();
                    Console.WriteLine(
                        $"  File complete, {createdCount} new games added to DB (file contained {pgnGames.Count()}) , DB Total Games: {repo.Select().Count()}");
                    Console.WriteLine(
                        $"  time taken: {sw.Elapsed}, avg. time per game: {new TimeSpan(Convert.ToInt64(sw.Elapsed.Ticks / pgnGames.Count()))}");

                    _archiver.ArchiveImportedFile(file, scanPath);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"ERROR: Importing file: {file}");

                    Console.WriteLine(e);
                    var failPath  = _archiver.ArchiveFailedFile(file, scanPath);
                    Console.WriteLine($"Fail archived at: {failPath}");
                }
            });
        }


        private static void UpdateDatabase(ChessGamesDbContext dbContext)
        {
            var migs = dbContext.Database.GetPendingMigrations().ToList();

            if (migs.Any())
            {
                Console.WriteLine("Pending DB migrations:");
                migs.ForEach(m => Console.WriteLine($"  {m}"));

                Console.WriteLine("Applying...");
                dbContext.Database.Migrate();
                Console.WriteLine("DB Migrated");
            }
        }

        private static string DotProgress(int count, bool created)
        {
            return count % 1000 == 0 ? "M"
                : count % 500 == 0 ? "D"
                : count % 100 == 0 ? "C"
                : created ? "+" : "=";
        }

        private static int lastStringLength = 0;
        private static string Progress(int current, int total)
        {
            var newString = $"{current}/{total}";
            var s = new string('\b', lastStringLength) + newString;

            lastStringLength = newString.Length;

            return s;
        }
    }
}
