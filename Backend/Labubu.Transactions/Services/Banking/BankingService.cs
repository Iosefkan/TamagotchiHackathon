using System.Text.Json;
using FluentResults;
using Flurl;
using Flurl.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Shared.Contracts;
using Transactions.Configuration;
using Transactions.Extensions;
using Transactions.Services.Banking.Models;

namespace Transactions.Services.Banking;

public class BankingService(
    IOptions<AppConfig> appOptions,
    IOptions<BanksConfig> bankOptions,
    IDistributedCache cache,
    ILogger<BankingService> logger) : IBankingService
{
    private const string BearerCacheKey = "Bearer";
    private static readonly string[] Permissions =
    [
        "ReadAccountsDetail",
        "ReadBalances",
        "ReadTransactionsDetail"
    ];

    private static readonly JsonSerializerOptions CamelOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private Task<string> GetToken(BankInfo bank)
    {
        var key = $"{bank.BankName}:{BearerCacheKey}";
        return cache.GetOrSetAsync(key, 
            async () => await RefreshBearer(bank),
            new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(12)));
    }

    private async Task<string> RefreshBearer(BankInfo bank)
    {
        var bearerModel = await bank.ToBearerUrl()
            .SetQueryParam("client_id", appOptions.Value.Id)
            .SetQueryParam("client_secret", bank.Secret)
            .GetJsonAsync<BearerModel>();
        
        return bearerModel.AccessToken;
    }
    
    private async Task<IFlurlRequest> GetRequestBase(string url, BankInfo bank)
    {
        return new FlurlRequest(url)
            .WithOAuthBearerToken(await GetToken(bank));
    }

    // Hate the FluentResult
    public async Task<Result<List<TransactionModel>>> GetTransactionsAsync(
        string accountId, 
        string consentId, 
        string bankName, 
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        int page, 
        int pageSize)
    {
        var bankInfo = bankOptions.Value.Banks.FirstOrDefault(b => b.BankName == bankName);
        if (bankInfo is null) return Result.Fail<List<TransactionModel>>("Invalid bank name");

        try
        {
            var request = await GetRequestBase(bankInfo.ToTransactionsUrl(accountId), bankInfo);
            using var json = await JsonDocument.ParseAsync(await request
                .SetQueryParam("from_booking_date_time", startDate)
                .SetQueryParam("to_booking_date_time", endDate)
                .SetQueryParam("page", page)
                .SetQueryParam("limit", pageSize)
                .SetQueryParam("account_id", accountId)
                .SetQueryParam("x-consent-id", consentId)
                .SetQueryParam("x-requesting-bank", appOptions.Value.Id)
                .GetStreamAsync());
            
            return json.RootElement
                .GetProperty("data")
                .GetProperty("transaction")
                .Deserialize<List<TransactionModel>>(CamelOptions)!;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while getting transactions");
            return Result.Fail<List<TransactionModel>>(ex.Message);
        }
    }

    public async Task<Result<List<AccountModel>>> GetAccountsAsync(string consentId, string username, string bankName)
    {
        var bankInfo = bankOptions.Value.Banks.FirstOrDefault(b => b.BankName == bankName);
        if (bankInfo is null) return Result.Fail<List<AccountModel>>("Invalid bank name");
        
        try
        {
            var request = await GetRequestBase(bankInfo.ToAccountsUrl(), bankInfo);
            using var json = await JsonDocument.ParseAsync(await request
                .SetQueryParam("x-consent-id", consentId)
                .SetQueryParam("x-requesting-bank", appOptions.Value.Id)
                .SetQueryParam("client_id", username)
                .GetStreamAsync());
            
            return json.RootElement
                .GetProperty("data")
                .GetProperty("account")
                .Deserialize<List<AccountModel>>(CamelOptions)!;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while getting accounts");
            return Result.Fail<List<AccountModel>>(ex.Message);
        }
    }

    public async Task<Result<ConsentModel>> GetConsentAsync(string username, string bankName)
    {
        var bankInfo = bankOptions.Value.Banks.FirstOrDefault(b => b.BankName == bankName);
        if (bankInfo is null) return Result.Fail<ConsentModel>("Invalid bank name");
        
        try
        {
            var request = await GetRequestBase(bankInfo.ToConsentUrl(), bankInfo);
            var result = await request
                .SetQueryParam("x-requesting-bank", appOptions.Value.Id)
                .PostJsonAsync(new
                {
                    client_id = username,
                    permissions = Permissions,
                    reason = appOptions.Value.Reason,
                    requesting_bank = appOptions.Value.Id,
                    requesting_bank_name = appOptions.Value.Name
                });

            return await result.GetJsonAsync<ConsentModel>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while getting consent");
            return Result.Fail<ConsentModel>(ex.Message);
        }
    }

    public async Task<Result<AccountBalanceModel>> GetAccountBalanceAsync(string accountId, string consentId, string bankName)
    {
        var bankInfo = bankOptions.Value.Banks.FirstOrDefault(b => b.BankName == bankName);
        if (bankInfo is null) return Result.Fail<AccountBalanceModel>("Invalid bank name");
        
        try
        {
            var request = await GetRequestBase(bankInfo.ToAccountBalanceUrl(accountId), bankInfo);
            using var json = await JsonDocument.ParseAsync(await request
                .SetQueryParam("account_id", accountId)
                .SetQueryParam("x-consent-id", consentId)
                .SetQueryParam("x-requesting-bank", appOptions.Value.Id)
                .GetStreamAsync());
            
            var res = json.RootElement
                .GetProperty("data")
                .GetProperty("balance")
                .Deserialize<List<AccountBalanceModel>>(CamelOptions)!;
            
            return res.First(r => r.Type == "InterimAvailable");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception while getting balance");
            return Result.Fail<AccountBalanceModel>(ex.Message);
        }
    }
}

file static class UrlExtensions
{
    public static string ToBearerUrl(this BankInfo bankInfo)
    {
        return Url.Combine(bankInfo.BankUrl, "auth", "bank-token");
    }

    public static string ToConsentUrl(this BankInfo bankInfo)
    {
        return Url.Combine(bankInfo.BankUrl, "account-consents", "request");
    }

    public static string ToAccountsUrl(this BankInfo bankInfo)
    {
        return Url.Combine(bankInfo.BankUrl, "accounts");
    }

    public static string ToAccountBalanceUrl(this BankInfo bankInfo, string accountId)
    {
        return Url.Combine(bankInfo.BankUrl, "accounts", accountId, "balances");
    }

    public static string ToTransactionsUrl(this BankInfo bankInfo, string accountId)
    {
        return Url.Combine(bankInfo.BankUrl, "accounts", accountId, "transactions");
    }

}