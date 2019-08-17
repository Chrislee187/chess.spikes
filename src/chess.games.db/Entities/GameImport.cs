using System.ComponentModel.DataAnnotations;

namespace chess.games.db.Entities
{
    public class GameImport : DbEntity
    {
        public Event Event { get; set; }
        public Site Site { get; set; }
        public Player White { get; set; }
        public Player Black { get; set; }

        [MaxLength(30)]
        public string Date { get; set; }

        public string Round { get; set; }
        public GameResult Result { get; set; }

        public string MoveText { get; set; }
    }
}