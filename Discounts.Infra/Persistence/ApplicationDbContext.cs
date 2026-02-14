using Discounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infra.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<User>  Users { get; set; } = null!;
    public DbSet<Role>  Roles { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<Coupon> Coupons { get; set; } = null!;
    public DbSet<Offer> Offers { get; set; } = null!;
    public DbSet<OfferStatus> OfferStatuses { get; set; } = null!;
    public DbSet<Reservation> Reservations { get; set; } = null!;
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}