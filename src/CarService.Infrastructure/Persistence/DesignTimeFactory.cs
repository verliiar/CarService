using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CarService.Infrastructure;

public sealed class DesignTimeFactory : IDesignTimeDbContextFactory<DealershipDbContext>
{
    public DealershipDbContext CreateDbContext(string[] args) => new(new DbContextOptionsBuilder<DealershipDbContext>()
        .UseNpgsql(Environment.GetEnvironmentVariable("Database__ConnectionString") ?? "Host=localhost;Database=car_service;Username=car_service").Options);
}
