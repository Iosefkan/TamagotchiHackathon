using FluentResults;
using Shared.Contracts;

namespace Labubu.Main.Services;

public interface IMessageGenerationService
{
    Task<Result<string>> GenerateMessageAsync(UserStatisticsResult statistics, DateTime today, string timezone, string currency);
}

