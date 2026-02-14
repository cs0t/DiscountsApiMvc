using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using Discounts.Infra.Persistence;
using Dsicounts.Application.Interfaces.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infra.Repositories;

public class OfferRepository : Repository<Offer>, IOfferRepository
{
    public OfferRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<Offer?> GetWithDetailsByIdAsync(int offerId, CancellationToken ct = default)
    {
        return _context.Offers
            .AsNoTracking()
            .Include(o => o.Categories)
            .Include(o => o.Status)
            .Include(o => o.Seller)
            .FirstOrDefaultAsync(o => o.Id == offerId, ct);
    }

    public Task<List<Offer>> GetApprovedActiveOffersAsync(CancellationToken ct = default)
    {
        var dateNow = DateTime.UtcNow;
        return _context.Offers
            .AsNoTracking()
            .Where(o=>o.StatusId == (int)OfferStatusesEnum.Approved && o.ExpirationDate > dateNow)
            .Include(o => o.Categories)
            .Include(o => o.Status)
            .Include(o => o.Seller)
            .ToListAsync(ct);
    }

    public Task<List<Offer>> GetBySellerAsync(int sellerId, CancellationToken ct = default)
    {
        return _context.Offers
            .AsNoTracking()
            .Where(o=>o.SellerId == sellerId)
            .Include(o => o.Categories)
            .Include(o => o.Status)
            .ToListAsync(ct);
    }
    
    public Task<int> GetOfferCountBySellerAndStatusAsync(int sellerId, int? statusId, CancellationToken ct = default)
    {
        return _context.Offers
            .AsNoTracking()
            .Where(o=>o.SellerId == sellerId && (!statusId.HasValue || o.StatusId == statusId.Value))
            .CountAsync(ct);
    }
    
    
}