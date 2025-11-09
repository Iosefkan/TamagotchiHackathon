namespace Labubu.Main.DAL;

public class User
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    
    public Labubu Labubu { get; set; } = null!;
}