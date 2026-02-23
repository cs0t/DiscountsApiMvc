using Discounts.Application.Exceptions;
using Discounts.Application.Exceptions.UserExceptions;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Services.SellerModuleServices;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Discounts.Tests.SellerModuleTests;

public class SellerDashboardServiceTests
{
    private readonly SellerDashboardService _dashboardService;
    private readonly Mock<IOfferRepository> _offerRepositoryMock;
    private readonly Mock<ICouponRepository> _couponRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    
    public SellerDashboardServiceTests()
    {
        _offerRepositoryMock = new Mock<IOfferRepository>();
        _couponRepositoryMock = new Mock<ICouponRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _dashboardService = new SellerDashboardService(_offerRepositoryMock.Object,
            _couponRepositoryMock.Object, _userRepositoryMock.Object);
    }
    
    [Fact]
    public async Task GetDashboardStatsAsync_ShouldReturnCorrectStats_WhenValidSeller()
    {
        var sellerId = 1;

        var seller = new User
        {
            Id = sellerId,
            RoleId = (int)RoleEnum.Seller
        };

        _userRepositoryMock
            .Setup(x => x.GetWithRolesAsync(sellerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(seller);

        _offerRepositoryMock
            .Setup(x => x.GetOfferCountBySellerAndStatusAsync(sellerId, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(10);

        _offerRepositoryMock
            .Setup(x => x.GetOfferCountBySellerAndStatusAsync(sellerId, (int)OfferStatusesEnum.Approved, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        _offerRepositoryMock
            .Setup(x => x.GetOfferCountBySellerAndStatusAsync(sellerId, (int)OfferStatusesEnum.Pending, It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        _offerRepositoryMock
            .Setup(x => x.GetOfferCountBySellerAndStatusAsync(sellerId, (int)OfferStatusesEnum.Expired, It.IsAny<CancellationToken>()))
            .ReturnsAsync(2);

        _couponRepositoryMock
            .Setup(x => x.GetCouponCountForSellerAsync(sellerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(50);

        _couponRepositoryMock
            .Setup(x => x.GetTotalIncomeFromCouponsForSellerAsync(sellerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(1250m);

        var result = await _dashboardService.GetDashboardStatsAsync(sellerId);

        Assert.Equal(10, result.TotalOffers);
        Assert.Equal(5, result.ApprovedOffers);
        Assert.Equal(3, result.PendingOffers);
        Assert.Equal(2, result.ExpiredOffers);
        Assert.Equal(50, result.TotalCouponsSold);
        Assert.Equal(1250m, result.TotalIncome);
    }
    
    [Fact]
    public async Task GetDashboardStatsAsync_ShouldThrow_WhenUserIsNotSeller()
    {
        var user = new User
        {
            Id = 1,
            RoleId = (int)RoleEnum.Customer
        };

        _userRepositoryMock
            .Setup(x => x.GetWithRolesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var act = async () =>
            await _dashboardService.GetDashboardStatsAsync(1);

        await act.Should().ThrowAsync<ForbiddenException>();
    }
    
    [Fact]
    public async Task GetDashboardStatsAsync_ShouldThrow_WhenUserNotFound()
    {
        _userRepositoryMock
            .Setup(x => x.GetWithRolesAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var  act = async () =>
           await  _dashboardService.GetDashboardStatsAsync(1);

        await act.Should().ThrowAsync<UserNotFoundException>();
    }
}