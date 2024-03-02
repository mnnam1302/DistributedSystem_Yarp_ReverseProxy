using DistributedSystem.Domain.Entities.Identity;
using DistributedSystem.Persistence.Constants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DistributedSystem.Persistence.Configurations
{
    internal sealed class AppRoleConfiguration : IEntityTypeConfiguration<AppRole>
    {
        public void Configure(EntityTypeBuilder<AppRole> builder)
        {
            builder.ToTable(TableNames.AppRoles);

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Description).HasMaxLength(250).IsRequired(true);
            builder.Property(x => x.RoleCode).HasMaxLength(50).IsRequired(true);

            // Each AppRole can have many RoleClaims
            builder.HasMany(e => e.Claims)
                .WithOne()
                .HasForeignKey(rc => rc.RoleId)
                .IsRequired();

            // Each AppRole can have many entries in the UserRole join table
            builder.HasMany(e => e.UserRoles)
                .WithOne()
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            // Each AppRole can have many Permissions
            builder.HasMany(e => e.Permissions)
                .WithOne()
                .HasForeignKey(p => p.RoleId)
                .IsRequired();
        }
    }
}