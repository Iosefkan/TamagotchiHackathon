namespace Labubu.Main.DAL;

public class Achievement
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string Url { get; set; } = null!;
    
    public ICollection<AchievementProgress> AchievementProgresses { get; set; } = new List<AchievementProgress>();
}

