using Discounts.Application.Exceptions;
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
        
        if (seller is null)
        {
            throw new UserNotFoundException($"User with id {sellerId} not found !");
        }
        if (seller.RoleId != (int)RoleEnum.Seller)
        {
            throw new ForbiddenException("User does not have permission to view this dashboard !");
        } 
        
        var totalOffers = await _offerRepository
            .GetOfferCountBySellerAndStatusAsync(sellerId,null,ct);
        var approvedOffers = await _offerRepository
            .GetOfferCountBySellerAndStatusAsync(sellerId,(int)OfferStatusesEnum.Approved,ct);
        var pendingOffers = await _offerRepository
            .GetOfferCountBySellerAndStatusAsync(sellerId,(int)OfferStatusesEnum.Pending,ct);
        var expiredOffers = await _offerRepository
            .GetOfferCountBySellerAndStatusAsync(sellerId, (int)OfferStatusesEnum.Expired, ct);
        var totalCouponsSold = await _couponRepository.GetCouponCountForSellerAsync(sellerId, ct);
        
        var totalIncome =await  _couponRepository.GetTotalIncomeFromCouponsForSellerAsync(sellerId, ct);
        
        // await Task.WhenAll(totalOffers, approvedOffers, 
        //     pendingOffers, expiredOffers, 
        //     totalCouponsSold,totalIncome);
        
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