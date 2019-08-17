using System.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace chess.games.db.Entities
{
    public class Event : DbEntity, IHaveAName
    {
        public string Name { get; set; }
    }
}