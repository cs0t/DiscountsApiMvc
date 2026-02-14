using Discounts.Domain.Entities;
using Discounts.Infra.Persistence;
using Dsicounts.Application.Interfaces;
using Dsicounts.Application.Interfaces.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infra.Repositories;

public class CouponRepository : Repository<Coupon>,ICouponRepository
{
    public CouponRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<Coupon?> GetByCodeAsync(string code, CancellationToken ct = default)
    {
        return _context.Coupons.Where(c => c.Code == code).FirstOrDefaultAsync(ct);
    }

    public Task<List<Coupon>> GetByCustomerIdAsync(int customerId, CancellationToken ct = default)
    {
        return _context.Coupons
            .AsNoTracking()
            .Include(c => c.Customer)
            .Include(c => c.Offer)
            .Where(c => c.CustomerId == customerId).ToListAsync(ct);
    }

    public Task<List<Coupon>> GetByOfferIdAsync(int offerId, CancellationToken ct = default)
    {
        return _context.Coupons
            .AsNoTracking()
            .Include(c => c.Customer)
            .Include(c => c.Offer)
            .Where(c => c.OfferId == offerId).ToListAsync(ct);
    }
    
    public Task<int> GetCouponCountForSellerAsync(int sellerId, CancellationToken ct = default)
    {
        return _context.Coupons
            .AsNoTracking()
            .Where(c => c.Offer.SellerId == sellerId)
            .CountAsync(ct);
    }
}