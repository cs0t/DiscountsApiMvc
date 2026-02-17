using Discounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discounts.Infra.Persistence.Configurations;

public class SystemSettingsConfiguration : IEntityTypeConfiguration<SystemSettings>
{
    public void Configure(EntityTypeBuilder<SystemSettings> builder)
    {
        builder.ToTable("SystemSettings");
        builder.HasKey(ss => ss.Id);
        
        builder.Property(ss => ss.Key)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(ss => ss.SettingValue).IsRequired().HasMaxLength(500);
        
        //indexes
        builder.HasIndex(ss => ss.Key).IsUnique();
    }
}