namespace Shared.Contracts;

public class UserStatisticsResult
{
    public HashSet<string> BankNames { get; set; } = new();
    public decimal FundsAvailable { get; set; }
    public Dictionary<string, int> Transactions { get; set; } = new();
    public decimal TotalSpent { get; set; }
    public List<UserTransaction> TransactionFeed { get; set; } = new();
}