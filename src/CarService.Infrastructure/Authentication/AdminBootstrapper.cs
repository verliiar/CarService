using CarService.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CarService.Infrastructure;

public sealed class AdminBootstrapper(DealershipDbContext db, UserManager<ApplicationUser> accounts, IOptions<BootstrapAdminOptions> options)
{
    public async Task InitializeAsync(CancellationToken ct)
    {
        var config = options.Value;
        if (string.IsNullOrEmpty(config.Email))
        {
            return;
        }

        var existing = await accounts.FindByEmailAsync(config.Email.Trim());
        if (existing != null)
        {
            await db.Entry(existing).Reference(x => x.Profile).LoadAsync(ct);
            if (existing.Profile.Role != UserRole.SystemAdmin || config.ExistingUserId is { } id && id != existing.Id)
            {
                throw new InvalidOperationException("Bootstrap administrator conflicts with an existing account.");
            }

            return;
        }
        if (await db.Set<ApplicationUser>().AnyAsync(x => x.Profile.Role == UserRole.SystemAdmin, ct))
        {
            throw new InvalidOperationException("An administrator already exists. Bootstrap provisioning is only available for initial setup.");
        }

        var profile = config.ExistingUserId is { } userId
            ? await db.Set<User>().SingleOrDefaultAsync(x => x.Id == userId && x.Role == UserRole.SystemAdmin, ct)
                ?? throw new InvalidOperationException("The selected legacy administrator does not exist.")
            : new User { Name = config.Name, Role = UserRole.SystemAdmin };
        if (config.ExistingUserId == null)
        {
            db.Add(profile);
        }

        profile.IsRemoved = false;
        var email = config.Email.Trim();
        var account = new ApplicationUser { Id = profile.Id, Profile = profile, Email = email, UserName = email, LockoutEnabled = true };
        var result = await accounts.CreateAsync(account, config.Password!);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException("Administrator provisioning failed. Check bootstrap configuration and password requirements.");
        }
    }
}
