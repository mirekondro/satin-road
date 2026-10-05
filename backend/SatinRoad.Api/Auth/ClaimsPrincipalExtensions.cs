using System.Security.Claims;
using SatinRoad.Core.Common;

namespace SatinRoad.Api.Auth;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? user.FindFirstValue("sub");

        return int.TryParse(value, out var id)
            ? id
            : throw new UnauthorizedException("You must be logged in.");
    }
}