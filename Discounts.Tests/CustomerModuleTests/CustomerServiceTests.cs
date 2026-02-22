using Discounts.Application.Commands;
using Discounts.Application.Exceptions.OfferExceptions;
using Discounts.Application.Interfaces.CustomerModuleContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Interfaces.SystemSettingsContracts;
using Discounts.Application.Interfaces.UnitOfWorkContracts;
using Discounts.Application.Services.CustomerModuleServices;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Discounts.Tests.CustomerModuleTests;

public class CustomerServiceTests
{
    private readonly ICustomerService _customerService;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IOfferRepository> _offerRepositoryMock;
    private readonly Mock<ICouponRepository> _couponRepositoryMock;
    private readonly Mock<IReservationRepository> _reservationRepositoryMock;
    private readonly Mock<ISystemSettingsService> _settingsServiceMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    
    public CustomerServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _offerRepositoryMock = new Mock<IOfferRepository>();
        _couponRepositoryMock = new Mock<ICouponRepository>();
        _reservationRepositoryMock = new Mock<IReservationRepository>();
        _settingsServiceMock = new Mock<ISystemSettingsService>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _customerService = new CustomerService(
            _userRepositoryMock.Object,
            _offerRepositoryMock.Object,
            _couponRepositoryMock.Object,
            _reservationRepositoryMock.Object,
            _settingsServiceMock.Object,
            _unitOfWorkMock.Object);
    }

    private Offer CreateOfferForTest(byte[] rowVersion) => new Offer
    {
        Id = 1,
        StatusId = (int)OfferStatusesEnum.Approved,
        ExpirationDate = DateTime.UtcNow.AddHours(1),
        RemainingQuantity = 10,
        RowVersion = rowVersion
    };
    
    [Fact]
    public async Task CreateReservationAsync_ShouldCreateReservation_WhenValid()
    {
        var customerId = 1;
        var offerId = 1;
        var rowVersion = new byte[] {1,2,3};
        
        var offer = CreateOfferForTest(rowVersion);

        _offerRepositoryMock
            .Setup(x => x.GetWithDetailsByIdAsync(offerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        _reservationRepositoryMock
            .Setup(x => 
                x.GetActiveReservationByUserIdAndOfferIdAsync(customerId, offerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reservation?)null);

        _userRepositoryMock
            .Setup(x => x.GetWithRolesAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = customerId, RoleId = (int)RoleEnum.Customer });

        _settingsServiceMock
            .Setup(x => x.GetSettingValueByKeyAsync<int>(
                SystemSettingNames.ReservationTimeLimitInHours, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        _unitOfWorkMock
            .Setup(u => u.ExecuteInTransactionAsync(
                It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>((action, _) => action());

        await _customerService.CreateReservationAsync(customerId, offerId, rowVersion);

        _reservationRepositoryMock.Verify(x =>
                x.Add(It.Is<Reservation>
                        (r => r.UserId == customerId && r.OfferId == offerId && r.IsActive == true),
                    It.IsAny<CancellationToken>()), Times.Once);

        Assert.Equal(9, offer.RemainingQuantity);
    }
    
    [Fact]
    public async Task CreateReservationAsync_ShouldThrow_WhenNotCustomer()
    {
        var customerId = 1;
        var offerId = 1;
        var rowVersion = new byte[] {1,2,3};
        
        var offer = CreateOfferForTest(rowVersion);

        _offerRepositoryMock
            .Setup(x => x.GetWithDetailsByIdAsync(offerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        _reservationRepositoryMock
            .Setup(x => 
                x.GetActiveReservationByUserIdAndOfferIdAsync(customerId, offerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reservation?)null);

        _userRepositoryMock
            .Setup(x => x.GetWithRolesAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = customerId, RoleId = (int)RoleEnum.Seller });

        _settingsServiceMock
            .Setup(x => x.GetSettingValueByKeyAsync<int>(
                SystemSettingNames.ReservationTimeLimitInHours, It.IsAny<CancellationToken>()))
            .ReturnsAsync(5);

        _unitOfWorkMock
            .Setup(u => u.ExecuteInTransactionAsync(
                It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>((action, _) => action());

        var action = async() => await _customerService.CreateReservationAsync(customerId, offerId, rowVersion);
        
        await action.Should().ThrowAsync<ApplicationException>();

        _reservationRepositoryMock.Verify(x =>
            x.Add(It.IsAny<Reservation>(), It.IsAny<CancellationToken>()), Times.Never);

        Assert.Equal(10, offer.RemainingQuantity);
    }
    
    [Fact]
    public async Task CreateReservationAsync_ShouldThrow_WhenOfferNotFound()
    {
        _offerRepositoryMock
            .Setup(x => x.GetWithDetailsByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Offer?)null);

        _unitOfWorkMock
            .Setup(u => u.ExecuteInTransactionAsync(
                It.IsAny<Func<Task>>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>((action, _) => action());

        var act = async () => 
            await _customerService.CreateReservationAsync(1, 1, new byte[] { 1 });

        await act.Should().ThrowAsync<OfferNotFoundException>();
    }

    [Fact]
    public async Task CreateReservationAsync_ShouldThrow_ModifiedCausesMismatch()
    {
        var offer = CreateOfferForTest(new byte[] { 1, 2, 3 });
        
        _offerRepositoryMock
            .Setup(x => x.GetWithDetailsByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        _unitOfWorkMock
            .Setup(u => u.ExecuteInTransactionAsync(
                It.IsAny<Func<Task>>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>((action, _) => action());
        
        var act = async () => 
            await _customerService.CreateReservationAsync(1, 1, new byte[] { 4, 5, 6 });
        
        await act.Should().ThrowAsync<ApplicationException>();
    }
    
    [Fact]
    public async Task PurchaseCouponAsync_ShouldCreateCoupon_AndDeactivateReservation_WhenValid()
    {
        var customerId = 1;
        var offerId = 1;

        var offer = new Offer
        {
            Id = offerId,
            StatusId = (int)OfferStatusesEnum.Approved,
            ExpirationDate = DateTime.UtcNow.AddDays(5)
        };

        var reservation = new Reservation
        {
            UserId = customerId,
            OfferId = offerId,
            IsActive = true,
            ValidUntil = DateTime.UtcNow.AddHours(2)
        };

        var user = new User
        {
            Id = customerId,
            RoleId = (int)RoleEnum.Customer
        };

        _offerRepositoryMock
            .Setup(x => x.GetWithDetailsByIdAsync(offerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        _reservationRepositoryMock
            .Setup(x => x.GetActiveReservationByUserIdAndOfferIdAsync(
                customerId, offerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservation);

        _userRepositoryMock
            .Setup(x => x.GetWithRolesAsync(customerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _unitOfWorkMock
            .Setup(u => u.ExecuteInTransactionAsync(
                It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>((action, _) => action());

        await _customerService.PurchaseCouponAsync(customerId, offerId);

        _couponRepositoryMock.Verify(x =>
                x.Add(It.Is<Coupon>(c =>
                        c.OfferId == offerId &&
                        c.CustomerId == customerId &&
                        c.StatusId == (int)CouponStatusesEnum.Active &&
                        c.Code.Length == 11),
                    It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.False(reservation.IsActive);
        Assert.NotNull(reservation.CancelledAt);
    }
    
    [Fact]
    public async Task PurchaseCouponAsync_ShouldThrow_WhenOfferNotFound()
    {
        _offerRepositoryMock
            .Setup(x => x.GetWithDetailsByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Offer)null);

        _unitOfWorkMock
            .Setup(u => u.ExecuteInTransactionAsync(
                It.IsAny<Func<Task>>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>((action, _) => action());

        var act = async () => await _customerService.PurchaseCouponAsync(1, 1);

        await act.Should().ThrowAsync<OfferNotFoundException>();
    }
    
    [Fact]
    public async Task PurchaseCouponAsync_ShouldThrow_WhenNoReservation()
    {
        var offer = new Offer
        {
            Id = 1,
            StatusId = (int)OfferStatusesEnum.Approved
        };

        _offerRepositoryMock
            .Setup(x => x.GetWithDetailsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        _reservationRepositoryMock
            .Setup(x => x.GetActiveReservationByUserIdAndOfferIdAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Reservation?)null);

        _unitOfWorkMock
            .Setup(u => u.ExecuteInTransactionAsync(
                It.IsAny<Func<Task>>(),
                It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>((action, _) => action());

        var act = async () => await  _customerService.PurchaseCouponAsync(1, 1);

        await act.Should().ThrowAsync<ApplicationException>();
    }
    
    [Fact]
    public async Task PurchaseCouponAsync_ShouldThrow_WhenReservationExpired()
    {
        var offer = new Offer
        {
            Id = 1,
            StatusId = (int)OfferStatusesEnum.Approved
        };

        var reservation = new Reservation
        {
            IsActive = true,
            ValidUntil = DateTime.UtcNow.AddHours(-1)
        };

        _offerRepositoryMock
            .Setup(x => x.GetWithDetailsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        _reservationRepositoryMock
            .Setup(x => x.GetActiveReservationByUserIdAndOfferIdAsync(
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(reservation);

        _unitOfWorkMock
            .Setup(u => u.ExecuteInTransactionAsync(
                It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>((action, _) => action());

        var act = async () => await 
            _customerService.PurchaseCouponAsync(1, 1);

        await act.Should().ThrowAsync<ApplicationException>();
    }
}