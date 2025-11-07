namespace Shared.Contracts;

public class UserStatisticsRequest
{
    public string Username { get; set; } = null!;
    public DateTimeOffset FromDate { get; set; }
    public DateTimeOffset ToDate { get; set; }
}