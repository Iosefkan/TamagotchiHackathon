using Shared.Enums;

namespace Transactions.DAL;

public class Account
{
    public Guid Id { get; set; }
    public string BankAccountId { get; set; } = null!;
    
    public CurrencyCode Currency { get; set; }
    public string Name { get; set; } = null!;
    public decimal InterimAvailable { get; set; }
    public DateTimeOffset InterimAvailableAt { get; set; }
    public string BankName { get; set; } = null!;
    
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
    
    public Guid ConsentId { get; set; }
    public Consent Consent { get; set; } = null!;
}