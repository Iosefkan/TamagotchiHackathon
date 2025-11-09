using Shared.Enums;

namespace Transactions.Services.Banking.Models;

public class TransactionAmountModel
{
    public decimal Amount { get; set; }
    public CurrencyCode Currency { get; set; }
}