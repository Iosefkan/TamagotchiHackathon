using System.Security.Claims;
using Labubu.Auth.Data.Models;

namespace Labubu.Auth.Services.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromToken(string token);
}