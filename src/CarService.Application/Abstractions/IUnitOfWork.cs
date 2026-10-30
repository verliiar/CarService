namespace CarService.Application;

public interface IUnitOfWork
{
    Task SaveAsync(CancellationToken ct);
}
