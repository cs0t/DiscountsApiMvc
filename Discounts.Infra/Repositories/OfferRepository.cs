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

    public override async Task<PagedResult<Offer>> GetPagedAsync(int pageNumber = 1, int pageSize = 8, CancellationToken ct = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize is < 1 or > 20 ? 8 : pageSize;

        var query = _context.Offers.AsNoTracking();
        
        var totalCount = await query.CountAsync(ct);
        
        var items = await query
            .Include(o => o.Status)
            .Include(o => o.Seller)
            .Include(o => o.Categories)
            .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<Offer>(items, totalCount, pageNumber, pageSize);
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

    public async Task<PagedResult<Offer>> GetApprovedActiveOffersAsync(OfferListQuery query,CancellationToken ct = default)
    {
        var pageSize = query.PageSize is < 1 or > 20 ? 10 : query.PageSize;
        var pageNumber = query.PageNumber < 1 ? 1 : query.PageNumber;
        var dateNow = DateTime.UtcNow;

        var buildQuery = _context.Offers
            .AsNoTracking()
            .Where(o => 
                o.StatusId == (int)OfferStatusesEnum.Approved 
                && o.ExpirationDate > dateNow);

        if (query.PriceStart is not null)
        {
            var priceStart = Math.Max(1.0M, query.PriceStart.Value);
            buildQuery = buildQuery.Where(o => o.DiscountedPrice >= priceStart);
        }
        
        if (query.PriceEnd is not null)
        {
            var priceEnd = Math.Max(1.0M, query.PriceEnd.Value);
            buildQuery = buildQuery.Where(o => o.DiscountedPrice <= priceEnd);
        }
        
        if(query.CategoryIds is not null && query.CategoryIds.Count > 0)
        {
            buildQuery = buildQuery.Where(o => o.Categories.Any(c => query.CategoryIds.Contains(c.Id)));
        }
        
        var totalCount = await buildQuery.CountAsync(ct);
        
        var paged = await buildQuery
            .Include(o => o.Categories)
            .Include(o => o.Status)
            .Include(o => o.Seller)
            .OrderByDescending(o => o.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Offer>(paged,totalCount,pageNumber,pageSize);
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