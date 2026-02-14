using Discounts.Domain.Entities;

namespace Dsicounts.Application.Interfaces.SellerModuleServiceContracts;

public interface IOfferManagementService
{
    Task<int?> CreateOfferAsync(Offer offer, int sellerId, CancellationToken ct = default);

    Task UpdateOfferAsync(Offer offer, int sellerId, CancellationToken ct = default);

    Task DisableOfferAsync(int offerId, int sellerId, CancellationToken ct = default);

    Task<List<Offer>> GetMerchantOffersAsync(int sellerId, CancellationToken ct = default);
}