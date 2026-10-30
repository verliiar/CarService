using CarService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarService.Infrastructure.Persistence.Configurations;

internal sealed class TransmissionConfiguration : IEntityTypeConfiguration<Transmission>
{
    public void Configure(EntityTypeBuilder<Transmission> builder)
    {
        builder.Property(x => x.Gearbox).HasConversion<string>().HasMaxLength(64);
        builder.Property(x => x.WheelDrive).HasConversion<string>().HasMaxLength(64);
        builder.Property(x => x.EnergyTransfer).HasConversion<string>().HasMaxLength(64);

        builder.ToTable("transmissions", t =>
        {
            t.HasCheckConstraint("valid_gearbox", "\"Gearbox\" IN ('Manual','Automatic')");
            t.HasCheckConstraint("valid_wheel_drive", "\"WheelDrive\" IN ('Front','Rear','All')");
            t.HasCheckConstraint("valid_energy_transfer", "\"EnergyTransfer\" IN ('Mechanical','Hydromechanical','Hydrostatic')");
        });
    }
}
