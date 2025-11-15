using FluentResults;

namespace Labubu.Main.Services;

public interface IFinanceService
{
    Task<Result<FinanceSummaryDto>> GetSummaryAsync(Guid userId);
}

public record FinanceSummaryDto(
    FinanceUserDto User,
    decimal Balance,
    List<FinanceBankDto> Banks,
    List<FinanceTransactionDto> Transactions);

public record FinanceUserDto(Guid Id, string Name, string Email, string? Picture);

public record FinanceBankDto(string Id, string Name);

public record FinanceTransactionDto(
    string Uuid,
    string BankId,
    string Name,
    string Category,
    string Type,
    decimal Amount,
    string Status,
    DateTimeOffset Date);

