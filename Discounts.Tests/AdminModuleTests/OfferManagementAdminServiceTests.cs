using Discounts.Application.Commands;
using Discounts.Application.Exceptions;
using Discounts.Application.Exceptions.OfferExceptions;
using Discounts.Application.Exceptions.UserExceptions;
using Discounts.Application.Interfaces.AdminModuleContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Services.AdminModuleServices;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Discounts.Tests.AdminModuleTests;

public class OfferManagementAdminServiceTests
{
    private readonly IOfferManagementAdminService _offerManagementAdminService;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IOfferRepository> _offerRepositoryMock;
    
    public OfferManagementAdminServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _offerRepositoryMock = new Mock<IOfferRepository>();
        _offerManagementAdminService = new OfferManagementAdminService(_userRepositoryMock.Object, _offerRepositoryMock.Object);
    }
    
    [Fact]
    public async Task ApproveOfferAsync_ShouldApproveOffer_WhenAdmin()
    {
        var adminId = 1;
        var offerId = 1;
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(adminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = adminId, RoleId = (int)RoleEnum.Administrator });
        
        _offerRepositoryMock.Setup(repo => repo.GetWithDetailsByIdAsync(offerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Offer { Id = offerId, StatusId = (int)OfferStatusesEnum.Pending, ExpirationDate = DateTime.UtcNow.AddDays(1) });
        
        await _offerManagementAdminService.ApproveOfferAsync(adminId, offerId);
        
        _offerRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task ApproveOfferAsync_ShouldThrow_WhenOfferNotPending()
    {
        var adminId = 1;
        var offerId = 1;
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(adminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = adminId, RoleId = (int)RoleEnum.Administrator });
        
        _offerRepositoryMock.Setup(repo => repo.GetWithDetailsByIdAsync(offerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Offer { Id = offerId, StatusId = (int)OfferStatusesEnum.Expired, ExpirationDate = DateTime.UtcNow.AddDays(-1) });
        
        var act = async() => await _offerManagementAdminService.ApproveOfferAsync(adminId, offerId);
        
        await act.Should().ThrowAsync<ApplicationException>();
        
        _offerRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task ApproveOfferAsync_ShouldThrow_WhenNotAdmin()
    {
        var adminId = 1;
        var offerId = 1;
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(adminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = adminId, RoleId = (int)RoleEnum.Customer });
        
        _offerRepositoryMock.Setup(repo => repo.GetWithDetailsByIdAsync(offerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Offer { Id = offerId, StatusId = (int)OfferStatusesEnum.Pending, ExpirationDate = DateTime.UtcNow.AddDays(1) });
        
        var act = async() => await _offerManagementAdminService.ApproveOfferAsync(adminId, offerId);
        
        await act.Should().ThrowAsync<ForbiddenException>();
        
        _offerRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task ApproveOfferAsync_ShouldThrow_WhenAdminNotFound()
    {
        var adminId = 1;
        var offerId = 1;
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(adminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User) null);
        
        _offerRepositoryMock.Setup(repo => repo.GetWithDetailsByIdAsync(offerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Offer { Id = offerId, StatusId = (int)OfferStatusesEnum.Pending, ExpirationDate = DateTime.UtcNow.AddDays(1) });
        
        var act = async() => await _offerManagementAdminService.ApproveOfferAsync(adminId, offerId);
        
        await act.Should().ThrowAsync<UserNotFoundException>();
        
        _offerRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task ApproveOfferAsync_ShouldThrow_WhenOfferNotFound()
    {
        var adminId = 1;
        var offerId = 1;
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(adminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = adminId, RoleId = (int)RoleEnum.Administrator });
        
        _offerRepositoryMock.Setup(repo => repo.GetWithDetailsByIdAsync(offerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Offer) null);
        
        var act = async() => await _offerManagementAdminService.ApproveOfferAsync(adminId, offerId);
        
        await act.Should().ThrowAsync<OfferNotFoundException>();
        
        _offerRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task RejectOfferAsync_ShouldRejectOffer_WhenAdmin()
    {
        var adminId = 1;
        
        var command = new RejectOfferCommand { OfferId = 1, Reason = "Reason ... " };
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(adminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = adminId, RoleId = (int)RoleEnum.Administrator });
        
        _offerRepositoryMock.Setup(repo => repo.GetWithDetailsByIdAsync(command.OfferId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Offer { Id = command.OfferId, StatusId = (int)OfferStatusesEnum.Pending, ExpirationDate = DateTime.UtcNow.AddDays(1) });
        
        await _offerManagementAdminService.RejectOfferAsync(adminId, command);
        
        _offerRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task RejectOfferAsync_ShouldThrow_WhenNotAdmin()
    {
        var adminId = 1;
        
        var command = new RejectOfferCommand { OfferId = 1, Reason = "Reason ... " };
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(adminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = adminId, RoleId = (int)RoleEnum.Customer });
        
        _offerRepositoryMock.Setup(repo => repo.GetWithDetailsByIdAsync(command.OfferId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Offer { Id = command.OfferId, StatusId = (int)OfferStatusesEnum.Pending, ExpirationDate = DateTime.UtcNow.AddDays(1) });
        
        var act = async() => await _offerManagementAdminService.RejectOfferAsync(adminId, command);
        
        await act.Should().ThrowAsync<ForbiddenException>();
        
        _offerRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task RejectOfferAsync_ShouldThrow_WhenOfferNotPending()
    {
        var adminId = 1;
        
        var command = new RejectOfferCommand { OfferId = 1, Reason = "Reason ... " };
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(adminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = adminId, RoleId = (int)RoleEnum.Administrator });
        
        _offerRepositoryMock.Setup(repo => repo.GetWithDetailsByIdAsync(command.OfferId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Offer { Id = command.OfferId, StatusId = (int)OfferStatusesEnum.Expired, ExpirationDate = DateTime.UtcNow.AddDays(-1) });
        
        var act = async() => await _offerManagementAdminService.RejectOfferAsync(adminId, command);
        
        await act.Should().ThrowAsync<ApplicationException>();
        
        _offerRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task RejectOfferAsync_ShouldThrow_WhenOfferNotFound()
    {
        var adminId = 1;
        
        var command = new RejectOfferCommand { OfferId = 1, Reason = "Reason ... " };
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(adminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = adminId, RoleId = (int)RoleEnum.Administrator });
        
        _offerRepositoryMock.Setup(repo => repo.GetWithDetailsByIdAsync(command.OfferId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Offer) null);
        
        var act = async() => await _offerManagementAdminService.RejectOfferAsync(adminId, command);
        
        await act.Should().ThrowAsync<OfferNotFoundException>();
        
        _offerRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task RejectOfferAsync_ShouldThrow_WhenAdminNotFound()
    {
        var adminId = 1;
        
        var command = new RejectOfferCommand { OfferId = 1, Reason = "Reason ... " };
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(adminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User) null);
        
        _offerRepositoryMock.Setup(repo => repo.GetWithDetailsByIdAsync(command.OfferId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Offer { Id = command.OfferId, StatusId = (int)OfferStatusesEnum.Expired, ExpirationDate = DateTime.UtcNow.AddDays(-1) });
        
        var act = async() => await _offerManagementAdminService.RejectOfferAsync(adminId, command);
        
        await act.Should().ThrowAsync<UserNotFoundException>();
        
        _offerRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}