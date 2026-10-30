using System.Linq.Expressions;

namespace CarService.Application;

public interface IRepository<T> where T : class
{
    Task<List<T>> ListAsync(Func<IQueryable<T>, IQueryable<T>> query, CancellationToken ct, bool tracking = true);
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken ct);
    void Add(T entity);
    void Remove(T entity);
}
