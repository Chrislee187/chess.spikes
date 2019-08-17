using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace chess.games.db.Entities
{
    public class ChessGamesDbContext : DbContext
    {
        private readonly string _dataSource =
            "Server=localhost,1433;Database=ChessGames;User Id=game-importer;Password=Abcde123!"; // My local docker hosted SQL
        public DbSet<Event> Events { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameImport> GameImports { get; set; }

        public ChessGamesDbContext()
        {
        }

        public void RunWithExtendedTimeout(Action action, TimeSpan timeout)
        {
            var oldTimeOut = Database.GetCommandTimeout();
            Database.SetCommandTimeout(timeout);

            action();

            Database.SetCommandTimeout(oldTimeOut);
        }

        public ChessGamesDbContext(string sqlLiteDbFile)
        {
            _dataSource = $"Data Source={sqlLiteDbFile }" ;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (_dataSource.Contains("Data Source"))
            {
                optionsBuilder.UseSqlite(_dataSource);
            }
            else
            {
                optionsBuilder.UseSqlServer(_dataSource);
            }
        }

        public IEnumerable<Game> GamesWithIncludes()
        {
            return Games
                .Include(i => i.Black)
                .Include(i => i.White)
                .Include(i => i.Event)
                .Include(i => i.Site);
        }

        public TEntity GetOrCreate<TEntity>(
            Func<TEntity, bool> matcher,
            Func<TEntity> builder
        ) where TEntity : class
            => Set<TEntity>().Any(matcher)
                ? Set<TEntity>().Single(matcher)
                : builder();


        public void UpdateDatabase()
        {
            var migs = Database.GetPendingMigrations().ToList();

            if (migs.Any())
            {
                Console.WriteLine("Pending DB migrations:");
                migs.ForEach(m => Console.WriteLine($"  {m}"));

                Console.WriteLine("Applying...");
                var oldTimeOut = Database.GetCommandTimeout();
                Database.SetCommandTimeout(TimeSpan.FromMinutes(5));

                Database.Migrate();
                Database.SetCommandTimeout(oldTimeOut);
                Console.WriteLine("DB Migrated");
            }
        }
    }
}