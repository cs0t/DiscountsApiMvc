using Discounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discounts.Infra.Persistence.Configurations;

public class OfferConfiguration : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.ToTable("Offers");
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.Title).IsRequired().HasMaxLength(550);
        builder.Property(o => o.Description).HasMaxLength(2000);
        builder.Property(o => o.OriginalPrice).IsRequired().HasPrecision(18,2);
        builder.Property(o => o.DiscountedPrice).IsRequired().HasPrecision(18,2);
        builder.Property(o => o.MaxQuantity).IsRequired();
        builder.Property(o =>o.RemainingQuantity).IsRequired();
        builder.Property(o => o.CreatedAt).IsRequired().HasColumnType("datetime2").HasDefaultValueSql("getutcdate()");
        builder.Property(o => o.ExpirationDate).IsRequired().HasColumnType("datetime2");
        
        //relations
        builder.HasOne(o => o.Seller)
            .WithMany()
            .HasForeignKey(o => o.SellerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(o => o.Status)
            .WithMany(s=> s.Offers)
            .HasForeignKey(o => o.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(o => o.Categories)
            .WithMany(c => c.Offers)
            .UsingEntity(j => j.ToTable("OfferCategories"));
        
        //indexes
         builder.HasIndex(o => o.Title);
         builder.HasIndex(o => o.ExpirationDate);
    }
}