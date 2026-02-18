using Discounts.Application.Commands;
using Discounts.Application.Models;
using Discounts.Application.Queries;
using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.SellerModuleServiceContracts;

public interface IOfferManagementService
{
    Task<Offer> CreateOfferAsync(CreateOfferCommand offer, int sellerId, CancellationToken ct = default);

    Task<Offer> UpdateOfferAsync(UpdateOfferCommand offer, int sellerId, CancellationToken ct = default);

    Task DisableOfferAsync(int offerId, int sellerId, CancellationToken ct = default);

    Task<PagedResult<Offer>> GetMerchantOffersAsync(OfferListQuery query, int sellerId, CancellationToken ct = default);
    
    Task<Offer> GetOfferDetailsAsync(int offerId, int sellerId, CancellationToken ct = default);
}