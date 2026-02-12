using Discounts.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infra.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<User>  Users { get; set; }
    public DbSet<Role>  Roles { get; set; }
    public DbSet<Category> Categories { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}