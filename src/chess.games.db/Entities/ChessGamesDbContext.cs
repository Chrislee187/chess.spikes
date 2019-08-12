using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace chess.games.db.Entities
{
    public class ChessGamesDbContext : DbContext
    {
        private readonly string _dataSource =
            "Server=localhost,1433;Database=ChessGames;User Id=game-importer;Password=Abcde123!"; // My local docker hosted SQL Dev edition
//            "Data Source=D:\\src\\chess\\src\\chess.games.db\\chessgames.db"; //SQLite
        public DbSet<Event> Events { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Game> Games { get; set; }

        public ChessGamesDbContext()
        {
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
    }
}