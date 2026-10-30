using CarService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarService.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(64).IsRequired();
        builder.Property(x => x.Role).HasConversion<string>().HasMaxLength(64);

        builder.ToTable("users", t => t.HasCheckConstraint("valid_user_role", "\"Role\" IN ('Manager','WarehouseAdmin','Customer','SystemAdmin')"));
    }
}
