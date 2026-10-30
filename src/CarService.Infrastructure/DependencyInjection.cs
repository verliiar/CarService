using CarService.Application;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace CarService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddOptions<DatabaseOptions>().BindConfiguration("Database").ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<JwtOptions>().BindConfiguration("Jwt").ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<BootstrapAdminOptions>().BindConfiguration("BootstrapAdmin")
            .Validate(options => options.IsValid(), "Supply a valid bootstrap email, password and name together.").ValidateOnStart();
        services.AddDbContext<DealershipDbContext>((provider, options) =>
            options.UseNpgsql(provider.GetRequiredService<IOptions<DatabaseOptions>>().Value.ConnectionString));
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<DealershipDbContext>());
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddSingleton(TimeProvider.System);
        services.AddIdentityCore<ApplicationUser>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequiredLength = 12;
            options.Password.RequiredUniqueChars = 4;
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
        }).AddEntityFrameworkStores<DealershipDbContext>().AddSignInManager();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IUserSessionValidator, UserSessionValidator>();
        services.AddScoped<TokenIssuer>();
        services.AddScoped<AdminBootstrapper>();
        return services;
    }

    public static async Task InitializeInfrastructureAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var provider = scope.ServiceProvider;
        if (provider.GetRequiredService<IOptions<DatabaseOptions>>().Value.MigrateOnStartup)
        {
            await provider.GetRequiredService<DealershipDbContext>().Database.MigrateAsync(cancellationToken);
        }

        await provider.GetRequiredService<AdminBootstrapper>().InitializeAsync(cancellationToken);
    }
}
