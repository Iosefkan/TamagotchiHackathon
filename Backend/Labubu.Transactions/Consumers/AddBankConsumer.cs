using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;
using Transactions.DAL;
using Transactions.Services.Banking;

namespace Transactions.Consumers;

public class AddBankConsumer(
    TransactionsDataContext db, 
    IBankingService service,
    ILogger<AddBankConsumer> logger) : IConsumer<AddBankToUserRequest>
{
    public async Task Consume(ConsumeContext<AddBankToUserRequest> context)
    {
        var message = context.Message;
        
        await using var transaction = await db.Database.BeginTransactionAsync();

        try
        {
            var userId = (await db.Users.FirstAsync(u => u.Name == message.Username)).Id;
            var consentResult = await service.GetConsentAsync(message.Username, message.BankName);
            if (consentResult.IsFailed) throw new Exception();

            var consent = new Consent
            {
                CreatedAt = consentResult.Value.CreatedAt,
                BankConsentId = consentResult.Value.ConsentId,
                IsApproved = consentResult.Value.AutoApproved,
                RequestId = consentResult.Value.RequestId,
            };
            db.Consents.Add(consent);
            await db.SaveChangesAsync();
        
            var accountsResult = await service.GetAccountsAsync(consent.BankConsentId, message.Username, message.BankName);
            if (accountsResult.IsFailed) throw new Exception();
            
            var accounts = accountsResult.Value
                .Select(a => new Account
                {
                    BankAccountId = a.AccountId,
                    BankName = message.BankName,
                    ConsentId = consent.Id,
                    Currency = a.Currency,
                    UserId = userId,
                    Name = a.Nickname
                }).ToList();

            foreach (var account in accounts)
            {
                var balanceResult = await service.GetAccountBalanceAsync(account.BankAccountId, consent.BankConsentId, message.BankName);
                if (balanceResult.IsFailed) continue;
                account.InterimAvailable = balanceResult.Value.Amount.Amount;
                account.InterimAvailableAt = balanceResult.Value.DateTime;
            }
            
            db.Accounts.AddRange(accounts);
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "Failed adding bank to user");
        }
    }
}