using System.Linq.Expressions;
using CarService.Application;
using CarService.Domain;
using Microsoft.EntityFrameworkCore;

namespace CarService.Infrastructure;

public sealed class Repository<T>(DealershipDbContext db) : IRepository<T> where T : class
{
    public Task<List<T>> ListAsync(Func<IQueryable<T>, IQueryable<T>> query, CancellationToken ct, bool tracking = true)
    {
        IQueryable<T> source = db.Set<T>();
        if (typeof(T) == typeof(Car))
        {
            source = (IQueryable<T>)db.Set<Car>().Include(c => c.RequiredDetails).AsSplitQuery();
        }

        if (typeof(T) == typeof(Component))
        {
            source = (IQueryable<T>)db.Set<Component>().Include(c => c.SupportedCars).AsSplitQuery();
        }

        if (typeof(T) == typeof(TestDriveBooking))
        {
            source = (IQueryable<T>)db.Set<TestDriveBooking>().Include(b => b.Car).ThenInclude(c => c.RequiredDetails).AsSplitQuery();
        }

        if (!tracking)
        {
            source = source.AsNoTracking();
        }

        return query(source).ToListAsync(ct);
    }

    public Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct) => db.Set<T>().AnyAsync(predicate, ct);

    public void Add(T entity) => db.Set<T>().Add(entity);
    public void Remove(T entity) => db.Set<T>().Remove(entity);
}
