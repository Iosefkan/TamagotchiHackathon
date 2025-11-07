using FluentResults;
using Transactions.Services.Banking.Models;

namespace Transactions.Services.Banking;

public interface IBankingService
{
    Task<Result<List<TransactionModel>>> GetTransactionsAsync(
        string accountId,
        string consentId, 
        string bankName, 
        DateTimeOffset startDate,
        DateTimeOffset endDate, 
        int page, 
        int pageSize);
    Task<Result<List<AccountModel>>> GetAccountsAsync(string consentId, string username, string bankName);
    Task<Result<ConsentModel>> GetConsentAsync(string username, string bankName);
    Task<Result<AccountBalanceModel>> GetAccountBalanceAsync(string accountId, string consentId, string bankName);
}