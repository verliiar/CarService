using CarService.Domain;

namespace CarService.Application;

public sealed class UserService(IRepository<User> users, IUnitOfWork unit)
{
    public async Task<UserRole> RequireAsync(Guid id, CancellationToken ct, params UserRole[] roles)
    {
        var user = Rules.Found((await users.ListAsync(q => q.Where(u => u.Id == id && !u.IsRemoved), ct, false)).SingleOrDefault());
        if (roles.Length > 0 && !roles.Contains(user.Role))
        {
            throw new BusinessException(FailureKind.Forbidden, "No permission to perform this operation.");
        }

        return user.Role;
    }

    public async Task CloseAsync(Guid id, CancellationToken ct)
    {
        var user = Rules.Found((await users.ListAsync(q => q.Where(u => u.Id == id && !u.IsRemoved), ct)).SingleOrDefault());
        user.IsRemoved = true;
        await unit.SaveAsync(ct);
    }
}
