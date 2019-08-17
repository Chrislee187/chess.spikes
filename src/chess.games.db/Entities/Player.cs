namespace chess.games.db.Entities
{
    public class Player : DbEntity, IHaveAName
    {
        public string Name { get; set; }
    }
}