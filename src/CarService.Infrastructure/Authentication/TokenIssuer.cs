using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CarService.Application;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CarService.Infrastructure;

public sealed class TokenIssuer(IOptions<JwtOptions> options, TimeProvider clock)
{
    public AuthResponse Issue(ApplicationUser account)
    {
        var config = options.Value;
        var now = clock.GetUtcNow();
        var expires = now.AddMinutes(config.AccessTokenMinutes);
        var user = account.Profile;
        var token = new JwtSecurityToken(config.Issuer, config.Audience,
            [new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
             new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
             new Claim("name", user.Name), new Claim("role", user.Role.ToString()),
             new Claim("session", account.SecurityStamp!)],
            now.UtcDateTime, expires.UtcDateTime,
            new SigningCredentials(new SymmetricSecurityKey(config.GetSigningKey()), SecurityAlgorithms.HmacSha256));
        return new(new JwtSecurityTokenHandler().WriteToken(token), expires, new(user.Id, user.Name, user.Role.ToString()));
    }
}
