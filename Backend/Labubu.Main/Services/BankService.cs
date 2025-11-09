using FluentResults;
using Shared.Contracts;

namespace Labubu.Main.Services;

public class BankService
{
    private readonly ITransactionService _transactionService;

    public BankService(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    public async Task<Result> AddUserAsync(string username)
    {
        return await _transactionService.AddUserAsync(username);
    }

    public async Task<Result> AddBankToUserAsync(string username, string bankName)
    {
        return await _transactionService.AddBankToUserAsync(username, bankName);
    }

    public async Task<Result<List<BankInfo>>> GetBanksInfoAsync()
    {
        return await _transactionService.GetBanksInfoAsync();
    }

    public async Task<Result<UserStatisticsResult>> GetUserStatisticsAsync(
        string username, 
        DateTimeOffset fromDate, 
        DateTimeOffset toDate)
    {
        return await _transactionService.GetUserStatisticsAsync(username, fromDate, toDate);
    }
}
