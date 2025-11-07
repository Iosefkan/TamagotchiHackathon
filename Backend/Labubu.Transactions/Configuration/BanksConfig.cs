using Shared.Contracts;

namespace Transactions.Configuration;

public class BanksConfig
{
    public List<BankInfo> Banks { get; set; } = null!;
}