using System.ComponentModel.DataAnnotations;

namespace CarService.Infrastructure;

public sealed class BootstrapAdminOptions
{
    public string? Email
    {
        get; set;
    }

    public string? Password
    {
        get; set;
    }

    public string Name { get; set; } = "Administrator";

    public Guid? ExistingUserId
    {
        get; set;
    }

    public bool IsValid() => string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Password) && ExistingUserId == null ||
        Email is { Length: <= 254 } && new EmailAddressAttribute().IsValid(Email) &&
        Password is { Length: >= 12 and <= 128 } && !string.IsNullOrWhiteSpace(Name) && Name.Length <= 64;
}
