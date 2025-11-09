namespace Shared.Contracts;

public class AddBankToUserRequest
{
    public string Username { get; set; } = null!;
    public string BankName { get; set; } = null!;
}