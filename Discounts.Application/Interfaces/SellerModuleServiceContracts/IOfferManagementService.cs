using Discounts.Application.Commands;
using Discounts.Application.Models;
using Discounts.Application.Queries;
using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.SellerModuleServiceContracts;

public interface IOfferManagementService
{
    Task<int?> CreateOfferAsync(CreateOfferCommand offer, int sellerId, CancellationToken ct = default);

    Task UpdateOfferAsync(UpdateOfferCommand offer, int sellerId, CancellationToken ct = default);

    Task DisableOfferAsync(int offerId, int sellerId, CancellationToken ct = default);

    Task<PagedResult<Offer>> GetMerchantOffersAsync(OfferListQuery query, int sellerId, CancellationToken ct = default);
}