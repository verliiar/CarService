using System.Net;
using CarService.Application;
using CarService.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace CarService.IntegrationTests;

[Collection("Postgres")]
public sealed class ErrorHandlingTests(ApiFixture fixture)
{
    [Theory]
    [InlineData("Production")]
    [InlineData("Development")]
    public async Task SessionValidationFailureDoesNotExposeInternalDetails(string environment)
    {
        await using var factory = fixture.WithWebHostBuilder(builder => builder.UseEnvironment(environment)
            .ConfigureServices(services =>
            {
                services.RemoveAll<IUserSessionValidator>();
                services.AddScoped<IUserSessionValidator, FailingSessionValidator>();
            }));
        using var client = factory.CreateClient();
        using var authenticated = fixture.CreateAuthenticatedClient(Guid.Parse("11111111-1111-1111-1111-111111111111"));
        client.DefaultRequestHeaders.Authorization = authenticated.DefaultRequestHeaders.Authorization;
        using var response = await client.GetAsync("/api/auth/me");
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var body = await response.Content.ReadAsStringAsync();
        Assert.DoesNotContain("PrivateInternalMarker", body, StringComparison.Ordinal);
        Assert.DoesNotContain("stackTrace", body, StringComparison.OrdinalIgnoreCase);
    }

    private sealed class FailingSessionValidator : IUserSessionValidator
    {
        public Task<bool> IsValidAsync(Guid userId, string securityStamp, UserRole role, CancellationToken cancellationToken) =>
            throw new InvalidOperationException("PrivateInternalMarker");
    }
}
