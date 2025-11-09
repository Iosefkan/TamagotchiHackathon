namespace Labubu.Main.DAL;

public class BattlePassProgress
{
    public Guid Id { get; set; }
    public Guid BattlePassId { get; set; }
    public BattlePass BattlePass { get; set; } = null!;
    public Guid LabubuId { get; set; }
    public Labubu Labubu { get; set; } = null!;
    public int CurrentLevel { get; set; } = 1;
    public int CurrentXp { get; set; } = 0;
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
}
