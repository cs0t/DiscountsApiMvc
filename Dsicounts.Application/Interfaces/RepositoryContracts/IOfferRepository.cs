using Discounts.Domain.Entities;

namespace Dsicounts.Application.Interfaces.RepositoryContracts;

public interface IOfferRepository : IRepository<Offer>
{
    Task<Offer?> GetWithDetailsByIdAsync(int offerId, CancellationToken ct = default);
    Task<List<Offer>> GetApprovedActiveOffersAsync(CancellationToken ct = default);
    Task<List<Offer>> GetBySellerAsync(int sellerId, CancellationToken ct = default);
    Task<int> GetOfferCountBySellerAndStatusAsync(int sellerId, int? statusId, CancellationToken ct = default);
}