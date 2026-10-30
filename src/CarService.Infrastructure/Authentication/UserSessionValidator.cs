using CarService.Application;
using CarService.Domain;
using Microsoft.EntityFrameworkCore;

namespace CarService.Infrastructure;

internal sealed class UserSessionValidator(DealershipDbContext database, TimeProvider timeProvider) : IUserSessionValidator
{
    public Task<bool> IsValidAsync(Guid userId, string securityStamp, UserRole role, CancellationToken cancellationToken)
    {
        var now = timeProvider.GetUtcNow();
        return database.Set<ApplicationUser>().AnyAsync(account =>
            account.Id == userId && !account.Profile.IsRemoved &&
            account.SecurityStamp == securityStamp && account.Profile.Role == role &&
            (!account.LockoutEnabled || account.LockoutEnd == null || account.LockoutEnd <= now), cancellationToken);
    }
}
