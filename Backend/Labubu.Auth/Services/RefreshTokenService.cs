using Labubu.Auth.Configuration;
using Labubu.Auth.Data;
using Labubu.Auth.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Labubu.Auth.Services;

public class RefreshTokenService : IRefreshTokenService
{
    private readonly AuthDbContext _context;
    private readonly IJwtService _jwtService;
    private readonly JwtConfig _config;

    public RefreshTokenService(
        AuthDbContext context, 
        IJwtService jwtService,
        IOptions<JwtConfig> config)
    {
        _context = context;
        _jwtService = jwtService;
        _config = config.Value;
    }

    public async Task<RefreshToken> CreateRefreshTokenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = _jwtService.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(_config.RefreshTokenExpirationDays),
            IsRevoked = false,
            UserId = userId
        };

        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        return refreshToken;
    }

    public async Task<RefreshToken?> GetValidRefreshTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        return await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token &&
                                       !rt.IsRevoked &&
                                       rt.ExpiresAt > DateTime.UtcNow, cancellationToken);
    }

    public async Task RevokeRefreshTokenAsync(RefreshToken refreshToken, CancellationToken cancellationToken = default)
    {
        refreshToken.IsRevoked = true;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RevokeAllTokensAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var tokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in tokens)
        {
            token.IsRevoked = true;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
