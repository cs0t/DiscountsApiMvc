using Discounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discounts.Infra.Persistence.Configurations;

public class CouponStatusConfiguration : IEntityTypeConfiguration<CouponStatus>
{

    public void Configure(EntityTypeBuilder<CouponStatus> builder)
    {
        builder.ToTable("CouponStatuses");
        builder.HasKey(cs => cs.Id);
        
        builder.Property(cs => cs.Name)
            .HasMaxLength(50)
            .IsRequired();
        
        
        
        //indexes
        builder.HasIndex(cs => cs.Name).IsUnique();
    }
}