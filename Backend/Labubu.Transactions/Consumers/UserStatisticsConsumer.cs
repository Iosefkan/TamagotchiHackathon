using System.Linq;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
using Transactions.DAL;
using Transactions.Extensions;
using Transactions.Services.Banking;
using Transactions.Services.Banking.Models;

namespace Transactions.Consumers;

public class UserStatisticsConsumer(
    TransactionsDataContext db,
    IBankingService service,
    ILogger<UserStatisticsConsumer> logger,
    IDistributedCache cache) : IConsumer<UserStatisticsRequest>
{
    public async Task Consume(ConsumeContext<UserStatisticsRequest> context)
    {
        try
        {
            var message = context.Message;
            var cacheKey = $"user_stats:{message.Username}:{message.FromDate.UtcDateTime:O}:{message.ToDate.UtcDateTime:O}";

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            };
            
            var result = await cache.GetOrSetAsync(cacheKey, async () =>
            {
                var user = await db.Users.FirstAsync(u => u.Name == message.Username);

                var accounts = await db.Accounts.Where(a => a.UserId == user.Id)
                    .Include(a => a.Consent)
                    .ToListAsync();

                var stats = new UserStatisticsResult();
                stats.BankNames.UnionWith(accounts.Select(a => a.BankName));

                foreach (var account in accounts)
                {
                    await UpdateBalanceIfNeeded(account);

                    stats.FundsAvailable += account.InterimAvailable;

                    var spent = await CollectTransactionsAsync(
                        account,
                        message.FromDate,
                        message.ToDate,
                        stats);

                    stats.TotalSpent += spent;
                }

                stats.TransactionFeed = stats.TransactionFeed
                    .OrderByDescending(t => t.Date)
                    .ToList();

                return stats;
            }, cacheOptions);


            await context.RespondAsync(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while aggregating data");
        }
    }

    private async Task UpdateBalanceIfNeeded(Account account)
    {
        if (DateTimeOffset.UtcNow.Subtract(account.InterimAvailableAt) <= TimeSpan.FromHours(5))
        {
            return;
        }

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

    private async Task<decimal> CollectTransactionsAsync(
        Account account, 
        DateTimeOffset startDate, 
        DateTimeOffset endDate, 
        UserStatisticsResult aggregate)
    {
        decimal result = 0;
        const int pageSize = 50;
        var page = 1;
        bool hasMore;

        do
        {
            var transactionsResult = await service.GetTransactionsAsync(
                account.BankAccountId,
                account.Consent.BankConsentId,
                account.BankName,
                startDate,
                endDate,
                page,
                pageSize);

            hasMore = transactionsResult.IsSuccess 
                      && transactionsResult.Value is { Count: > 0 };

            if (!hasMore)
            {
                break;
            }

            foreach (var transaction in transactionsResult.Value!)
            {
                var categoryKey = ResolveCategory(transaction.TransactionInformation);

                if (!aggregate.Transactions.ContainsKey(categoryKey))
                {
                    aggregate.Transactions[categoryKey] = 0;
                }

                aggregate.Transactions[categoryKey] += 1;

                var amount = Math.Abs(transaction.Amount.Amount);
                var isIncome = transaction.CreditDebitIndicator is CreditDebitIndicator.Credit;

                result += isIncome ? -amount : amount;

                aggregate.TransactionFeed.Add(new UserTransaction
                {
                    Uuid = transaction.TransactionId,
                    BankId = account.BankName,
                    Name = transaction.TransactionInformation ?? "Transaction",
                    Category = categoryKey,
                    Type = isIncome ? "income" : "expense",
                    Amount = amount,
                    Status = NormalizeStatus(transaction.Status),
                    Date = transaction.BookingDateTime
                });
            }

            page++;
        } while (hasMore && page < 100); // guard runaway pagination

        return result;
    }

    private static string ResolveCategory(string? information)
    {
        return string.IsNullOrWhiteSpace(information)
            ? "Other"
            : information;
    }

    private static string NormalizeStatus(string? status)
    {
        return status?.ToLowerInvariant() switch
        {
            "success" or "booked" or "completed" or "accepted" => "success",
            "pending" or "in_progress" or "processing" => "pending",
            "failed" or "rejected" or "error" or "declined" => "error",
            _ => "pending"
        };
    }
}