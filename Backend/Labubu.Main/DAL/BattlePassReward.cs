namespace Labubu.Main.DAL;

public class BattlePassReward
{
    public Guid Id { get; set; }
    public Guid BattlePassId { get; set; }
    public BattlePass BattlePass { get; set; } = null!;
    public int Level { get; set; }
    public bool IsPremium { get; set; }
    public Guid? ClothesId { get; set; }
    public Clothes? Clothes { get; set; }
    public int? CurrencyAmount { get; set; }
    public string? CurrencyType { get; set; }
    
    public ICollection<BattlePassRewardClaim> Claims { get; set; } = new List<BattlePassRewardClaim>();
}
