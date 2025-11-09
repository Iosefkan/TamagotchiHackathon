namespace Shared.Contracts;

public class AddUserRequest
{
    public Guid UserId { get; set; }
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
}