using Discounts.Application.Models;
using Discounts.Application.Queries;
using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.CustomerModuleContracts;

public interface ICustomerService
{
    Task PurchaseCouponAsync(int customerId, int offerId, CancellationToken ct = default);
    Task CreateReservationAsync(int customerId, int offerId, byte[] rowVersion,CancellationToken ct = default);
    Task<PagedResult<Reservation>> GetCustomerReservationsAsync(int customerId,
        int page = 1, int pageSize = 8, CancellationToken ct = default);
    Task<PagedResult<Coupon>> GetCustomerCouponsAsync(int customerId, 
        int page=1, int pageSize=8,CancellationToken ct = default);

    Task CancelReservationAsync(int reservationId, int customerId, CancellationToken ct = default);

    Task<PagedResult<Offer>> GetApprovedOffersAsync(OfferListQuery query, int customerId,
        CancellationToken ct = default);

    Task<CustomerOfferState> GetOfferStateForCustomerAsync(int offerId, int customerId,
        CancellationToken ct = default);
}