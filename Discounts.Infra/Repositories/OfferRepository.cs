using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using Discounts.Infra.Persistence;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Models;
using Discounts.Application.Queries;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infra.Repositories;

public class OfferRepository : Repository<Offer>, IOfferRepository
{
    public OfferRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
    
    public async Task<Offer> AddAndReturnAsync(Offer entity, CancellationToken ct = default)
    {
        await base.Add(entity,ct);
        await base.SaveChangesAsync(ct);
        
        return await _context.Offers
            .Include(o => o.Categories)
            .Include(o => o.Status)
            .Include(o => o.Seller)
            .FirstAsync(u => u.Id == entity.Id, ct);
    }
    
    public async Task<Offer> UpdateAndReturnAsync(Offer entity, CancellationToken ct = default)
    {
        base.Update(entity);
        await base.SaveChangesAsync(ct);
        
        return await _context.Offers
            .Include(o => o.Categories)
            .Include(o => o.Status)
            .Include(o => o.Seller)
            .FirstAsync(u => u.Id == entity.Id, ct);
    }
    
    public Task<Offer?> GetWithDetailsByIdAsync(int offerId, CancellationToken ct = default)
    {
        return _context.Offers
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

    public async Task<PagedResult<Offer>> GetBySellerAsync(OfferListQuery query, int sellerId, CancellationToken ct = default)
    {
        var pageSize = query.PageSize is < 1 or > 20 ? 10 : query.PageSize;
        var pageNumber = query.PageNumber < 1 ? 1 : query.PageNumber;

        var sellerOffersQuery = _context.Offers
            .AsNoTracking()
            .Where(o => o.SellerId == sellerId);
        
        var totalCount = await sellerOffersQuery.CountAsync(ct);
        
        var paged =  await sellerOffersQuery
            .Include(o => o.Categories)
            .Include(o => o.Status)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);
        
        return new PagedResult<Offer>(paged, totalCount, pageNumber, pageSize );
    }
    
    public Task<int> GetOfferCountBySellerAndStatusAsync(int sellerId, int? statusId, CancellationToken ct = default)
    {
        return _context.Offers
            .AsNoTracking()
            .Where(o=>o.SellerId == sellerId && (!statusId.HasValue || o.StatusId == statusId.Value))
            .CountAsync(ct);
    }
    
    
}