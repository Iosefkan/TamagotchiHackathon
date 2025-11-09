namespace Transactions.Services.Banking.Models;

public class ConsentModel
{
    public string RequestId { get; set; } = null!;
    public string ConsentId { get; set; } = null!;
    public string Status { get; set; } = null!;
    public string Message { get; set; } = null!;
    public DateTimeOffset CreatedAt { get; set; }
    public bool AutoApproved { get; set; }
}