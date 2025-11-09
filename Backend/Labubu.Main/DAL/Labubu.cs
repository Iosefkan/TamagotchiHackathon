namespace Labubu.Main.DAL;

public class Labubu
{
    public Guid Id { get; set; }
    public int Health { get; set; } = 100;
    public int Weight { get; set; } = 1;
    public int Status { get; set; } = 1;
    public int Energy { get; set; } = 100;
    public string? Message { get; set; }
    public bool IsMessageRead { get; set; } = true;
    public DateTime? LastMessageGeneratedAt { get; set; }
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public ICollection<LabubuClothes> LabubuClothes { get; set; } = new List<LabubuClothes>();
    public ICollection<AchievementProgress> AchievementProgresses { get; set; } = new List<AchievementProgress>();
    public ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
    public ICollection<BattlePassProgress> BattlePassProgresses { get; set; } = new List<BattlePassProgress>();
    public ICollection<BattlePassPurchase> BattlePassPurchases { get; set; } = new List<BattlePassPurchase>();
    public ICollection<BattlePassRewardClaim> BattlePassRewardClaims { get; set; } = new List<BattlePassRewardClaim>();
}