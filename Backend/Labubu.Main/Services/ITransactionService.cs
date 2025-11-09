using FluentResults;
using Shared.Contracts;

namespace Labubu.Main.Services;

public interface ITransactionService
{
    Task<Result> AddUserAsync(string username);
    Task<Result> AddBankToUserAsync(string username, string bankName);
    Task<Result<List<BankInfo>>> GetBanksInfoAsync();
    Task<Result<UserStatisticsResult>> GetUserStatisticsAsync(string username, DateTimeOffset fromDate, DateTimeOffset toDate);
}

