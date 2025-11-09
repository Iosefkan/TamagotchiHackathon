using Labubu.Auth.Data.Models;

namespace Labubu.Auth.Services;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    System.Security.Claims.ClaimsPrincipal? GetPrincipalFromToken(string token);
}

