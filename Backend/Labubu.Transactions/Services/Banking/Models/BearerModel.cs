namespace Transactions.Services.Banking.Models;

public class BearerModel
{
    public string AccessToken { get; set; } = null!;
    public string TokenType { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string Algorithm { get; set; } = null!;
    public int ExpiresIn { get; set; }
}