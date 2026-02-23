using Discounts.Application.Exceptions;
using Discounts.Application.Exceptions.UserExceptions;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Interfaces.SellerModuleServiceContracts;
using Discounts.Application.Services.SellerModuleServices;
using Discounts.Domain.Entities;
using Moq;

namespace Discounts.Tests.SellerModuleTests;

public class SellerSalesServiceTests
{
    private readonly Mock<ICouponRepository> _couponRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly ISellerSalesService _sellerSalesService;
    
    public SellerSalesServiceTests()
    {
        _couponRepositoryMock = new Mock<ICouponRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _sellerSalesService = new SellerSalesService(_couponRepositoryMock.Object, _userRepositoryMock.Object);
    }
    
    [Fact]
    public async Task GetSalesHistoryAsync_ShouldReturnSalesHistory_WhenSeller()
    {
        var sellerId = 1;
        var coupons = new List<Coupon>
        {
            new Coupon { Id = 1, OfferId = 1, CustomerId = 2 },
            new Coupon { Id = 2, OfferId = 2, CustomerId = 3 }
        };
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(sellerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = sellerId, RoleId = (int)Domain.Constants.RoleEnum.Seller });
        
        _couponRepositoryMock.Setup(repo => repo.GetBySellerIdAsync(sellerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupons);
        
        var result = await _sellerSalesService.GetSalesHistoryAsync(sellerId);
        
        Assert.Equal(coupons.Count, result.Count);
        Assert.Equal(coupons[0].Id, result[0].Id);
        Assert.Equal(coupons[1].Id, result[1].Id);
    }

    [Fact]
    public async Task GetSalesHistoryAsync_ShouldThrow_WhenNotSeller()
    {
        var sellerId = 1;
        var coupons = new List<Coupon>
        {
            new Coupon { Id = 1, OfferId = 1, CustomerId = 2 },
            new Coupon { Id = 2, OfferId = 2, CustomerId = 3 }
        };
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(sellerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = sellerId, RoleId = (int)Domain.Constants.RoleEnum.Customer });
        
        _couponRepositoryMock.Setup(repo => repo.GetBySellerIdAsync(sellerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupons);
        
        var act  =  async () => await _sellerSalesService.GetSalesHistoryAsync(sellerId);
        await Assert.ThrowsAsync<ForbiddenException>(act);
    }

    [Fact]
    public async Task GetSalesHistoryAsync_ShouldThrow_WhenSellerNotFound()
    {
        var sellerId = 1;
        var coupons = new List<Coupon>
        {
            new Coupon { Id = 1, OfferId = 1, CustomerId = 2 },
            new Coupon { Id = 2, OfferId = 2, CustomerId = 3 }
        };
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(sellerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);
        
        _couponRepositoryMock.Setup(repo => repo.GetBySellerIdAsync(sellerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(coupons);
        
        var act  =  async () => await _sellerSalesService.GetSalesHistoryAsync(sellerId);
        await Assert.ThrowsAsync<UserNotFoundException>(act);
    }
    
    [Fact]
    public async Task GetSalesHistoryAsync_ShouldReturnEmptyList_WhenNoCoupons()
    {
        var sellerId = 1;
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(sellerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = sellerId, RoleId = (int)Domain.Constants.RoleEnum.Seller });
        
        _couponRepositoryMock.Setup(repo => repo.GetBySellerIdAsync(sellerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Coupon>());
        
        var result = await _sellerSalesService.GetSalesHistoryAsync(sellerId);
        
        Assert.Empty(result);
    }
}