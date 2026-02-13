using Discounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discounts.Infra.Persistence.Configurations;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("Coupons");
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Code)
            .HasMaxLength(11)
            .IsRequired();
        
        builder.Property(c => c.ExpirationDate)
            .HasColumnType("datetime2")
            .IsRequired();
        
        //relations
        builder.HasOne(c => c.Offer)
            .WithMany()
            .HasForeignKey(c => c.OfferId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(c => c.Customer)
            .WithMany()
            .HasForeignKey(c => c.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
        
        //indexes
        builder.HasIndex(c => c.Code).IsUnique();
    }
}