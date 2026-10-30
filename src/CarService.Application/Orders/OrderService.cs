using CarService.Domain;

namespace CarService.Application;

public sealed class OrderService(IRepository<Order> orders, IRepository<User> userRepository, CarCatalog cars, UserService users, IUnitOfWork unit)
{
    private static OrderDto Map(Order o) => new(o.State.ToString(), o.Type.ToString(), o.Id, o.ManagerId, o.Configuration);

    private async Task<Order> FindAsync(Guid id, CancellationToken ct, bool tracking = true) => Rules.Found((await orders.ListAsync(q => q.Where(o => o.Id == id && !o.IsRemoved), ct, tracking)).SingleOrDefault());
    public async Task<OrderDto> CreateAsync(Guid userId, string brand, string model, List<string>? options, CancellationToken ct)
    {
        await users.RequireAsync(userId, ct, UserRole.Customer, UserRole.SystemAdmin);
        var car = await cars.FindAsync(brand, model, ct);
        if (car.RequiredDetails.Count == 0 || car.RequiredDetails.Any(c => c.IsRemoved))
        {
            throw new BusinessException(FailureKind.Validation, "Car has no valid base configuration.");
        }

        if (options == null && !car.IsAvailable)
        {
            throw new BusinessException(FailureKind.Validation, "The selected car is not available.");
        }

        var config = options == null ? CarSnapshot.From(car) : Configurator.Configure(car, await cars.ResolveAsync(options, ct));
        var managers = await userRepository.ListAsync(q => q.Where(u => !u.IsRemoved && u.Role == UserRole.Manager), ct, false);
        if (managers.Count == 0)
        {
            throw new BusinessException(FailureKind.Conflict, "No active manager is available.");
        }

        var order = new Order { CustomerId = userId, ManagerId = managers[Random.Shared.Next(managers.Count)].Id, CarId = car.Id, Type = options == null ? OrderType.Stock : OrderType.Custom, Configuration = config };
        orders.Add(order);
        await unit.SaveAsync(ct);
        return Map(order);
    }

    public async Task ChangeAsync(Guid userId, Guid id, bool cancel, CancellationToken ct)
    {
        var role = await users.RequireAsync(userId, ct);
        var order = await FindAsync(id, ct);
        EnsureAccess(order, userId, role);
        if (cancel)
        {
            order.Cancel(role);
        }
        else
        {
            order.Advance(role);
        }

        await unit.SaveAsync(ct);
    }

    public async Task<OrderDto> ReadAsync(Guid userId, Guid id, CancellationToken ct)
    {
        var role = await users.RequireAsync(userId, ct);
        var order = await FindAsync(id, ct, false);
        EnsureAccess(order, userId, role);
        return Map(order);
    }

    public async Task<List<OrderDto>> ListAsync(Guid userId, OrderType type, CancellationToken ct)
    {
        var role = await users.RequireAsync(userId, ct);
        return (await orders.ListAsync(q => q.Where(o => !o.IsRemoved && o.Type == type &&
            (role == UserRole.SystemAdmin || role == UserRole.WarehouseAdmin ||
             (role == UserRole.Customer && o.CustomerId == userId) || (role == UserRole.Manager && o.ManagerId == userId))).OrderBy(o => o.CreatedAt), ct, false)).Select(Map).ToList();
    }

    private static void EnsureAccess(Order order, Guid userId, UserRole role)
    {
        if ((role == UserRole.Customer && order.CustomerId != userId) || (role == UserRole.Manager && order.ManagerId != userId))
        {
            throw new BusinessException(FailureKind.Forbidden, "No permission to access this order.");
        }
    }

    public async Task DeleteAsync(Guid userId, Guid id, CancellationToken ct)
    {
        await users.RequireAsync(userId, ct, UserRole.SystemAdmin);
        (await FindAsync(id, ct)).IsRemoved = true;
        await unit.SaveAsync(ct);
    }
}
