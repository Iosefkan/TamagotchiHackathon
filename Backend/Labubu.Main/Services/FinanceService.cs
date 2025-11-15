using System.Linq;
using FluentResults;
using Labubu.Main.Data;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace Labubu.Main.Services;

public class FinanceService : IFinanceService
{
    private readonly MainContext _context;
    private readonly ITransactionService _transactionService;
    private readonly BankService _bankService;
    private readonly ILogger<FinanceService> _logger;

    public FinanceService(
        MainContext context,
        ITransactionService transactionService,
        BankService bankService,
        ILogger<FinanceService> logger)
    {
        _context = context;
        _transactionService = transactionService;
        _bankService = bankService;
        _logger = logger;
    }

    public async Task<Result<FinanceSummaryDto>> GetSummaryAsync(Guid userId)
    {
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return Result.Fail("User not found");
        }

        var toDate = DateTimeOffset.UtcNow;
        var fromDate = toDate.AddMonths(-1);

        var statisticsResult = await _transactionService.GetUserStatisticsAsync(
            user.Name,
            fromDate,
            toDate);

        if (statisticsResult.IsFailed)
        {
            _logger.LogWarning("Failed to retrieve statistics for user {UserId}: {Error}",
                userId,
                statisticsResult.Errors.FirstOrDefault()?.Message);
        }

        var statistics = statisticsResult.IsSuccess
            ? statisticsResult.Value
            : new UserStatisticsResult();

        var banksInfoResult = await _bankService.GetBanksInfoAsync();
        var banksInfo = banksInfoResult.IsSuccess
            ? banksInfoResult.Value
            : new List<BankInfo>();

        var banks = statistics.BankNames
            .Select(bankId =>
            {
                var knownBank = banksInfo.FirstOrDefault(b => b.BankName == bankId);
                var name = knownBank?.BankName ?? bankId;
                return new FinanceBankDto(bankId, name);
            })
            .ToList();

        var transactions = statistics.TransactionFeed
            .Select(t => new FinanceTransactionDto(
                t.Uuid,
                t.BankId,
                t.Name,
                t.Category,
                t.Type,
                t.Amount,
                t.Status,
                t.Date))
            .ToList();

        var userDto = new FinanceUserDto(
            user.Id,
            user.Name,
            user.Email,
            Picture: null);

        var summary = new FinanceSummaryDto(
            userDto,
            statistics.FundsAvailable,
            banks,
            transactions);

        return Result.Ok(summary);
    }
}

