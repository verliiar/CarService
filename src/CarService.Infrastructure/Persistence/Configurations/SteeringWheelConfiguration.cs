using CarService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarService.Infrastructure.Persistence.Configurations;

internal sealed class SteeringWheelConfiguration : IEntityTypeConfiguration<SteeringWheel>
{
    public void Configure(EntityTypeBuilder<SteeringWheel> builder)
    {
        builder.ToTable("steering_wheels", t => t.HasCheckConstraint("steering_size", "\"Size\" > 0"));
        builder.Property(x => x.Material).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Shape).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Size).HasPrecision(14, 4);
    }
}
