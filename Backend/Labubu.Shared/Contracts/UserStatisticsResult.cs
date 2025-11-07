using System.Transactions;

namespace Shared.Contracts;

public class UserStatisticsResult
{
    public HashSet<string> BankNames { get; set; } = null!;
    public decimal FundsAvailable { get; set; }
    public Dictionary<string, int> Transactions { get; set; } = null!;
    public decimal TotalSpent { get; set; }
}