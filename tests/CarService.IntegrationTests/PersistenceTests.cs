using CarService.Domain;
using CarService.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Npgsql;
using Xunit;

namespace CarService.IntegrationTests;

[Collection("Postgres")]
public sealed class PersistenceTests(ApiFixture fixture)
{
    private async Task<DealershipDbContext> CreateDatabase()
    {
        var database = "audit_" + Guid.NewGuid().ToString("N");
        await using var connection = new NpgsqlConnection(fixture.ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand($"CREATE DATABASE \"{database}\"", connection);
        await command.ExecuteNonQueryAsync();
        var builder = new NpgsqlConnectionStringBuilder(fixture.ConnectionString) { Database = database };
        return new DealershipDbContext(new DbContextOptionsBuilder<DealershipDbContext>().UseNpgsql(builder.ConnectionString).Options);
    }

    [Fact]
    public async Task MigrationUpgradesPreviousSchemaAndPreservesExistingData()
    {
        await using var db = await CreateDatabase();
        await db.GetService<IMigrator>().MigrateAsync("20261030203015_EnumConstraints");
        var legacyUser = new User { Name = "Existing customer", Role = UserRole.Customer };
        db.Add(legacyUser);
        await db.SaveAsync(default);
        var orders = await db.Set<Order>().AsNoTracking().OrderBy(x => x.Id).ToListAsync();
        var bookings = await db.Set<TestDriveBooking>().AsNoTracking().OrderBy(x => x.Id).ToListAsync();
        var configurations = orders.Select(x => System.Text.Json.JsonSerializer.Serialize(x.Configuration)).ToList();
        db.ChangeTracker.Clear();
        await db.Database.MigrateAsync();
        Assert.Empty(await db.Database.GetPendingMigrationsAsync());
        Assert.False(db.Database.HasPendingModelChanges());
        Assert.Equal(6, await db.Set<User>().CountAsync());
        Assert.Equal("Existing customer", (await db.Set<User>().SingleAsync(x => x.Id == legacyUser.Id)).Name);
        Assert.Empty(await db.Set<ApplicationUser>().ToListAsync());
        Assert.Equal(orders.Select(x => x.Id), await db.Set<Order>().OrderBy(x => x.Id).Select(x => x.Id).ToListAsync());
        Assert.Equal(configurations, (await db.Set<Order>().AsNoTracking().OrderBy(x => x.Id).ToListAsync()).Select(x => System.Text.Json.JsonSerializer.Serialize(x.Configuration)));
        Assert.Equal(bookings.Select(x => x.Id), await db.Set<TestDriveBooking>().OrderBy(x => x.Id).Select(x => x.Id).ToListAsync());
    }

    [Theory]
    [InlineData("car")]
    [InlineData("component")]
    [InlineData("order")]
    [InlineData("booking")]
    public async Task ConcurrentChangesDoNotOverwriteData(string entityType)
    {
        await using var first = await CreateDatabase();
        await first.Database.MigrateAsync();
        await using var second = new DealershipDbContext(new DbContextOptionsBuilder<DealershipDbContext>().UseNpgsql(first.Database.GetConnectionString()).Options);
        Entity a;
        Entity b;
        switch (entityType)
        {
            case "car":
                a = await first.Set<Car>().FirstAsync();
                b = await second.Set<Car>().SingleAsync(x => x.Id == a.Id);
                ((Car)a).Color = "Green";
                ((Car)b).Color = "Blue";
                break;
            case "component":
                a = await first.Set<Component>().FirstAsync();
                b = await second.Set<Component>().SingleAsync(x => x.Id == a.Id);
                ((Component)a).AdditionalCost = 1;
                ((Component)b).AdditionalCost = 2;
                break;
            case "order":
                a = await first.Set<Order>().FirstAsync();
                b = await second.Set<Order>().SingleAsync(x => x.Id == a.Id);
                ((Order)a).State = OrderState.Canceled;
                ((Order)b).State = OrderState.Completed;
                break;
            default:
                a = await first.Set<TestDriveBooking>().FirstAsync();
                b = await second.Set<TestDriveBooking>().SingleAsync(x => x.Id == a.Id);
                ((TestDriveBooking)a).BookingDateTime = DateTimeOffset.UtcNow.AddDays(1);
                ((TestDriveBooking)b).BookingDateTime = DateTimeOffset.UtcNow.AddDays(2);
                break;
        }
        await first.SaveAsync(default);
        var exception = await Assert.ThrowsAsync<BusinessException>(() => second.SaveAsync(default));
        Assert.Equal(FailureKind.Conflict, exception.Kind);
    }

    [Fact]
    public async Task BootstrapCreatesInitialAdminIdempotentlyAndCanLinkExplicitLegacyAdmin()
    {
        await using var db = await CreateDatabase();
        await db.Database.MigrateAsync();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(db);
        services.AddIdentityCore<ApplicationUser>(options => options.User.RequireUniqueEmail = true).AddEntityFrameworkStores<DealershipDbContext>();
        await using var provider = services.BuildServiceProvider();
        var manager = provider.GetRequiredService<UserManager<ApplicationUser>>();
        var id = Guid.Parse("55555555-5555-5555-5555-555555555555");
        var options = Options.Create(new BootstrapAdminOptions { Email = "admin@example.test", Password = fixture.Password, ExistingUserId = id });
        var bootstrap = new AdminBootstrapper(db, manager, options);
        await bootstrap.InitializeAsync(default);
        await bootstrap.InitializeAsync(default);
        var account = Assert.Single(await db.Set<ApplicationUser>().Include(x => x.Profile).ToListAsync());
        Assert.Equal(id, account.Id);
        Assert.Equal(UserRole.SystemAdmin, account.Profile.Role);
        Assert.True(await manager.CheckPasswordAsync(account, fixture.Password));
        var other = new AdminBootstrapper(db, manager, Options.Create(new BootstrapAdminOptions { Email = "another@example.test", Password = fixture.Password }));
        await Assert.ThrowsAsync<InvalidOperationException>(() => other.InitializeAsync(default));
    }

    [Fact]
    public async Task BootstrapDoesNotPromoteExistingCustomer()
    {
        await using var scope = fixture.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<DealershipDbContext>();
        var manager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var customer = await db.Set<ApplicationUser>().Include(x => x.Profile).FirstAsync(x => x.Profile.Role == UserRole.Customer);
        var bootstrap = new AdminBootstrapper(db, manager, Options.Create(new BootstrapAdminOptions { Email = customer.Email, Password = fixture.Password }));
        await Assert.ThrowsAsync<InvalidOperationException>(() => bootstrap.InitializeAsync(default));
        Assert.Equal(UserRole.Customer, customer.Profile.Role);
    }

    [Fact]
    public async Task ConcurrentCollectionEditsUpdateAggregateVersion()
    {
        await using var first = await CreateDatabase();
        await first.Database.MigrateAsync();
        await using var second = new DealershipDbContext(new DbContextOptionsBuilder<DealershipDbContext>().UseNpgsql(first.Database.GetConnectionString()).Options);
        var car = await first.Set<Car>().Include(x => x.RequiredDetails).SingleAsync(x => x.Brand == "BMW" && x.Model == "320i");
        var other = await second.Set<Car>().Include(x => x.RequiredDetails).SingleAsync(x => x.Id == car.Id);
        car.RequiredDetails.Add(await first.Set<Component>().SingleAsync(x => x.Name == "19’’ M-Sport"));
        other.RequiredDetails.Add(await second.Set<Component>().SingleAsync(x => x.Name == "Кожаный Dakota"));
        await first.SaveAsync(default);
        Assert.Equal(FailureKind.Conflict, (await Assert.ThrowsAsync<BusinessException>(() => second.SaveAsync(default))).Kind);
    }
}
