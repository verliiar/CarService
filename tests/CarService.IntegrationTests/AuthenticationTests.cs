using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CarService.Application;
using CarService.Domain;
using CarService.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Xunit;

namespace CarService.IntegrationTests;

[Collection("Postgres")]
public sealed class AuthenticationTests(ApiFixture fixture)
{
    private static readonly Guid Admin = Guid.Parse("55555555-5555-5555-5555-555555555555");

    private static readonly Guid Customer = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private async Task<(HttpClient Client, AuthResponse Auth, string Email)> Register()
    {
        var client = fixture.CreateClient();
        var email = Guid.NewGuid().ToString("N") + "@example.test";
        using var response = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, fixture.Password, "Customer"));
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var auth = (await response.Content.ReadFromJsonAsync<AuthResponse>())!;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
        return (client, auth, email);
    }

    [Fact]
    public async Task RegistrationHashesPasswordAndAssignsOnlyCustomer()
    {
        var (client, auth, email) = await Register();
        using (client)
        {
            Assert.Equal("Customer", auth.User.Role);
            Assert.True(auth.ExpiresAt > DateTimeOffset.UtcNow);
            Assert.Equal(auth.User, await client.GetFromJsonAsync<UserDto>("/api/auth/me"));
            await using var scope = fixture.Services.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<DealershipDbContext>();
            var account = await db.Set<ApplicationUser>().SingleAsync(x => x.Email == email);
            Assert.NotEqual(fixture.Password, account.PasswordHash);
            Assert.NotNull(account.PasswordHash);
            Assert.Equal(PasswordVerificationResult.Success, new PasswordHasher<ApplicationUser>().VerifyHashedPassword(account, account.PasswordHash!, fixture.Password));
        }
    }

    [Fact]
    public async Task DuplicateEmailIsRejectedAfterNormalization()
    {
        var (client, _, email) = await Register();
        using (client)
        using (var response = await client.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email.ToUpperInvariant(), fixture.Password, "Another name")))
        {
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }
    }

    [Fact]
    public async Task LoginVerifiesCredentialsAndDoesNotDiscloseAccountExistence()
    {
        var (client, auth, email) = await Register();
        using (client)
        {
            using var wrong = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, "IncorrectPassword1!"));
            using var unknown = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(Guid.NewGuid() + "@example.test", "IncorrectPassword1!"));
            Assert.Equal(HttpStatusCode.Unauthorized, wrong.StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, unknown.StatusCode);
            var wrongProblem = await wrong.Content.ReadFromJsonAsync<JsonElement>();
            var unknownProblem = await unknown.Content.ReadFromJsonAsync<JsonElement>();
            Assert.Equal(wrongProblem.GetProperty("detail").GetString(), unknownProblem.GetProperty("detail").GetString());
            using var login = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email.ToUpperInvariant(), fixture.Password));
            Assert.Equal(HttpStatusCode.OK, login.StatusCode);
            Assert.Equal(auth.User.UserId, (await login.Content.ReadFromJsonAsync<AuthResponse>())!.User.UserId);
        }
    }

    [Theory]
    [InlineData("email", "invalid")]
    [InlineData("password", "weak")]
    [InlineData("name", " ")]
    public async Task InvalidRegistrationIsRejected(string field, string value)
    {
        using var client = fixture.CreateClient();
        var request = new Dictionary<string, string> { ["email"] = Guid.NewGuid() + "@example.test", ["password"] = fixture.Password, ["name"] = "Valid name" };
        request[field] = value;
        using var response = await client.PostAsJsonAsync("/api/auth/register", request);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("expired")]
    [InlineData("signature")]
    [InlineData("issuer")]
    [InlineData("audience")]
    [InlineData("algorithm")]
    [InlineData("no-expiration")]
    [InlineData("missing-subject")]
    [InlineData("malformed-subject")]
    [InlineData("missing-session")]
    [InlineData("missing-role")]
    [InlineData("numeric-role")]
    [InlineData("undefined-role")]
    public async Task InvalidJwtIsRejected(string defect)
    {
        var (client, auth, _) = await Register();
        using (client)
        {
            var original = new JwtSecurityTokenHandler().ReadJwtToken(auth.AccessToken);
            var claims = original.Claims.Where(x => x.Type is not ("iss" or "aud" or "exp" or "nbf")).ToList();
            if (defect is "missing-subject" or "malformed-subject")
            {
                claims.RemoveAll(claim => claim.Type == "sub");
                if (defect == "malformed-subject")
                {
                    claims.Add(new Claim("sub", "invalid"));
                }
            }
            if (defect == "missing-session")
            {
                claims.RemoveAll(claim => claim.Type == "session");
            }

            if (defect is "missing-role" or "numeric-role" or "undefined-role")
            {
                claims.RemoveAll(claim => claim.Type == "role");
                if (defect != "missing-role")
                {
                    claims.Add(new Claim("role", defect == "numeric-role" ? "2" : "Unknown"));
                }
            }
            var now = DateTime.UtcNow;
            var key = defect == "signature" ? Convert.ToBase64String(RandomNumberGenerator.GetBytes(48)) : fixture.SigningKey;
            var token = new JwtSecurityToken(defect == "issuer" ? "Attacker" : "CarService", defect == "audience" ? "OtherApi" : "CarService.Api",
                claims, now.AddMinutes(-20), defect == "no-expiration" ? null : defect == "expired" ? now.AddMinutes(-1) : now.AddMinutes(10),
                new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), defect == "algorithm" ? SecurityAlgorithms.HmacSha384 : SecurityAlgorithms.HmacSha256));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", new JwtSecurityTokenHandler().WriteToken(token));
            using var response = await client.GetAsync("/api/auth/me");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Contains(response.Headers.WwwAuthenticate, x => x.Scheme == "Bearer");
        }
    }

    [Theory]
    [InlineData("/api/auth/me")]
    [InlineData("/api/cars")]
    [InlineData("/api/details/wheel")]
    [InlineData("/api/orders/stock")]
    [InlineData("/api/test_drives")]
    public async Task ProtectedEndpointsRequireToken(string path)
    {
        using var client = fixture.CreateClient();
        using var response = await client.GetAsync(path);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task LeastPrivilegeAndLegacyIdentifierSpoofingAreEnforced()
    {
        using var customer = fixture.CreateAuthenticatedClient(Customer);
        Assert.Equal(HttpStatusCode.Forbidden, (await customer.GetAsync("/api/details/wheel")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await customer.GetAsync($"/api/cars/{Admin}")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await customer.DeleteAsync($"/api/users/{Admin}")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await customer.PutAsJsonAsync($"/api/users/{Customer}/role", new ChangeRoleRequest("Manager"))).StatusCode);
        using var admin = fixture.CreateAuthenticatedClient(Admin);
        Assert.Equal(HttpStatusCode.OK, (await admin.GetAsync("/api/details/wheel")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await admin.GetAsync("/api/cars")).StatusCode);
    }

    [Fact]
    public async Task RegistrationAndLegacySessionCannotEscalateRole()
    {
        using var client = fixture.CreateClient();
        using var registration = await client.PostAsJsonAsync("/api/auth/register", new
        {
            email = Guid.NewGuid() + "@example.test",
            password = fixture.Password,
            name = "Attacker",
            role = "SystemAdmin"
        });
        Assert.Equal(HttpStatusCode.BadRequest, registration.StatusCode);
        using var legacy = await client.PostAsync("/api/users/SystemAdmin/Attacker", null);
        Assert.Equal(HttpStatusCode.Gone, legacy.StatusCode);
        var (customer, auth, _) = await Register();
        using (customer)
        using (var admin = fixture.CreateAuthenticatedClient(Admin))
        {
            Assert.Equal(HttpStatusCode.Forbidden, (await admin.PutAsJsonAsync($"/api/users/{auth.User.UserId}/role", new ChangeRoleRequest("SystemAdmin"))).StatusCode);
        }
    }

    [Fact]
    public async Task RoleChangeRevokesTokensAndCannotResurrectThem()
    {
        var (client, auth, email) = await Register();
        using (client)
        using (var admin = fixture.CreateAuthenticatedClient(Admin))
        {
            Assert.Equal(HttpStatusCode.NoContent, (await admin.PutAsJsonAsync($"/api/users/{auth.User.UserId}/role", new ChangeRoleRequest("WarehouseAdmin"))).StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/auth/me")).StatusCode);
            using var login = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, fixture.Password));
            var promoted = (await login.Content.ReadFromJsonAsync<AuthResponse>())!;
            Assert.Equal("WarehouseAdmin", promoted.User.Role);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", promoted.AccessToken);
            Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/details/wheel")).StatusCode);
            Assert.Equal(HttpStatusCode.NoContent, (await admin.PutAsJsonAsync($"/api/users/{auth.User.UserId}/role", new ChangeRoleRequest("Customer"))).StatusCode);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
            Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/auth/me")).StatusCode);
        }
    }

    [Fact]
    public async Task PasswordFailuresLockOutAccount()
    {
        var (client, _, email) = await Register();
        using (client)
        {
            for (var i = 0; i < 5; i++)
            {
                Assert.Equal(HttpStatusCode.Unauthorized, (await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, "WrongPassword1!"))).StatusCode);
            }

            Assert.Equal(HttpStatusCode.Unauthorized, (await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, fixture.Password))).StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/auth/me")).StatusCode);
        }
    }

    [Fact]
    public async Task AnotherCustomersOrderCannotBeReadChangedOrListed()
    {
        var (owner, auth, _) = await Register();
        var (other, _, _) = await Register();
        using (owner)
        using (other)
        {
            using var creation = await owner.PostAsync($"/api/orders/stock/BMW/320i?userId={Admin}", null);
            Assert.Equal(HttpStatusCode.Created, creation.StatusCode);
            var order = (await creation.Content.ReadFromJsonAsync<OrderDto>())!;
            Assert.Equal(HttpStatusCode.Forbidden, (await other.GetAsync($"/api/orders/{order.OrderId}")).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await other.PatchAsync($"/api/orders/{order.OrderId}/cancel", null)).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await other.PatchAsync($"/api/orders/{order.OrderId}/next_state", null)).StatusCode);
            Assert.DoesNotContain((await other.GetFromJsonAsync<List<OrderDto>>("/api/orders/stock"))!, x => x.OrderId == order.OrderId);
            Assert.Equal(HttpStatusCode.OK, (await owner.GetAsync($"/api/orders/{order.OrderId}")).StatusCode);
            Assert.Equal(HttpStatusCode.NoContent, (await owner.PatchAsync($"/api/orders/{order.OrderId}/cancel", null)).StatusCode);
            await using var scope = fixture.Services.CreateAsyncScope();
            Assert.Equal(auth.User.UserId, (await scope.ServiceProvider.GetRequiredService<DealershipDbContext>().Set<Order>().SingleAsync(x => x.Id == order.OrderId)).CustomerId);
        }
    }

    [Fact]
    public async Task AnotherCustomersBookingCannotBeManagedOrListed()
    {
        var (owner, auth, _) = await Register();
        var (other, _, _) = await Register();
        using (owner)
        using (other)
        {
            var request = new BookingRequest("BMW", "320i", DateTimeOffset.UtcNow.AddDays(60).AddMinutes(Random.Shared.Next(10000)));
            using var response = await owner.PostAsJsonAsync("/api/test_drives/booking", new
            {
                request.CarBrand,
                request.CarModel,
                request.Date,
                bookerId = Admin
            });
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var booking = (await response.Content.ReadFromJsonAsync<BookingDto>())!;
            Assert.Equal(auth.User.UserId, booking.BookerId);
            Assert.Equal(HttpStatusCode.Forbidden, (await other.PutAsJsonAsync($"/api/test_drives/{booking.BookingId}", request with
            {
                Date = request.Date.AddHours(1)
            })).StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, (await other.DeleteAsync($"/api/test_drives/{booking.BookingId}")).StatusCode);
            Assert.DoesNotContain((await other.GetFromJsonAsync<List<BookingDto>>("/api/test_drives"))!, x => x.BookingId == booking.BookingId);
            Assert.Equal(HttpStatusCode.NoContent, (await owner.PutAsJsonAsync($"/api/test_drives/{booking.BookingId}", request with
            {
                Date = request.Date.AddHours(1)
            })).StatusCode);
            Assert.Equal(HttpStatusCode.NoContent, (await owner.DeleteAsync($"/api/test_drives/{booking.BookingId}")).StatusCode);
        }
    }

    [Fact]
    public async Task LogoutInvalidatesJwtAndLoginDoesNotReactivateOldToken()
    {
        var (client, auth, email) = await Register();
        using (client)
        {
            Assert.Equal(HttpStatusCode.NoContent, (await client.PostAsync("/api/auth/logout", null)).StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/auth/me")).StatusCode);
            Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, fixture.Password))).StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync("/api/auth/me")).StatusCode);
            Assert.NotEmpty(auth.AccessToken);
        }
    }

    [Fact]
    public async Task OpenApiDescribesBearerOnlyForProtectedOperations()
    {
        using var client = fixture.CreateClient();
        var schema = await client.GetFromJsonAsync<JsonElement>("/openapi/v1.json");
        Assert.Equal("bearer", schema.GetProperty("components").GetProperty("securitySchemes").GetProperty("Bearer").GetProperty("scheme").GetString());
        var paths = schema.GetProperty("paths");
        Assert.True(paths.GetProperty("/api/orders/stock").GetProperty("get").GetProperty("security").GetArrayLength() > 0);
        Assert.False(paths.GetProperty("/api/auth/login").GetProperty("post").TryGetProperty("security", out _));
    }

    [Fact]
    public async Task LegacyAccountRequiresExplicitAdministratorProvisioning()
    {
        var legacy = new User { Name = "Legacy customer " + Guid.NewGuid().ToString("N"), Role = UserRole.Customer };
        Order order;
        await using (var scope = fixture.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DealershipDbContext>();
            var car = await db.Set<Car>().Include(x => x.RequiredDetails).SingleAsync(x => x.Brand == "BMW" && x.Model == "320i");
            order = new Order { CustomerId = legacy.Id, ManagerId = Guid.Parse("33333333-3333-3333-3333-333333333333"), CarId = car.Id, Configuration = CarSnapshot.From(car) };
            db.Add(legacy);
            db.Add(order);
            await db.SaveAsync(default);
        }
        using var anonymous = fixture.CreateClient();
        var claimed = await anonymous.PostAsJsonAsync("/api/auth/register", new RegisterRequest(Guid.NewGuid() + "@example.test", fixture.Password, legacy.Name));
        Assert.Equal(HttpStatusCode.Created, claimed.StatusCode);
        var newAccount = (await claimed.Content.ReadFromJsonAsync<AuthResponse>())!;
        Assert.NotEqual(legacy.Id, newAccount.User.UserId);
        using var impostor = fixture.CreateClient();
        impostor.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", newAccount.AccessToken);
        Assert.Equal(HttpStatusCode.Forbidden, (await impostor.GetAsync($"/api/orders/{order.Id}")).StatusCode);
        var request = new ProvisionAccountRequest(Guid.NewGuid() + "@example.test", fixture.Password);
        var path = $"/api/users/{legacy.Id}/credentials";
        Assert.Equal(HttpStatusCode.Unauthorized, (await anonymous.PostAsJsonAsync(path, request)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await impostor.PostAsJsonAsync(path, request)).StatusCode);
        using var admin = fixture.CreateAuthenticatedClient(Admin);
        Assert.Equal(HttpStatusCode.Created, (await admin.PostAsJsonAsync(path, request)).StatusCode);
        Assert.Equal(HttpStatusCode.Conflict, (await admin.PostAsJsonAsync(path, request)).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await admin.PostAsJsonAsync($"/api/users/{Admin}/credentials", request)).StatusCode);
        using var login = await anonymous.PostAsJsonAsync("/api/auth/login", new LoginRequest(request.Email, request.Password));
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        var auth = (await login.Content.ReadFromJsonAsync<AuthResponse>())!;
        Assert.Equal(legacy.Id, auth.User.UserId);
        anonymous.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.AccessToken);
        Assert.Equal(HttpStatusCode.OK, (await anonymous.GetAsync($"/api/orders/{order.Id}")).StatusCode);
    }

    [Fact]
    public async Task ConcurrentRegistrationCannotCreateDuplicateAccounts()
    {
        using var client = fixture.CreateClient();
        var email = Guid.NewGuid() + "@example.test";
        var request = new RegisterRequest(email, fixture.Password, "Concurrent customer");
        var responses = await Task.WhenAll(client.PostAsJsonAsync("/api/auth/register", request), client.PostAsJsonAsync("/api/auth/register", request));
        try
        {
            Assert.Single(responses, x => x.StatusCode == HttpStatusCode.Created);
            Assert.Single(responses, x => x.StatusCode == HttpStatusCode.Conflict);
            await using var scope = fixture.Services.CreateAsyncScope();
            Assert.Equal(1, await scope.ServiceProvider.GetRequiredService<DealershipDbContext>().Set<ApplicationUser>().CountAsync(x => x.Email == email));
        }
        finally
        {
            foreach (var response in responses)
            {
                response.Dispose();
            }
        }
    }
}
