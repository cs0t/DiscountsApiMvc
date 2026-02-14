using Discounts.Domain.Constants;
using Dsicounts.Application.Interfaces.RepositoryContracts;
using Dsicounts.Application.Interfaces.SellerModuleServiceContracts;
using Dsicounts.Application.Models;

namespace Dsicounts.Application.Services.SellerModuleServices;

public class SellerDashboardService : ISellerDashboardService
{
    private readonly IOfferRepository _offerRepository;
    private readonly ICouponRepository _couponRepository;

    public SellerDashboardService(IOfferRepository offerRepository, ICouponRepository couponRepository)
    {
        _offerRepository = offerRepository;
        _couponRepository = couponRepository;
    }
    
    public async Task<SellerDashboardStats> GetDashboardStatsAsync(int sellerId, CancellationToken ct = default)
    {
        var totalOffers = _offerRepository
            .GetOfferCountBySellerAndStatusAsync(sellerId,null,ct);
        var approvedOffers = _offerRepository
            .GetOfferCountBySellerAndStatusAsync(sellerId,(int)OfferStatusesEnum.Approved,ct);
        var pendingOffers = _offerRepository
            .GetOfferCountBySellerAndStatusAsync(sellerId,(int)OfferStatusesEnum.Pending,ct);
        var expiredOffers = _offerRepository
            .GetOfferCountBySellerAndStatusAsync(sellerId, (int)OfferStatusesEnum.Expired, ct);
        var totalCouponsSold = _couponRepository.GetCouponCountForSellerAsync(sellerId, ct);
        
        await Task.WhenAll(totalOffers, approvedOffers, pendingOffers, expiredOffers, totalCouponsSold);
        
        return new SellerDashboardStats
        {
            TotalOffers = totalOffers.Result,
            ApprovedOffers = approvedOffers.Result,
            PendingOffers = pendingOffers.Result,
            ExpiredOffers = expiredOffers.Result,
            TotalCouponsSold = totalCouponsSold.Result
        };
    }
}