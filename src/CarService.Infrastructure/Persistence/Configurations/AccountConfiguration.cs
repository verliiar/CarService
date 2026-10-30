using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CarService.Infrastructure.Persistence.Configurations;

internal sealed class AccountConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("accounts");
        builder.HasOne(x => x.Profile).WithOne().HasForeignKey<ApplicationUser>(x => x.Id).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(x => x.NormalizedEmail).IsUnique();
    }
}
