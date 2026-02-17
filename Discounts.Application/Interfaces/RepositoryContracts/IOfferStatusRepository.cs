using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.RepositoryContracts;

public interface IOfferStatusRepository : IRepository<OfferStatus>
{
    Task<List<Offer>> GetOffersByStatusAsync(int statusId, CancellationToken ct = default);
}