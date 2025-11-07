namespace Transactions.Services.Banking.Models;

public class TransactionModel
{
    public string AccountId { get; set; } = null!;
    public string TransactionId { get; set; } = null!;
    public TransactionAmountModel Amount { get; set; } = null!;
    public CreditDebitIndicator CreditDebitIndicator { get; set; }
    public string Status { get; set; } = null!;
    public DateTimeOffset BookingDateTime { get; set; }
    public DateTimeOffset ValueDateTime { get; set; }
    public string TransactionInformation { get; set; } = null!;
}

public enum CreditDebitIndicator
{
    Debit,
    Credit
}
