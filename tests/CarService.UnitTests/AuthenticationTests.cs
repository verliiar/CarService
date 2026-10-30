using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using CarService.Domain;
using CarService.Infrastructure;
using Microsoft.Extensions.Options;
using Xunit;

namespace CarService.UnitTests;

public sealed class AuthenticationTests
{
    [Fact]
    public void AccessTokenUsesServerIdentityRoleSessionAndConfiguredLifetime()
    {
        var now = new DateTimeOffset(2026, 10, 31, 0, 0, 0, TimeSpan.Zero);
        var settings = new JwtOptions { Issuer = "issuer", Audience = "audience", SigningKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48)), AccessTokenMinutes = 10 };
        var profile = new User { Name = "Server name", Role = UserRole.Customer };
        var account = new ApplicationUser { Id = profile.Id, Profile = profile, SecurityStamp = Guid.NewGuid().ToString() };
        var response = new TokenIssuer(Options.Create(settings), new FixedClock(now)).Issue(account);
        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(response.AccessToken);
        Assert.Equal(profile.Id.ToString(), jwt.Subject);
        Assert.Equal("issuer", jwt.Issuer);
        Assert.Equal("audience", Assert.Single(jwt.Audiences));
        Assert.Equal("Customer", Assert.Single(jwt.Claims, x => x.Type == "role").Value);
        Assert.Equal(account.SecurityStamp, Assert.Single(jwt.Claims, x => x.Type == "session").Value);
        Assert.Equal(now.AddMinutes(10), response.ExpiresAt);
        Assert.Equal(now.AddMinutes(10).UtcDateTime, jwt.ValidTo);
        Assert.Equal(now.UtcDateTime, jwt.ValidFrom);
        Assert.Equal("HS256", jwt.Header.Alg);
        Assert.Equal(profile.Id, response.User.UserId);
    }

    [Fact]
    public void EachAccessTokenHasADistinctIdentifier()
    {
        var settings = new JwtOptions { Issuer = "issuer", Audience = "audience", SigningKey = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48)) };
        var account = new ApplicationUser { Profile = new User(), SecurityStamp = Guid.NewGuid().ToString() };
        var issuer = new TokenIssuer(Options.Create(settings), TimeProvider.System);
        var reader = new JwtSecurityTokenHandler();
        Assert.NotEqual(reader.ReadJwtToken(issuer.Issue(account).AccessToken).Id, reader.ReadJwtToken(issuer.Issue(account).AccessToken).Id);
    }

    [Fact]
    public void JwtConfigurationRequiresSecretsAndBoundedLifetime()
    {
        var options = new JwtOptions();
        Assert.False(Validator.TryValidateObject(options, new ValidationContext(options), [], true));
        options.Issuer = "issuer";
        options.Audience = "audience";
        options.SigningKey = new string('a', 32);
        Assert.True(Validator.TryValidateObject(options, new ValidationContext(options), [], true));
        options.AccessTokenMinutes = 0;
        Assert.False(Validator.TryValidateObject(options, new ValidationContext(options), [], true));
        options.AccessTokenMinutes = 61;
        Assert.False(Validator.TryValidateObject(options, new ValidationContext(options), [], true));
    }

    [Fact]
    public void BootstrapRejectsPartialCredentials()
    {
        Assert.True(new BootstrapAdminOptions().IsValid());
        Assert.True(new BootstrapAdminOptions { Email = "", Password = "" }.IsValid());
        Assert.False(new BootstrapAdminOptions { Email = "admin@example.test" }.IsValid());
        Assert.False(new BootstrapAdminOptions { Password = "ValidPassword1!" }.IsValid());
        Assert.False(new BootstrapAdminOptions { ExistingUserId = Guid.NewGuid() }.IsValid());
        Assert.False(new BootstrapAdminOptions { Email = "invalid", Password = "ValidPassword1!" }.IsValid());
    }

    private sealed class FixedClock(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
