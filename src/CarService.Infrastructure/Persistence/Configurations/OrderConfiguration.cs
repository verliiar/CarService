using System.Text.Json;
using CarService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarService.Infrastructure.Persistence.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.State).HasConversion<string>().HasMaxLength(64);
        builder.Property(x => x.Type).HasConversion<string>().HasMaxLength(64);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<User>().WithMany().HasForeignKey(x => x.ManagerId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<Car>().WithMany().HasForeignKey(x => x.CarId).OnDelete(DeleteBehavior.Restrict);
        builder.Property(x => x.Version).IsRowVersion();
        builder.Property(x => x.Configuration).HasColumnType("jsonb").HasConversion(
            v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            v => JsonSerializer.Deserialize<CarSnapshot>(v, (JsonSerializerOptions?)null)!)
            .Metadata.SetValueComparer(new ValueComparer<CarSnapshot>(
                (a, c) => JsonSerializer.Serialize(a, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(c, (JsonSerializerOptions?)null),
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null).GetHashCode(),
                v => JsonSerializer.Deserialize<CarSnapshot>(JsonSerializer.Serialize(v, (JsonSerializerOptions?)null), (JsonSerializerOptions?)null)!));

        builder.ToTable("orders", t =>
        {
            t.HasCheckConstraint("valid_order_state", "\"State\" IN ('Registered','Agreed','AwaitingPayment','Paid','AwaitingDelivery','ReadyForPickUp','Completed','Canceled')");
            t.HasCheckConstraint("valid_order_type", "\"Type\" IN ('Stock','Custom')");
        });

        builder.HasIndex(x => new { x.Type, x.IsRemoved });
    }
}
