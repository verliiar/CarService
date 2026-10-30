using CarService.Application;
using CarService.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CarService.Infrastructure;

public sealed class DealershipDbContext(DbContextOptions<DealershipDbContext> options) : IdentityUserContext<ApplicationUser, Guid>(options), IUnitOfWork
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(DealershipDbContext).Assembly);
        builder.Seed();
        foreach (var entity in builder.Model.GetEntityTypes().Where(x => typeof(Entity).IsAssignableFrom(x.ClrType) && x.BaseType == null))
        {
            builder.Entity(entity.ClrType).Property(nameof(Entity.CreatedAt)).IsRequired();
            builder.Entity(entity.ClrType).Property(nameof(Entity.UpdatedAt)).IsRequired();
        }
    }

    public async Task SaveAsync(CancellationToken ct) => await SaveChangesAsync(ct);

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTimeOffset.UtcNow;
        ChangeTracker.DetectChanges();
        foreach (var entry in ChangeTracker.Entries<Entity>().Where(x => x.State is EntityState.Added or EntityState.Modified || x.Collections.Any(c => c.IsModified)).ToList())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = now;
            }

            entry.Entity.UpdatedAt = now;
            if (entry.State != EntityState.Added)
            {
                entry.Property(x => x.UpdatedAt).IsModified = true;
            }
        }
        try
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new BusinessException(FailureKind.Conflict, "The record changed concurrently. Reload and retry.");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            throw new BusinessException(FailureKind.Conflict, "A record with the same unique values already exists.");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.ForeignKeyViolation })
        {
            throw new BusinessException(FailureKind.Conflict, "A related record changed. Reload and retry.");
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.CheckViolation or PostgresErrorCodes.NumericValueOutOfRange or PostgresErrorCodes.StringDataRightTruncation })
        {
            throw new BusinessException(FailureKind.Validation, "The supplied values violate data constraints.");
        }
    }
}
