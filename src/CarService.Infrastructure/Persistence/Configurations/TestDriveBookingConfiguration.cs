using CarService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarService.Infrastructure.Persistence.Configurations;

internal sealed class TestDriveBookingConfiguration : IEntityTypeConfiguration<TestDriveBooking>
{
    public void Configure(EntityTypeBuilder<TestDriveBooking> builder)
    {
        builder.ToTable("test_drive_bookings");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Version).IsRowVersion();
        builder.HasOne(x => x.Car).WithMany().HasForeignKey(x => x.CarId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.BookerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => new { x.CarId, x.BookingDateTime }).IsUnique().HasFilter("NOT \"IsRemoved\"");
    }
}
