using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
using Transactions.DAL;
using Transactions.Services.Banking;
using Transactions.Services.Banking.Models;

namespace Transactions.Consumers;

public class UserStatisticsConsumer(
    TransactionsDataContext db,
    IBankingService service,
    ILogger<UserStatisticsConsumer> logger) : IConsumer<UserStatisticsRequest>
{
    public async Task Consume(ConsumeContext<UserStatisticsRequest> context)
    {
        try
        {
            var result = new UserStatisticsResult();
            var user = await db.Users.FirstAsync(u => u.Name == context.Message.Username);

            var accounts = await db.Accounts.Where(a => a.UserId == user.Id)
                .Include(a => a.Consent)
                .ToListAsync();
            
            result.BankNames = accounts.Select(a => a.Name).ToHashSet();
            foreach (var account in accounts)
            {
                if (DateTimeOffset.UtcNow.Subtract(account.InterimAvailableAt) > TimeSpan.FromHours(5))
                {
                    var balanceResult = await service.GetAccountBalanceAsync(
                        account.BankAccountId, 
                        account.Consent.BankConsentId, 
                        account.BankName);

                    if (balanceResult.IsSuccess)
                    {
                        account.InterimAvailable = balanceResult.Value.Amount.Amount;
                        account.InterimAvailableAt = balanceResult.Value.DateTime;
                    }
                }

                result.FundsAvailable += account.InterimAvailable;
                var (addRes, newTransacts) = await GetTransactionsSpending(
                    account, 
                    context.Message.FromDate, 
                    context.Message.ToDate, 
                    result.Transactions);
                result.TotalSpent += addRes;
                result.Transactions = newTransacts;
            }
            
            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while aggregating data");
        }
    }

    private async Task<(decimal, Dictionary<string, int>)> 
        GetTransactionsSpending(Account account, DateTimeOffset startDate, DateTimeOffset endDate, Dictionary<string, int> transacts)
    {
        decimal result = 0;
        const int pageSize = 50;
        var page = 1;
        var transactionsResult = await service.GetTransactionsAsync(
            account.BankAccountId,
            account.Consent.BankConsentId,
            account.BankName,
            startDate,
            endDate,
            page,
            pageSize);
        
        while (transactionsResult is { IsSuccess: true, Value.Count: >= 0 })
        {
            var transactions = transactionsResult.Value!;
            foreach (var transaction in transactions)
            {
                transacts[transaction.TransactionInformation] += 1;
                var amount = transaction.Amount.Amount;
                result += transaction.CreditDebitIndicator is CreditDebitIndicator.Credit 
                    ? -amount 
                    : amount;
            }
                
            page++;
            transactionsResult = await service.GetTransactionsAsync(
                account.BankAccountId,
                account.Consent.BankConsentId,
                account.BankName,
                startDate,
                endDate,
                page,
                pageSize);
        }

        return (result, transacts);
    }
}