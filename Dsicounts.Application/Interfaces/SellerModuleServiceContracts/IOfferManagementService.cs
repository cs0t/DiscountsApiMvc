using Discounts.Domain.Entities;
using Dsicounts.Application.Models;
using Dsicounts.Application.Queries;

namespace Dsicounts.Application.Interfaces.SellerModuleServiceContracts;

public interface IOfferManagementService
{
    Task<int?> CreateOfferAsync(Offer offer, int sellerId, CancellationToken ct = default);

    Task UpdateOfferAsync(Offer offer, int sellerId, CancellationToken ct = default);

    Task DisableOfferAsync(int offerId, int sellerId, CancellationToken ct = default);

    Task<PagedResult<Offer>> GetMerchantOffersAsync(OfferListQuery query, int sellerId, CancellationToken ct = default);
}