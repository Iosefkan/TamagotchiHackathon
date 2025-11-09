using System.Security.Claims;

namespace Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid? GetUserId(this ClaimsPrincipal claims)
    {
        var userIdClaim = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return null;
        }
        return userId;
    }

    public static string? GetUserName(this ClaimsPrincipal claims)
    {
        return claims.FindFirst(ClaimTypes.Name)?.Value;
    }

    public static string? GetUserEmail(this ClaimsPrincipal claims)
    {
        return claims.FindFirst(ClaimTypes.Email)?.Value;
    }
}
