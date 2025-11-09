using FluentResults;
using MassTransit;
using Shared.Contracts;

namespace Labubu.Main.Services;

public class TransactionService : ITransactionService
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IRequestClient<BankInfoRequest> _banksInfoClient;
    private readonly IRequestClient<UserStatisticsRequest> _userStatisticsClient;
    private readonly ILogger<TransactionService> _logger;

    public TransactionService(
        IPublishEndpoint publishEndpoint,
        IRequestClient<BankInfoRequest> banksInfoClient,
        IRequestClient<UserStatisticsRequest> userStatisticsClient,
        ILogger<TransactionService> logger)
    {
        _publishEndpoint = publishEndpoint;
        _banksInfoClient = banksInfoClient;
        _userStatisticsClient = userStatisticsClient;
        _logger = logger;
    }

    public async Task<Result> AddUserAsync(string username)
    {
        try
        {
            var request = new AddUserRequest { Username = username };
            await _publishEndpoint.Publish(request);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add user {Username}", username);
            return Result.Fail($"Failed to add user: {ex.Message}");
        }
    }

    public async Task<Result> AddBankToUserAsync(string username, string bankName)
    {
        try
        {
            var request = new AddBankToUserRequest
            {
                Username = username,
                BankName = bankName
            };
            await _publishEndpoint.Publish(request);
            return Result.Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add bank {BankName} to user {Username}", bankName, username);
            return Result.Fail($"Failed to add bank to user: {ex.Message}");
        }
    }

    public async Task<Result<List<BankInfo>>> GetBanksInfoAsync()
    {
        try
        {
            var request = new BankInfoRequest();
            var response = await _banksInfoClient.GetResponse<List<BankInfo>>(request, timeout: TimeSpan.FromSeconds(30));
            return Result.Ok(response.Message);
        }
        catch (RequestTimeoutException ex)
        {
            _logger.LogError(ex, "Timeout while getting banks info");
            return Result.Fail("Timeout while getting banks info");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get banks info");
            return Result.Fail($"Failed to get banks info: {ex.Message}");
        }
    }

    public async Task<Result<UserStatisticsResult>> GetUserStatisticsAsync(string username, DateTimeOffset fromDate, DateTimeOffset toDate)
    {
        try
        {
            var request = new UserStatisticsRequest
            {
                Username = username,
                FromDate = fromDate,
                ToDate = toDate
            };
            var response = await _userStatisticsClient.GetResponse<UserStatisticsResult>(request, timeout: TimeSpan.FromSeconds(60));
            return Result.Ok(response.Message);
        }
        catch (RequestTimeoutException ex)
        {
            _logger.LogError(ex, "Timeout while getting user statistics for {Username}", username);
            return Result.Fail("Timeout while getting user statistics");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get user statistics for {Username}", username);
            return Result.Fail($"Failed to get user statistics: {ex.Message}");
        }
    }
}

