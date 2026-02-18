using Discounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Discounts.Infra.Persistence.Configurations;

public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.ToTable("Reservations");
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.ReservedAt)
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(r => r.CancelledAt)
            .HasColumnType("datetime2");
        
        //relations
        builder.HasOne(r => r.Offer)
            .WithMany()
            .HasForeignKey(r => r.OfferId)
            .OnDelete(DeleteBehavior.NoAction);
        
        builder.HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.NoAction);
        
        //indexes
        builder.HasIndex(r => new { r.OfferId, r.UserId }).IsUnique().HasFilter("[CancelledAt] IS NULL");
        builder.HasIndex(r => r.ReservedAt);
        //builder.HasIndex(r => r.CancelledAt);
    }
}