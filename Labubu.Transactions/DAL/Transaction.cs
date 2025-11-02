namespace Labubu.DAL;

public class Transaction
{
    public Guid Id { get; set; }
    public string Type { get; set; } = null!;
    public DateTimeOffset Date { get; set; }
    public decimal Amount { get; set; }
    public string Bank { get; set; } = null!;
}