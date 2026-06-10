using System.Security.Claims;

namespace SGVO.Api.Extensions;

/// <summary>
/// Extension methods for extracting user information from ClaimsPrincipal.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Extracts the user ID from the JWT claims.
    /// Looks for "uid" claim first, then falls back to NameIdentifier.
    /// </summary>
    /// <param name="principal">The claims principal.</param>
    /// <returns>User ID if found; null otherwise.</returns>
    public static ulong? GetUserId(this ClaimsPrincipal principal)
    {
        var userIdClaim = principal.FindFirst("uid")?.Value
            ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (ulong.TryParse(userIdClaim, out var userId))
            return userId;

        return null;
    }
}
