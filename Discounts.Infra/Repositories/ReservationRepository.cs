using Discounts.Domain.Entities;
using Discounts.Infra.Persistence;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infra.Repositories;

public class ReservationRepository : Repository<Reservation>, IReservationRepository
{
    public ReservationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<List<Reservation>> GetActiveReservationsAsync(CancellationToken ct = default)
    {
        return _context.Reservations.Where(r => r.IsActive).ToListAsync(ct);
    }

    public async Task<PagedResult<Reservation>> GetActiveReservationsByUserIdAsync(int userId, int page=1, int pageSize=8, CancellationToken ct = default)
    {
        //return _context.Reservations.Where(r => r.UserId == userId).ToListAsync(ct);
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 12);
        var query = _context.Reservations
            .AsNoTracking()
            .Where(r => r.UserId == userId && r.IsActive);
        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Include(r => r.Offer)
            .ThenInclude(o => o.Seller)
            .Include(r => r.Offer)
            .ThenInclude(o => o.Categories)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<Reservation>(items, totalCount, page, pageSize);
    }

    public Task<List<Reservation>> GetActiveReservationsByOfferIdAsync(int offerId, CancellationToken ct = default)
    {
        return _context.Reservations.Where(r => r.OfferId == offerId && r.IsActive).ToListAsync(ct);
    }

    public Task<Reservation?> GetActiveReservationByUserIdAndOfferIdAsync(int userId, int offerId, CancellationToken ct = default)
    {
        return _context.Reservations.FirstOrDefaultAsync(r => r.UserId == userId && r.OfferId == offerId && r.IsActive, ct);
    }
    
}