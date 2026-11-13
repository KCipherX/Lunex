using System.Security.Claims;

namespace Lunex.Api.Extensions.Utilities;

public static class ClaimsPrincipalExtensions
{
    public static string? GetUserId(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public static string GetRequiredUserId(this ClaimsPrincipal principal)
    {
        var userId = principal.GetUserId();

        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new InvalidOperationException("User ID claim not found in token.");
        }

        return userId;
    }

    public static string? GetEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(ClaimTypes.Email);
    }

    public static string? GetName(this ClaimsPrincipal principal)
    {
        return principal.FindFirstValue(ClaimTypes.Name);
    }

    public static IEnumerable<string> GetRoles(this ClaimsPrincipal principal)
    {
        return principal.FindAll(ClaimTypes.Role).Select(c => c.Value);
    }

    public static bool HasRole(this ClaimsPrincipal principal, string role)
    {
        return principal.IsInRole(role);
    }

    public static bool HasAnyRole(this ClaimsPrincipal principal, params string[] roles)
    {
        return roles.Any(principal.IsInRole);
    }

    public static string? GetClaimValue(this ClaimsPrincipal principal, string claimType)
    {
        return principal.FindFirstValue(claimType);
    }

    public static IEnumerable<Claim> GetClaims(this ClaimsPrincipal principal, string claimType)
    {
        return principal.FindAll(claimType);
    }
}