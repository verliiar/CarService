using System.Linq.Expressions;
using CarService.Application;
using CarService.Domain;
using Xunit;

namespace CarService.UnitTests;

public sealed class ResourceAccessTests
{
    [Theory]
    [InlineData(UserRole.Customer)]
    [InlineData(UserRole.Manager)]
    public async Task AnotherUsersOrderCannotBeReadOrChanged(UserRole role)
    {
        var user = new User { Role = role };
        var unit = new MemoryUnit();
        var users = new UserService(new MemoryRepository<User>(user), unit);
        var order = new Order { CustomerId = Guid.NewGuid(), ManagerId = Guid.NewGuid(), State = OrderState.ReadyForPickUp, Configuration = Snapshot() };
        var service = new OrderService(new MemoryRepository<Order>(order), new MemoryRepository<User>(user), null!, users, unit);
        Assert.Equal(FailureKind.Forbidden, (await Assert.ThrowsAsync<BusinessException>(() => service.ReadAsync(user.Id, order.Id, default))).Kind);
        Assert.Equal(FailureKind.Forbidden, (await Assert.ThrowsAsync<BusinessException>(() => service.ChangeAsync(user.Id, order.Id, false, default))).Kind);
        order.State = OrderState.Registered;
        Assert.Equal(FailureKind.Forbidden, (await Assert.ThrowsAsync<BusinessException>(() => service.ChangeAsync(user.Id, order.Id, true, default))).Kind);
        Assert.Equal(OrderState.Registered, order.State);
        Assert.Equal(0, unit.Saves);
        Assert.Empty(await service.ListAsync(user.Id, OrderType.Stock, default));
    }

    [Fact]
    public async Task CustomerCanReadOwnOrder()
    {
        var user = new User { Role = UserRole.Customer };
        var unit = new MemoryUnit();
        var users = new UserService(new MemoryRepository<User>(user), unit);
        var order = new Order { CustomerId = user.Id, Configuration = Snapshot() };
        var service = new OrderService(new MemoryRepository<Order>(order), new MemoryRepository<User>(user), null!, users, unit);
        Assert.Equal(order.Id, (await service.ReadAsync(user.Id, order.Id, default)).OrderId);
        Assert.Single(await service.ListAsync(user.Id, OrderType.Stock, default));
    }

    [Fact]
    public async Task CustomerCannotDeleteAnotherBooking()
    {
        var user = new User { Role = UserRole.Customer };
        var unit = new MemoryUnit();
        var users = new UserService(new MemoryRepository<User>(user), unit);
        var booking = new TestDriveBooking { BookerId = Guid.NewGuid() };
        var service = new TestDriveService(new MemoryRepository<TestDriveBooking>(booking), new MemoryRepository<TestDriveCar>(), null!, users, unit, TimeProvider.System);
        Assert.Equal(FailureKind.Forbidden, (await Assert.ThrowsAsync<BusinessException>(() => service.DeleteAsync(user.Id, booking.Id, default))).Kind);
        Assert.False(booking.IsRemoved);
        Assert.Equal(0, unit.Saves);
        Assert.Empty(await service.ListAsync(user.Id, default));
    }

    private static CarSnapshot Snapshot() => CarSnapshot.From(new Car());

    private sealed class MemoryUnit : IUnitOfWork
    {
        public int Saves
        {
            get; private set;
        }
        public Task SaveAsync(CancellationToken ct)
        {
            Saves++;
            return Task.CompletedTask;
        }
    }

    private sealed class MemoryRepository<T>(params T[] values) : IRepository<T> where T : class
    {
        private readonly List<T> entities = [.. values];
        public Task<List<T>> ListAsync(Func<IQueryable<T>, IQueryable<T>> query, CancellationToken ct, bool tracking = true) => Task.FromResult(query(entities.AsQueryable()).ToList());
        public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct) => Task.FromResult(entities.AsQueryable().Any(predicate));
        public void Add(T entity) => entities.Add(entity);
        public void Remove(T entity) => entities.Remove(entity);
    }
}
