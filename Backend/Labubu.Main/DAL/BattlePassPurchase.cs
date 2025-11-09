namespace Labubu.Main.DAL;

public class BattlePassPurchase
{
    public Guid Id { get; set; }
    public Guid BattlePassId { get; set; }
    public BattlePass BattlePass { get; set; } = null!;
    public Guid LabubuId { get; set; }
    public Labubu Labubu { get; set; } = null!;
    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;
    public decimal PricePaid { get; set; }
}
