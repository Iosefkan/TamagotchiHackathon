namespace Shared.Contracts;

public class UserTransaction
{
    public string Uuid { get; set; } = null!;
    public string BankId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string Type { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Status { get; set; } = null!;
    public DateTimeOffset Date { get; set; }
}

