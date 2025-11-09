namespace Labubu.Main.DAL;

public class Game
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    
    public ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
}

