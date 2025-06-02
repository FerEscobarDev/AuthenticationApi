using System.Security.Claims;
using AuthenticationApi.Common.Constants;

namespace AuthenticationApi.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        return Guid.TryParse(userId, out var guid)
            ? guid
            : throw new UnauthorizedAccessException("Invalid or missing user identifier claim.");
    }

    public static string? GetEmail(this ClaimsPrincipal user)
    {
        return user.FindFirst(ClaimTypes.Email)?.Value;
    }

    public static IEnumerable<string> GetRoles(this ClaimsPrincipal user)
    {
        return user.FindAll(ClaimTypes.Role).Select(r => r.Value);
    }

    public static bool HasRole(this ClaimsPrincipal user, string role)
    {
        return user.IsInRole(role);
    }

    public static string? GetUserName(this ClaimsPrincipal user)
    {
        return user.Identity?.Name;
    }

    public static IEnumerable<string> GetPermissions(this ClaimsPrincipal user)
    {
        return user.FindAll(CustomClaimTypes.Permissions).Select(p => p.Value);
    }
}