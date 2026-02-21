using Discounts.Application.Models;
using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.RepositoryContracts;

public interface IReservationRepository : IRepository<Reservation>
{
    public Task<List<Reservation>> GetActiveReservationsAsync(CancellationToken ct = default);

    Task<PagedResult<Reservation>> GetActiveReservationsByUserIdAsync(int userId, int page = 1, int pageSize = 8,
        CancellationToken ct = default);
    public Task<List<Reservation>> GetActiveReservationsByOfferIdAsync(int offerId, CancellationToken ct = default);
    public Task<Reservation?> GetActiveReservationByUserIdAndOfferIdAsync(int userId, int offerId, CancellationToken ct = default);
    //public Task CancelReservationAsync(Reservation reservation, CancellationToken ct = default);
}