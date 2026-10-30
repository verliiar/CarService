using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Cryptography;
using CarService.Application;
using CarService.Domain;
using CarService.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace CarService.IntegrationTests;

public sealed class ApiFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer postgres = new PostgreSqlBuilder("postgres:16-alpine").Build();

    private readonly Dictionary<Guid, string> tokens = [];
    public string SigningKey { get; } = Convert.ToBase64String(RandomNumberGenerator.GetBytes(48));

    public string Password { get; } = "Aa1!" + Guid.NewGuid().ToString("N");
    public string ConnectionString => postgres.GetConnectionString();

    protected override void ConfigureWebHost(IWebHostBuilder builder) => builder.ConfigureAppConfiguration((_, configuration) =>
        configuration.AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Database:ConnectionString"] = ConnectionString,
            ["Database:MigrateOnStartup"] = "true",
            ["Jwt:SigningKey"] = SigningKey,
            ["Jwt:Issuer"] = "CarService",
            ["Jwt:Audience"] = "CarService.Api",
            ["BootstrapAdmin:Email"] = null,
            ["BootstrapAdmin:Password"] = null
        }));

    public async Task InitializeAsync()
    {
        await postgres.StartAsync();
        using var client = CreateClient();
        await using (var scope = Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DealershipDbContext>();
            var manager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            foreach (var profile in await db.Set<User>().ToListAsync())
            {
                var email = profile.Id.ToString("N") + "@example.test";
                var result = await manager.CreateAsync(new ApplicationUser { Id = profile.Id, Profile = profile, Email = email, UserName = email, LockoutEnabled = true }, Password);
                Assert.True(result.Succeeded, string.Join(", ", result.Errors.Select(x => x.Code)));
                using var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, Password));
                response.EnsureSuccessStatusCode();
                tokens[profile.Id] = (await response.Content.ReadFromJsonAsync<AuthResponse>())!.AccessToken;
            }
        }
    }

    public HttpClient CreateLegacyClient() => CreateDefaultClient(new LegacyIdentityHandler(tokens));

    public HttpClient CreateAuthenticatedClient(Guid id)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens[id]);
        return client;
    }
    async Task IAsyncLifetime.DisposeAsync()
    {
        await base.DisposeAsync();
        await postgres.DisposeAsync();
    }

    private sealed class LegacyIdentityHandler(IReadOnlyDictionary<Guid, string> tokens) : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
        {
            var parts = request.RequestUri!.AbsolutePath.Split('/');
            if (request.Headers.Authorization == null && parts.Length > 3 && Guid.TryParse(parts[3], out var id) && tokens.TryGetValue(id, out var token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return base.SendAsync(request, ct);
        }
    }
}
[CollectionDefinition("Postgres")]
public sealed class PostgresFixtureDefinition : ICollectionFixture<ApiFixture>;
