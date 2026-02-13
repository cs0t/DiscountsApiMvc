using Discounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discounts.Infra.Persistence.Configurations;

public class OfferStatusConfiguration : IEntityTypeConfiguration<OfferStatus>
{
    public void Configure(EntityTypeBuilder<OfferStatus> builder)
    {
        builder.ToTable("OfferStatuses");
        builder.HasKey(os => os.Id);
        
        builder.Property(os => os.Name)
            .HasMaxLength(50)
            .IsRequired();
        
        //relations
        // builder.HasMany(os => os.Offers)
        //     .WithOne(o => o.Status)
        //     .HasForeignKey(o => o.StatusId)
        //     .OnDelete(DeleteBehavior.Restrict);
        
        //indexes
        builder.HasIndex(os => os.Name).IsUnique();
    }
}