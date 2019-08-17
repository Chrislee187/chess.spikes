namespace chess.games.db.Entities
{
    public interface IHaveAName
    {
        string Name { get; set; }
    }

    public class Site : DbEntity, IHaveAName
    {
        public string Name { get; set; }
    }
}