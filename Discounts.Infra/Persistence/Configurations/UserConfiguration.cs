using Discounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discounts.Infra.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Email).IsRequired().HasMaxLength(200);
        builder.Property(x => x.UserName).IsRequired().HasMaxLength(200);
        builder.Property(x => x.PasswordHash).IsRequired().HasMaxLength(550);
        
        //relations
        builder.HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        //indexes
        builder.HasIndex(x=>x.Email).IsUnique();
        builder.HasIndex(x => x.UserName).IsUnique();
    }
}