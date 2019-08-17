using System;
using System.Collections.Generic;
using System.Linq;
using chess.games.db.Entities;
using Microsoft.EntityFrameworkCore;
using PgnReader;

namespace chess.games.db.api
{
    public interface IGamesRepository
    {
        long TotalGames { get; }
        int AddImportBatch(PgnGame[] games);
    }

    public class GamesRepository : IGamesRepository
    {
        private readonly ChessGamesDbContext _chessGamesDbContext;
        private IDictionary<Type, IDictionary<string, object>> _childCache;

        public GamesRepository(ChessGamesDbContext chessGamesDbContext)
        {
            _chessGamesDbContext = chessGamesDbContext;
            CacheChildren();
        }

        private void CacheChildren()
        {
            _chessGamesDbContext.RunWithExtendedTimeout(() =>
            {
                _childCache = new Dictionary<Type, IDictionary<string, object>>
                {
                    {
                        typeof(Event), _chessGamesDbContext.Events.ToList()
                            .GroupBy(k => NormaliseName(k.Name), (k, g) => g.First())
                            .ToDictionary(k => NormaliseName(k.Name), v => (object) v)
                    },
                    {
                        typeof(Site), _chessGamesDbContext.Sites.ToList()
                            .GroupBy(k => NormaliseName(k.Name), (k, g) => g.First())
                            .ToDictionary(k => NormaliseName(k.Name), v => (object) v)
                    },
                    {
                        typeof(Player), _chessGamesDbContext.Players.ToList()
                            .GroupBy(k => NormaliseName(k.Name), (k, g) => g.First())
                            .ToDictionary(k => NormaliseName(k.Name), v => (object) v)
                    },
                };

            }, TimeSpan.FromMinutes(5));
        }

        public long TotalGames => _chessGamesDbContext.Games.Count();

        public int AddImportBatch(PgnGame[] games)
        {
            int created = 0;
            _chessGamesDbContext.RunWithExtendedTimeout(() =>
            {
                _chessGamesDbContext.Database.ExecuteSqlCommand("TRUNCATE TABLE [GameImports]");

                games.ToList().ForEach(pgnGame =>
                {
                    var site = GetOrCreateCachedEntity<Site>(pgnGame.Site);
                    var whitePlayer = GetOrCreateCachedEntity<Player>(pgnGame.White);
                    var blackPlayer = GetOrCreateCachedEntity<Player>(pgnGame.Black);
                    var @event = GetOrCreateCachedEntity<Event>(pgnGame.Event);

                    var game = new GameImport
                    {
                        Event = @event,
                        Black = blackPlayer,
                        Site = site,
                        White = whitePlayer,
                        Round = pgnGame.Round,
                        MoveText = NormaliseMoveText(pgnGame),
                        Date = pgnGame.Date.ToString(),
//                        Result = pgnGame.Result // TODO: Mappers
                    };
                    _chessGamesDbContext.GameImports.Attach(game);
                });
                _chessGamesDbContext.SaveChanges();

                var sql = MergeNewGamesSql;
                created = _chessGamesDbContext.Database.ExecuteSqlCommand(sql);
            }, TimeSpan.FromMinutes(5));
            return created;
        }

        private static string  MergeNewGamesSql = 
            "INSERT Games (Id, EventId, SiteId, WhiteId, BlackId, Date, [Round], Result, MoveText) " +
            "SELECT Id, EventId, SiteId, WhiteId, BlackId, Date, [Round], Result, MoveText " +
            "FROM GameImports imp " +
            "WHERE NOT EXISTS (SELECT " +
            "EventId, SiteId, WhiteId, BlackId, Date, [Round], Result, MoveText " +
            "FROM Games g2 WHERE " +
            "g2.EventId = imp.EventId " +
            "AND g2.SiteId = imp.SiteId " +
            "AND g2.WhiteId = imp.WhiteId " +
            "AND g2.BlackId = imp.BlackId " +
            "AND g2.Date = imp.Date " +
            "AND g2.[Round] = imp.[Round] " +
            "AND g2.Result = imp.Result " +
            "AND g2.MoveText = imp.MoveText " +
            ")";

        private T GetOrCreateCachedEntity<T>(string name) where T : class, IHaveAName
        {
            var cache = _childCache[typeof(T)];
            var normaliseName = NormaliseName(name);
            if (!cache.TryGetValue(normaliseName, out var entity))
            {
                var instance = Activator.CreateInstance<T>();
                instance.Name = name;
                cache.Add(normaliseName, instance);
                entity = instance;
            }

            return (T)entity;

        }
        private static string NormaliseMoveText(PgnGame pgnGame)
        {
            return pgnGame.MoveText
                .Replace("\n", " ")
                .Replace("\r", " ")
                .Replace("  ", " ")
                .Replace("{ ", "{")
                .Replace(" }", "}");
        }
        private static string NormaliseName(string name) =>
            name.ToLower()
                .Replace("-", " ")
                .Replace(".", " ");

    }
}
