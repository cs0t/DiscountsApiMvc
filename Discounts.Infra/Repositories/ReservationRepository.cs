using Discounts.Domain.Entities;
using Discounts.Infra.Persistence;
using Discounts.Application.Interfaces.RepositoryContracts;
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

    public Task<List<Reservation>> GetReservationsByUserIdAsync(int userId, CancellationToken ct = default)
    {
        return _context.Reservations.Where(r => r.UserId == userId).ToListAsync(ct);
    }

    public Task<List<Reservation>> GetActiveReservationsByOfferIdAsync(int offerId, CancellationToken ct = default)
    {
        return _context.Reservations.Where(r => r.OfferId == offerId && r.IsActive).ToListAsync(ct);
    }
}