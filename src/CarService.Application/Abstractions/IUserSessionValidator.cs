using CarService.Domain;

namespace CarService.Application;

public interface IUserSessionValidator
{
    Task<bool> IsValidAsync(Guid userId, string securityStamp, UserRole role, CancellationToken cancellationToken);
}
