namespace Labubu.Main.DAL;

public class AchievementProgress
{
    public Guid Id { get; set; }
    public Guid AchievementId { get; set; }
    public Guid LabubuId { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    
    public Achievement Achievement { get; set; } = null!;
    public Labubu Labubu { get; set; } = null!;
}

