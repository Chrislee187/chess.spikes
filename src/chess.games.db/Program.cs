using System;
using System.Linq;
using chess.games.db.Entities;
using Microsoft.EntityFrameworkCore;

namespace chess.games.db
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Chess DB");
            var c = new ChessGamesDbContext();
            if (c.Database.GetPendingMigrations().Any())
            {
                Console.WriteLine("Applying pending migrations...");
                c.Database.Migrate();
            }
        }
    }


}
