namespace Transactions.DAL;

public class Consent
{
    public Guid Id { get; set; }
    public string BankConsentId { get; set; } = null!;
    
    public string RequestId { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public bool IsApproved { get; set; }
}