using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CarService.Application;
using CarService.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace CarService.Api;

internal sealed class JwtSessionEvents(IUserSessionValidator sessions) : JwtBearerEvents
{
    public override async Task TokenValidated(TokenValidatedContext context)
    {
        var principal = context.Principal;
        var stamp = principal?.FindFirstValue("session");
        var roleClaim = principal?.FindFirstValue("role");
        if (!Guid.TryParse(principal?.FindFirstValue(JwtRegisteredClaimNames.Sub), out var userId) ||
            string.IsNullOrEmpty(stamp) || !Enum.TryParse<UserRole>(roleClaim, out var role) || role.ToString() != roleClaim)
        {
            context.Fail("Invalid identity.");
            return;
        }

        if (!await sessions.IsValidAsync(userId, stamp, role, context.HttpContext.RequestAborted))
        {
            context.Fail("The session is no longer valid.");
        }
    }
}
