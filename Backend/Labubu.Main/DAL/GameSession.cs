using System.Text.Json;

namespace Labubu.Main.DAL;

public class GameSession
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public Guid LabubuId { get; set; }
    public int? Score { get; set; }
    public JsonDocument? SpecificData { get; set; }
    public int? Award { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    
    public Game Game { get; set; } = null!;
    public Labubu Labubu { get; set; } = null!;
}

