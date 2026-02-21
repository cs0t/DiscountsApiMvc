using Discounts.Domain.Constants;

namespace Discounts.Infra.Persistence.Seeding;

public static class SeedDataGenerator
{
    public static async Task SeedAsync(ApplicationDbContext context, CancellationToken ct = default)
    {
        await SeedRolesAsync(context, ct);
        await SeedUsersAsync(context, ct);
        await SeedCategoriesAsync(context, ct);
        await SeedOfferStatusesAsync(context, ct);
        await SeedCouponStatusesAsync(context, ct);
        await SeedSystemSettingsAsync(context, ct);
    }

    private static async Task SeedRolesAsync(ApplicationDbContext context, CancellationToken ct)
    {
        if (context.Roles.Any())
            return;
        context.Roles.AddRange(
            new Domain.Entities.Role {  Name = "Admin" },
            new Domain.Entities.Role {  Name = "Seller" },
            new Domain.Entities.Role {  Name = "Customer" },
            new Domain.Entities.Role {  Name = "Blocked" }
        );
        
        await context.SaveChangesAsync(ct);
    }

    private static async Task SeedUsersAsync(ApplicationDbContext context, CancellationToken ct)
    {
        if (context.Users.Any())
            return;
        
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("123123");
        context.Users.AddRange(
            new Domain.Entities.User
            {
                UserName = "admin",
                Email = "admin@gmail.com",
                PasswordHash = passwordHash,
                RoleId = (int)Domain.Constants.RoleEnum.Administrator
            }, new Domain.Entities.User
            {
                UserName = "seller",
                Email = "seller@gmail.com",
                PasswordHash = passwordHash,
                RoleId = (int)Domain.Constants.RoleEnum.Seller
            },
            new Domain.Entities.User
            {
                UserName = "client",
                Email = "client@gmail.com",
                PasswordHash = passwordHash,
                RoleId = (int)Domain.Constants.RoleEnum.Customer
            });
        
        await context.SaveChangesAsync(ct);
    }

    private static async Task SeedCategoriesAsync(ApplicationDbContext context, CancellationToken ct)
    {
        if (context.Categories.Any())
            return;
        
        context.Categories.AddRange(
            new Domain.Entities.Category { Name = "Food" },
            new Domain.Entities.Category { Name = "Electronics" },
            new Domain.Entities.Category { Name = "Clothing" },
            new Domain.Entities.Category { Name = "Health & Beauty" },
            new Domain.Entities.Category { Name = "Home & Garden" },
            new Domain.Entities.Category { Name = "Tourism" },
            new Domain.Entities.Category { Name = "Fitness" }
        );
        
        await context.SaveChangesAsync(ct);
    }

    private static async Task SeedOfferStatusesAsync(ApplicationDbContext context, CancellationToken ct)
    {
        if (context.OfferStatuses.Any())
            return;
        
        context.OfferStatuses.AddRange(
            new Domain.Entities.OfferStatus { Name = "Pending" },
            new Domain.Entities.OfferStatus { Name = "Approved" },
            new Domain.Entities.OfferStatus { Name = "Rejected" },
            new Domain.Entities.OfferStatus { Name = "Expired" },
            new Domain.Entities.OfferStatus { Name = "Disabled" }
        );
        
        await context.SaveChangesAsync(ct);
    }

    private static async Task SeedCouponStatusesAsync(ApplicationDbContext context, CancellationToken ct)
    {
        if (context.CouponStatuses.Any())
            return;
        
        context.CouponStatuses.AddRange(
            new Domain.Entities.CouponStatus { Name = "Active" },
            new Domain.Entities.CouponStatus { Name = "Used" },
            new Domain.Entities.CouponStatus { Name = "Expired" }
        );
        
        await context.SaveChangesAsync(ct);
    }

    private static async Task SeedSystemSettingsAsync(ApplicationDbContext context, CancellationToken ct)
    {
        if (context.SystemSettings.Any())
            return;
        
        context.SystemSettings.AddRange(
            new Domain.Entities.SystemSettings { Key = SystemSettingNames.OfferEditingTimeLimitInHours, SettingValue = "24" },
            new Domain.Entities.SystemSettings { Key = SystemSettingNames.ReservationTimeLimitInHours, SettingValue = "1" },
            new Domain.Entities.SystemSettings { Key = SystemSettingNames.CleanupServiceCycle, SettingValue = "0.25" }
        );
        
        await context.SaveChangesAsync(ct);
    }
}