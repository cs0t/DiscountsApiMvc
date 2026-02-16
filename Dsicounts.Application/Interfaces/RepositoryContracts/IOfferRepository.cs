using Discounts.Domain.Entities;
using Dsicounts.Application.Models;
using Dsicounts.Application.Queries;

namespace Dsicounts.Application.Interfaces.RepositoryContracts;

public interface IOfferRepository : IRepository<Offer>
{
    Task<Offer?> GetWithDetailsByIdAsync(int offerId, CancellationToken ct = default);
    Task<List<Offer>> GetApprovedActiveOffersAsync(CancellationToken ct = default);
    Task<PagedResult<Offer>> GetBySellerAsync(OfferListQuery query, int sellerId, CancellationToken ct = default);
    Task<int> GetOfferCountBySellerAndStatusAsync(int sellerId, int? statusId, CancellationToken ct = default);
}