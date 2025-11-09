using Labubu.Auth.Data.Models;

namespace Labubu.Auth.Services;

public interface IRefreshTokenService
{
    Task<RefreshToken> CreateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<RefreshToken?> GetValidRefreshTokenAsync(string token, CancellationToken cancellationToken = default);
    Task RevokeRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default);
    Task RevokeAllTokensAsync(Guid userId, CancellationToken cancellationToken = default);
}

