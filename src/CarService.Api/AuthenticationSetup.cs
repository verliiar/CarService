using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.RateLimiting;
using CarService.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CarService.Api;

public static class AuthenticationSetup
{
    public static IServiceCollection AddAccountAuthentication(this IServiceCollection services)
    {
        services.AddScoped<JwtSessionEvents>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();
        services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
            .Configure<IOptions<JwtOptions>>((options, jwt) =>
            {
                var config = jwt.Value;
                options.MapInboundClaims = false;
                options.IncludeErrorDetails = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = config.Issuer,
                    ValidateAudience = true,
                    ValidAudience = config.Audience,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(config.GetSigningKey()),
                    RequireSignedTokens = true,
                    RequireExpirationTime = true,
                    ValidateLifetime = true,
                    ValidAlgorithms = [SecurityAlgorithms.HmacSha256],
                    ClockSkew = TimeSpan.Zero,
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
                options.EventsType = typeof(JwtSessionEvents);
            });
        services.AddAuthorization(options =>
        {
            var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireAssertion(context =>
            {
                if (context.Resource is not HttpContext http || !http.Request.RouteValues.TryGetValue("id", out var route))
                {
                    return true;
                }

                return Guid.TryParse(route?.ToString(), out var id) &&
                    Guid.TryParse(context.User.FindFirstValue(JwtRegisteredClaimNames.Sub), out var current) && id == current;
            }).Build();
            options.DefaultPolicy = policy;
            options.FallbackPolicy = policy;
        });
        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.AddPolicy("authentication", context => RateLimitPartition.GetFixedWindowLimiter(
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown", _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 100,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0
                }));
        });
        return services;
    }
}
