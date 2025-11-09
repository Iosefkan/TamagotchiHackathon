namespace Labubu.Main.DAL;

public class BattlePass
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsActive { get; set; }
    public int MaxLevel { get; set; } = 100;
    public int XpPerLevel { get; set; } = 1000;
    public decimal PurchasePrice { get; set; }
    
    public ICollection<BattlePassReward> Rewards { get; set; } = new List<BattlePassReward>();
    public ICollection<BattlePassProgress> Progresses { get; set; } = new List<BattlePassProgress>();
    public ICollection<BattlePassPurchase> Purchases { get; set; } = new List<BattlePassPurchase>();
}
