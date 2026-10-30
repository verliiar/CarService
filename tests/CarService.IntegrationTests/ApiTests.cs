using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using CarService.Application;
using CarService.Domain;
using CarService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace CarService.IntegrationTests;

[Collection("Postgres")]
public sealed class ApiTests(ApiFixture fixture) : IDisposable
{
    private static readonly Guid Admin = Guid.Parse("55555555-5555-5555-5555-555555555555");

    private static readonly Guid Customer = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid Manager = Guid.Parse("33333333-3333-3333-3333-333333333333");

    private static readonly Guid Warehouse = Guid.Parse("44444444-4444-4444-4444-444444444444");
    private readonly HttpClient client = fixture.CreateLegacyClient();

    private static async Task<T> Read<T>(HttpResponseMessage response)
    {
        Assert.True(response.IsSuccessStatusCode, $"{response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
        return (await response.Content.ReadFromJsonAsync<T>())!;
    }

    [Fact]
    public async Task MigrationsApplyAndSchemaHasNoPendingChanges()
    {
        await using var scope = fixture.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<DealershipDbContext>();
        Assert.NotEmpty(await db.Database.GetAppliedMigrationsAsync());
        Assert.Empty(await db.Database.GetPendingMigrationsAsync());
        Assert.False(db.Database.HasPendingModelChanges());
        Assert.Equal(4, await db.Set<ComponentCategoryEntry>().CountAsync());
        var seededNames = new[] { "17’’ Standard", "19’’ M-Sport", "18’’ Aero", "Автоматическая 8AT", "Механическая 6MT", "Спортивный кожаный (Standard)", "M-Sport с подогревом", "Тканевый Graphite", "Кожаный Dakota" };
        Assert.Equal(9, await db.Set<Component>().CountAsync(c => seededNames.Contains(c.Name)));
    }

    [Fact]
    public async Task OpenApiExposesAllSourceOperations()
    {
        var schema = await Read<JsonElement>(await client.GetAsync("/openapi/v1.json"));
        Assert.True(schema.GetProperty("paths").EnumerateObject().Count() >= 25);
    }

    [Fact]
    public async Task SessionsReopenWithSameIdAndKeepServerAssignedRole()
    {
        using var session = fixture.CreateClient();
        var email = Guid.NewGuid().ToString("N") + "@example.test";
        var auth = await Read<AuthResponse>(await session.PostAsJsonAsync("/api/auth/register", new RegisterRequest(email, fixture.Password, "Session customer")));
        session.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", auth.AccessToken);
        Assert.Equal(HttpStatusCode.NoContent, (await session.DeleteAsync($"/api/users/{auth.User.UserId}")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await session.GetAsync("/api/auth/me")).StatusCode);
        var reopened = await Read<AuthResponse>(await session.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, fixture.Password)));
        Assert.Equal(auth.User.UserId, reopened.User.UserId);
        Assert.Equal("Customer", reopened.User.Role);
        Assert.Equal(HttpStatusCode.Unauthorized, (await session.GetAsync("/api/auth/me")).StatusCode);
        session.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", reopened.AccessToken);
        Assert.Equal(HttpStatusCode.OK, (await session.GetAsync("/api/auth/me")).StatusCode);
    }

    [Fact]
    public async Task InvalidRoleAndInvalidBodyReturnProblemDetails()
    {
        using var adminClient = fixture.CreateAuthenticatedClient(Admin);
        var invalid = await adminClient.PutAsJsonAsync($"/api/users/{Customer}/role", new ChangeRoleRequest("Unknown"));
        Assert.Equal(HttpStatusCode.BadRequest, invalid.StatusCode);
        Assert.Equal("application/problem+json", invalid.Content.Headers.ContentType?.MediaType);
        Assert.Equal(400, (await invalid.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("status").GetInt32());
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync($"/api/cars/{Admin}", new
        {
            brand = "BMW"
        })).StatusCode);
    }

    [Fact]
    public async Task RolesAreEnforced()
    {
        Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync($"/api/cars/{Customer}")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await client.GetAsync($"/api/details/{Manager}/wheel")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync($"/api/orders/{Customer}/stock")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await client.GetAsync($"/api/cars/{Guid.NewGuid()}")).StatusCode);
    }

    [Fact]
    public async Task FilteringRunsAgainstPostgresAndUsesAllCriteria()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/cars/{Customer}/available/filtered?brand=BMW&details={Uri.EscapeDataString("17’’ Standard")}")
        {
            Content = JsonContent.Create(new CarFilter { Brand = "BMW", Model = "320i", EnginePowerLowerLimit = 180, EnginePowerUpperLimit = 200, EngineDisplacementEquals = 2 })
        };
        var list = await Read<List<CarSnapshot>>(await client.SendAsync(request));
        Assert.Equal("320i", Assert.Single(list).Model);
        using var invalid = new HttpRequestMessage(HttpMethod.Get, $"/api/cars/{Customer}/available/filtered") { Content = JsonContent.Create(new CarFilter { Model = "320i" }) };
        Assert.Equal(HttpStatusCode.BadRequest, (await client.SendAsync(invalid)).StatusCode);
        using var noBody = new HttpRequestMessage(HttpMethod.Get, $"/api/cars/{Customer}/available/filtered?brand=Unknown");
        Assert.Empty(await Read<List<CarSnapshot>>(await client.SendAsync(noBody)));
    }

    [Fact]
    public async Task CarCrudPreservesLinksAndSoftDeleteCanBeReactivated()
    {
        var model = Guid.NewGuid().ToString("N");
        var r = new CarRequest { DetailsName = ["17’’ Standard", "Тканевый Graphite"], Price = 100.25m, Brand = "Test", Model = model, Body = "Sedan", Fuel = "Petrol", EnginePower = 100, EngineDisplacement = 2, Color = "Blue", IsAvailable = true };
        Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync($"/api/cars/{Warehouse}", r)).StatusCode);
        var car = await Read<CarSnapshot>(await client.GetAsync($"/api/cars/{Customer}/Test/{model}"));
        Assert.Equal(100.25m, car.Price);
        Assert.Equal(2, car.RequiredDetails.Count);
        var renamed = model + "x";
        Assert.Equal(HttpStatusCode.NoContent, (await client.PutAsJsonAsync($"/api/cars/{Warehouse}", r with
        {
            NewModelName = renamed
        })).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/cars/{Admin}/Test/{model}")).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/cars/{Admin}/Test/{renamed}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/cars/{Admin}/Test/{renamed}")).StatusCode);
        Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync($"/api/cars/{Warehouse}", r with
        {
            Model = renamed
        })).StatusCode);
        Assert.Equal(2, (await Read<CarSnapshot>(await client.GetAsync($"/api/cars/{Admin}/Test/{renamed}"))).RequiredDetails.Count);
    }

    [Theory]
    [InlineData("interior")]
    [InlineData("steering_wheel")]
    [InlineData("transmission")]
    [InlineData("wheel")]
    public async Task EveryComponentSupportsCrudAndCompatibility(string category)
    {
        var name = Guid.NewGuid().ToString("N");
        var r = new DetailRequest { DetailName = name, SupportedCars = new() { ["BMW"] = "320i" }, AdditionalCost = 123.45m, Material = "Leather", Shape = "Round", Size = 38, Diameter = 19, LoadIndex = 95, Gearbox = "Automatic", WheelDrive = "All", EnergyTransfer = "Mechanical", Seasonality = "Summer" };
        var path = $"/api/details/{Admin}/{category}";
        Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync(path, r)).StatusCode);
        var created = await Read<JsonElement>(await client.GetAsync(path + "/" + name));
        Assert.Equal(123.45m, created.GetProperty("additionalCost").GetDecimal());
        var renamed = name + "x";
        Assert.Equal(HttpStatusCode.NoContent, (await client.PutAsJsonAsync(path, r with
        {
            NewDetailName = renamed,
            SupportedCars = new()
            {
                ["Toyota"] = "Camry"
            }
        })).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync(path + "/" + name)).StatusCode);
        await using (var scope = fixture.Services.CreateAsyncScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DealershipDbContext>();
            var detail = await db.Set<Component>().Include(c => c.SupportedCars).SingleAsync(c => c.Name == renamed);
            Assert.Equal("Camry", Assert.Single(detail.SupportedCars).Model);
        }
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync(path + "/" + renamed)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync(path + "/" + renamed)).StatusCode);
    }

    [Fact]
    public async Task CustomOrderPersistsChosenComponentsPriceAndStateWithoutChangingCatalog()
    {
        var options = new[] { "19’’ M-Sport", "Автоматическая 8AT", "Спортивный кожаный (Standard)", "Тканевый Graphite" };
        var order = await Read<OrderDto>(await client.PostAsJsonAsync($"/api/orders/{Customer}/custom/BMW/320i", options));
        Assert.Equal(3595000m, order.Car.Price);
        var saved = await Read<OrderDto>(await client.GetAsync($"/api/orders/{Admin}/{order.OrderId}"));
        Assert.Contains(saved.Car.RequiredDetails, d => d.Name == "19’’ M-Sport");
        Assert.Equal(order.Car.Price, saved.Car.Price);
        Assert.Equal(HttpStatusCode.Forbidden, (await client.PatchAsync($"/api/orders/{Manager}/{order.OrderId}/next_state", null)).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.PatchAsync($"/api/orders/{Warehouse}/{order.OrderId}/next_state", null)).StatusCode);
        Assert.Equal("Agreed", (await Read<OrderDto>(await client.GetAsync($"/api/orders/{Admin}/{order.OrderId}"))).State);
        Assert.Equal(HttpStatusCode.NoContent, (await client.PatchAsync($"/api/orders/{Customer}/{order.OrderId}/cancel", null)).StatusCode);
        Assert.Equal("Canceled", (await Read<OrderDto>(await client.GetAsync($"/api/orders/{Admin}/{order.OrderId}"))).State);
        var original = await Read<CarSnapshot>(await client.GetAsync($"/api/cars/{Customer}/BMW/320i"));
        Assert.Equal(3500000m, original.Price);
        Assert.Contains(original.RequiredDetails, d => d.Name == "17’’ Standard");
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync($"/api/orders/{Customer}/custom/BMW/320i", new[] { "18’’ Aero", options[1], options[2], options[3] })).StatusCode);
    }

    [Fact]
    public async Task StockOrdersAdvanceThroughLifecycleAndCanBeDeleted()
    {
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsync($"/api/orders/{Customer}/stock/Audi/A4", null)).StatusCode);
        var order = await Read<OrderDto>(await client.PostAsync($"/api/orders/{Customer}/stock/BMW/320i", null));
        foreach (var expected in new[] { "Agreed", "AwaitingPayment", "Paid", "ReadyForPickUp", "Completed" })
        {
            Assert.Equal(HttpStatusCode.NoContent, (await client.PatchAsync($"/api/orders/{Admin}/{order.OrderId}/next_state", null)).StatusCode);
            Assert.Equal(expected, (await Read<OrderDto>(await client.GetAsync($"/api/orders/{Admin}/{order.OrderId}"))).State);
        }
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PatchAsync($"/api/orders/{Admin}/{order.OrderId}/cancel", null)).StatusCode);
        Assert.Contains(await Read<List<OrderDto>>(await client.GetAsync($"/api/orders/{order.ManagerId}/stock")), o => o.OrderId == order.OrderId);
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/orders/{Admin}/{order.OrderId}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.GetAsync($"/api/orders/{Admin}/{order.OrderId}")).StatusCode);
    }

    [Fact]
    public async Task BookingValidatesTimeAndAvailabilityAndKeepsBookerOnEdit()
    {
        var request = new BookingRequest("BMW", "330i", DateTimeOffset.UtcNow.AddDays(20));
        Assert.Equal(HttpStatusCode.BadRequest, (await client.PostAsJsonAsync($"/api/test_drives/{Customer}/booking", request with
        {
            Date = DateTimeOffset.UtcNow.AddDays(-1)
        })).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.PostAsJsonAsync($"/api/test_drives/{Customer}/booking", request with
        {
            CarBrand = "Audi",
            CarModel = "A4"
        })).StatusCode);
        var booking = await Read<BookingDto>(await client.PostAsJsonAsync($"/api/test_drives/{Customer}/booking", request));
        Assert.Equal(HttpStatusCode.Conflict, (await client.PostAsJsonAsync($"/api/test_drives/{Customer}/booking", request)).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.PutAsJsonAsync($"/api/test_drives/{Admin}/{booking.BookingId}", request with
        {
            Date = request.Date.AddHours(1)
        })).StatusCode);
        var saved = Assert.Single(await Read<List<BookingDto>>(await client.GetAsync($"/api/test_drives/{Manager}")), b => b.BookingId == booking.BookingId);
        Assert.Equal(Customer, saved.BookerId);
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync($"/api/test_drives/{Admin}/{booking.BookingId}")).StatusCode);
        Assert.DoesNotContain(await Read<List<BookingDto>>(await client.GetAsync($"/api/test_drives/{Manager}")), b => b.BookingId == booking.BookingId);
    }

    [Fact]
    public async Task ConcurrentBookingsProduceOneSuccessAndOneConflict()
    {
        var r = new BookingRequest("BMW", "320i", DateTimeOffset.UtcNow.AddDays(100));
        var responses = await Task.WhenAll(Enumerable.Range(0, 2).Select(_ => client.PostAsJsonAsync($"/api/test_drives/{Customer}/booking", r)));
        Assert.Single(responses, x => x.StatusCode == HttpStatusCode.Created);
        Assert.Single(responses, x => x.StatusCode == HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task FleetRegistrationAndRemovalControlsAvailability()
    {
        var path = $"/api/test_drives/{Manager}/car/Volkswagen/Passat";
        Assert.Equal(HttpStatusCode.NoContent, (await client.PostAsync(path, null)).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await client.PostAsync(path, null)).StatusCode);
        var booking = await Read<BookingDto>(await client.PostAsJsonAsync($"/api/test_drives/{Customer}/booking", new BookingRequest("Volkswagen", "Passat", DateTimeOffset.UtcNow.AddDays(30))));
        Assert.Equal(HttpStatusCode.NoContent, (await client.DeleteAsync(path)).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await client.PostAsJsonAsync($"/api/test_drives/{Customer}/booking", new BookingRequest("Volkswagen", "Passat", DateTimeOffset.UtcNow.AddDays(31)))).StatusCode);
        Assert.Contains(await Read<List<BookingDto>>(await client.GetAsync($"/api/test_drives/{Manager}")), b => b.BookingId == booking.BookingId);
    }

    [Fact]
    public async Task BaseComponentsCannotBeRemovedOrMadeIncompatible()
    {
        var path = $"/api/details/{Admin}/wheel/" + Uri.EscapeDataString("17’’ Standard");
        Assert.Equal(HttpStatusCode.Conflict, (await client.DeleteAsync(path)).StatusCode);
        var request = new DetailRequest { DetailName = "17’’ Standard", NewDetailName = "17’’ Standard", SupportedCars = [], Diameter = 17, LoadIndex = 91, Seasonality = "All" };
        Assert.Equal(HttpStatusCode.Conflict, (await client.PutAsJsonAsync($"/api/details/{Admin}/wheel", request)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync(path)).StatusCode);
        var options = new[] { "17’’ Standard", "Автоматическая 8AT", "Спортивный кожаный (Standard)", "Тканевый Graphite" };
        Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync($"/api/orders/{Customer}/custom/BMW/320i", options)).StatusCode);
    }

    public void Dispose()
    {
        client.Dispose();
        GC.SuppressFinalize(this);
    }
}
