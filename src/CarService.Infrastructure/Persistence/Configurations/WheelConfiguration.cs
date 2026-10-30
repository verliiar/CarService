using CarService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarService.Infrastructure.Persistence.Configurations;

internal sealed class WheelConfiguration : IEntityTypeConfiguration<Wheel>
{
    public void Configure(EntityTypeBuilder<Wheel> builder)
    {
        builder.Property(x => x.Diameter).HasPrecision(14, 4);
        builder.Property(x => x.Seasonality).HasConversion<string>().HasMaxLength(64);

        builder.ToTable("wheels", table =>
        {
            table.HasCheckConstraint("wheel_dimensions", "\"Diameter\" > 0 AND \"LoadIndex\" > 0");
            table.HasCheckConstraint("valid_seasonality", "\"Seasonality\" IN ('Summer','Winter','All')");
        });
    }
}
