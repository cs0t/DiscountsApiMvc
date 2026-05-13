using Discounts.Application.Interfaces.AuthContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Models;
using Discounts.Application.Queries.Seller;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Handlers.Seller.Dashboard;

public class GetSellerDashboardStatsHandler(
    IOfferRepository offerRepository,
    ICouponRepository couponRepository,
    ICurrentUserService currentUserService)
    : IRequestHandler<GetSellerDashboardStatsQuery, SellerDashboardStats>
{
    public async Task<SellerDashboardStats> Handle(GetSellerDashboardStatsQuery request, CancellationToken ct = default)
    {
        var sellerId = currentUserService.UserId;

        var totalOffers = await offerRepository
            .GetOfferCountBySellerAndStatusAsync(sellerId, null, ct);
        var approvedOffers = await offerRepository
            .GetOfferCountBySellerAndStatusAsync(sellerId, (int)OfferStatusesEnum.Approved, ct);
        var pendingOffers = await offerRepository
            .GetOfferCountBySellerAndStatusAsync(sellerId, (int)OfferStatusesEnum.Pending, ct);
        var expiredOffers = await offerRepository
            .GetOfferCountBySellerAndStatusAsync(sellerId, (int)OfferStatusesEnum.Expired, ct);
        var totalCouponsSold = await couponRepository
            .GetCouponCountForSellerAsync(sellerId, ct);
        var totalIncome = await couponRepository
            .GetTotalIncomeFromCouponsForSellerAsync(sellerId, ct);

        return new SellerDashboardStats
        {
            TotalOffers = totalOffers,
            ApprovedOffers = approvedOffers,
            PendingOffers = pendingOffers,
            ExpiredOffers = expiredOffers,
            TotalCouponsSold = totalCouponsSold,
            TotalIncome = totalIncome
        };
    }
}

