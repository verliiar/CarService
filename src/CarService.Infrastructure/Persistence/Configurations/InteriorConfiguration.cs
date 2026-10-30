using CarService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarService.Infrastructure.Persistence.Configurations;

internal sealed class InteriorConfiguration : IEntityTypeConfiguration<Interior>
{
    public void Configure(EntityTypeBuilder<Interior> builder)
    {
        builder.ToTable("interiors");
        builder.Property(x => x.Material).HasMaxLength(64).IsRequired();
    }
}
