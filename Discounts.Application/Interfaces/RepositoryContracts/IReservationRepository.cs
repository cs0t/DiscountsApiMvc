using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.RepositoryContracts;

public interface IReservationRepository : IRepository<Reservation>
{
    public Task<List<Reservation>> GetActiveReservationsAsync(CancellationToken ct = default);
    public Task<List<Reservation>> GetReservationsByUserIdAsync(int userId, CancellationToken ct = default);
    public Task<List<Reservation>> GetActiveReservationsByOfferIdAsync(int offerId, CancellationToken ct = default);
}