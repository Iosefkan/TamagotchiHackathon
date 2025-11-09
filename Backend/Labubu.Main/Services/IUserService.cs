using FluentResults;
using Shared.Contracts;

namespace Labubu.Main.Services;

public interface IUserService
{
    Task<Result<UserEnergyDto>> GetEnergyAsync(Guid userId);
    Task<Result<UserInfoDto>> GetUserInfoAsync(Guid userId);
}

public record UserEnergyDto(int Energy);
public record UserInfoDto(string Login, string Email, List<BankInfoDto> Banks);

public record BankInfoDto(int BankId, string BankName, decimal Balance);

