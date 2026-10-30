using CarService.Domain;
using Microsoft.AspNetCore.Identity;

namespace CarService.Infrastructure;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public User Profile { get; set; } = null!;
}
