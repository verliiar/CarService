using CarService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarService.Infrastructure.Persistence.Configurations;

internal sealed class TestDriveCarConfiguration : IEntityTypeConfiguration<TestDriveCar>
{
    public void Configure(EntityTypeBuilder<TestDriveCar> builder)
    {
        builder.ToTable("test_drive_cars");
        builder.HasKey(x => x.CarId);
        builder.HasOne<Car>().WithMany().HasForeignKey(x => x.CarId).OnDelete(DeleteBehavior.Restrict);
    }
}
