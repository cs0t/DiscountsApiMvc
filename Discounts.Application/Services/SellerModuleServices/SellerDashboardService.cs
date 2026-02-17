using Discounts.Application.Exceptions.UserExceptions;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Interfaces.SellerModuleServiceContracts;
using Discounts.Application.Models;
using Discounts.Domain.Constants;

namespace Discounts.Application.Services.SellerModuleServices;

public class SellerDashboardService : ISellerDashboardService
{
    private readonly IOfferRepository _offerRepository;
    private readonly ICouponRepository _couponRepository;
    private readonly IUserRepository _userRepository;

    public SellerDashboardService(
        IOfferRepository offerRepository, 
        ICouponRepository couponRepository,
        IUserRepository userRepository)
    {
        _offerRepository = offerRepository;
        _couponRepository = couponRepository;
        _userRepository = userRepository;
    }
    
    public async Task<SellerDashboardStats> GetDashboardStatsAsync(int sellerId, CancellationToken ct = default)
    {
        //fetch seller
        var seller = await _userRepository.GetWithRolesAsync(sellerId, ct);
        
        if (seller is null || seller.Role.Id != (int)RoleEnum.Seller)
        {
            throw new UnauthorizedException("User is not authorized to view this dashboard !");
        }
        
        var totalOffers = _offerRepository
            .GetOfferCountBySellerAndStatusAsync(sellerId,null,ct);
        var approvedOffers = _offerRepository
            .GetOfferCountBySellerAndStatusAsync(sellerId,(int)OfferStatusesEnum.Approved,ct);
        var pendingOffers = _offerRepository
            .GetOfferCountBySellerAndStatusAsync(sellerId,(int)OfferStatusesEnum.Pending,ct);
        var expiredOffers = _offerRepository
            .GetOfferCountBySellerAndStatusAsync(sellerId, (int)OfferStatusesEnum.Expired, ct);
        var totalCouponsSold = _couponRepository.GetCouponCountForSellerAsync(sellerId, ct);
        
        var totalIncome = _couponRepository.GetTotalIncomeFromCouponsForSellerAsync(sellerId, ct);
        
        await Task.WhenAll(totalOffers, approvedOffers, 
            pendingOffers, expiredOffers, 
            totalCouponsSold,totalIncome);
        
        return new SellerDashboardStats
        {
            TotalOffers = totalOffers.Result,
            ApprovedOffers = approvedOffers.Result,
            PendingOffers = pendingOffers.Result,
            ExpiredOffers = expiredOffers.Result,
            TotalCouponsSold = totalCouponsSold.Result,
            TotalIncome = totalIncome.Result
        };
    }
}