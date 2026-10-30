using CarService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarService.Infrastructure.Persistence.Configurations;

internal sealed class CarConfiguration : IEntityTypeConfiguration<Car>
{
    public void Configure(EntityTypeBuilder<Car> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.Brand, x.Model }).IsUnique();
        builder.HasIndex(x => x.IsAvailable);
        builder.Property(x => x.Brand).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Model).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Version).IsRowVersion();
        builder.Property(x => x.Color).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Price).HasPrecision(18, 2);
        builder.Property(x => x.Body).HasConversion<string>().HasMaxLength(64);
        builder.Property(x => x.Fuel).HasConversion<string>().HasMaxLength(64);
        builder.Property(x => x.EnginePower).HasPrecision(14, 4);
        builder.Property(x => x.EngineDisplacement).HasPrecision(14, 4);
        builder.HasMany(x => x.RequiredDetails).WithMany().UsingEntity<Dictionary<string, object>>("car_components",
            r => r.HasOne<Component>().WithMany().HasForeignKey("ComponentId").OnDelete(DeleteBehavior.Restrict),
            l => l.HasOne<Car>().WithMany().HasForeignKey("CarId").OnDelete(DeleteBehavior.Cascade));

        builder.ToTable("cars", t =>
        {
            t.HasCheckConstraint("car_price", "\"Price\" >= 0");
            t.HasCheckConstraint("car_engine", "\"EnginePower\" >= 0 AND \"EngineDisplacement\" >= 0");
            t.HasCheckConstraint("valid_body", "\"Body\" IN ('Sedan','Hatchback','Wagon','Coupe','Convertible','Liftback','Fastback','Limousine','Minivan','SportUtilityVehicle','CrossoverUtilityVehicle')");
            t.HasCheckConstraint("valid_fuel", "\"Fuel\" IN ('Petrol','Diesel','Electric')");
        });
    }
}
