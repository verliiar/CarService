using CarService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarService.Infrastructure.Persistence.Configurations;

internal sealed class ComponentConfiguration : IEntityTypeConfiguration<Component>
{
    public void Configure(EntityTypeBuilder<Component> builder)
    {
        builder.Property(x => x.Version).IsRowVersion();
        builder.UseTptMappingStrategy();
        builder.ToTable("components", t => t.HasCheckConstraint("component_price", "\"AdditionalCost\" >= 0"));
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => new { x.Category, x.Name }).IsUnique();
        builder.Property(x => x.Name).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Category).HasConversion<string>().HasMaxLength(64);
        builder.Property(x => x.AdditionalCost).HasPrecision(18, 2);
        builder.HasOne<ComponentCategoryEntry>().WithMany().HasForeignKey(x => x.Category).OnDelete(DeleteBehavior.Restrict);
        builder.HasMany(x => x.SupportedCars).WithMany().UsingEntity<Dictionary<string, object>>("compatibilities",
            r => r.HasOne<Car>().WithMany().HasForeignKey("CarId").OnDelete(DeleteBehavior.Cascade),
            l => l.HasOne<Component>().WithMany().HasForeignKey("ComponentId").OnDelete(DeleteBehavior.Cascade));
    }
}
