using Discounts.Application.Models;
using Discounts.Application.Queries;
using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.RepositoryContracts;

public interface IOfferRepository : IRepository<Offer>
{
    Task<Offer> AddAndReturnAsync(Offer entity, CancellationToken ct = default);
    Task<Offer> UpdateAndReturnAsync(Offer entity, CancellationToken ct = default);
    Task<Offer?> GetWithDetailsByIdAsync(int offerId, CancellationToken ct = default);
    Task<PagedResult<Offer>> GetApprovedActiveOffersAsync(OfferListQuery query, CancellationToken ct = default);
    Task<PagedResult<Offer>> GetBySellerAsync(OfferListQuery query, int sellerId, CancellationToken ct = default);
    Task<int> GetOfferCountBySellerAndStatusAsync(int sellerId, int? statusId, CancellationToken ct = default);
}