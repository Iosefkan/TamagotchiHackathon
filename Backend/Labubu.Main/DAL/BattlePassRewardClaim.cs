namespace Labubu.Main.DAL;

public class BattlePassRewardClaim
{
    public Guid Id { get; set; }
    public Guid BattlePassRewardId { get; set; }
    public BattlePassReward BattlePassReward { get; set; } = null!;
    public Guid LabubuId { get; set; }
    public Labubu Labubu { get; set; } = null!;
    public DateTime ClaimedAt { get; set; } = DateTime.UtcNow;
}
