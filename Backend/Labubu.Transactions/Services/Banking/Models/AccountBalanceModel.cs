namespace Transactions.Services.Banking.Models;

public class AccountBalanceModel
{
    public string AccountId { get; set; } = null!;
    public string Type { get; set; } = null!;
    public DateTimeOffset DateTime { get; set; } 
    public TransactionAmountModel Amount { get; set; } = null!;
    public string CreditDebitIndicator { get; set; } = null!;
}