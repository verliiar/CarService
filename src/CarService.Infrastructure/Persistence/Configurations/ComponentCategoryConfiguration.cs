using CarService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarService.Infrastructure.Persistence.Configurations;

internal sealed class ComponentCategoryConfiguration : IEntityTypeConfiguration<ComponentCategoryEntry>
{
    public void Configure(EntityTypeBuilder<ComponentCategoryEntry> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion<string>().HasMaxLength(64);
        builder.Property(x => x.Name).HasMaxLength(64).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();

        builder.ToTable("components_categories", t => t.HasCheckConstraint("valid_category", "\"Id\" IN ('Interior','SteeringWheel','Transmission','Wheel')"));
    }
}
