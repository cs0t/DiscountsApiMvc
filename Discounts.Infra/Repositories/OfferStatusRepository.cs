using Discounts.Domain.Entities;
using Discounts.Infra.Persistence;
using Dsicounts.Application.Interfaces.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infra.Repositories;

public class OfferStatusRepository : Repository<OfferStatus>, IOfferStatusRepository
{
    public OfferStatusRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<Offer>> GetOffersByStatusAsync(int statusId, CancellationToken ct = default)
    {
        return _context.Offers
            .AsNoTracking()
            .Where(o => o.StatusId == statusId)
            .ToListAsync(ct);
    }
}