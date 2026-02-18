using Discounts.Application.Exceptions;
using Discounts.Application.Exceptions.UserExceptions;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Interfaces.SellerModuleServiceContracts;
using Discounts.Application.Models;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;

namespace Discounts.Application.Services.SellerModuleServices;

public class SellerSalesService : ISellerSalesService
{
    private readonly ICouponRepository _couponRepository;
    private readonly IUserRepository _userRepository;
    
    public SellerSalesService(
        ICouponRepository couponRepository,
        IUserRepository userRepository)
    {
        _couponRepository = couponRepository;
        _userRepository = userRepository;
    }
    
    public async Task<List<Coupon>> GetSalesHistoryAsync(int sellerId, CancellationToken ct = default)
    {
        var seller = await _userRepository.GetWithRolesAsync(sellerId, ct);
        if (seller is null)
        {
            throw new UserNotFoundException($"User with id {sellerId} not found !");
        }
        if (seller.RoleId != (int)RoleEnum.Seller)
        {
            throw new ForbiddenException("User does not have permission to view sales history !");
        } 
        return await _couponRepository.GetBySellerIdAsync(sellerId, ct);
    }
}