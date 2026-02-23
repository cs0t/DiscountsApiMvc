using System.Linq.Expressions;
using Discounts.Application.Commands;
using Discounts.Application.Exceptions;
using Discounts.Application.Exceptions.CategoryExceptions;
using Discounts.Application.Exceptions.UserExceptions;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Interfaces.SystemSettingsContracts;
using Discounts.Application.Services.SellerModuleServices;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Discounts.Tests.SellerModuleTests;

public class OfferManagementServiceTests
{
    private readonly OfferManagementService _offerManagementService;
    private readonly Mock<IOfferRepository> _offerRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<ISystemSettingsService> _systemSettingsServiceMock; 
    
    public OfferManagementServiceTests()
    {
        _offerRepositoryMock = new Mock<IOfferRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _systemSettingsServiceMock = new Mock<ISystemSettingsService>();
        _offerManagementService = new OfferManagementService(
            _offerRepositoryMock.Object, 
            _userRepositoryMock.Object, 
            _categoryRepositoryMock.Object, 
            _systemSettingsServiceMock.Object);
    }

    [Fact]
    public async Task CreateOfferAsync_ShouldCreateOffer_WhenValid()
    {   
        var sellerId = 1;

        var command = new CreateOfferCommand
        {
            Title = "Test",
            Description = "Test Description",
            OriginalPrice = 100,
            DiscountedPrice = 80,
            MaxQuantity = 10,
            ExpirationDate = DateTime.UtcNow.AddDays(10),
            CategoryIds = new List<int> { 1, 2 }
        };

        var seller = new User
        {
            Id = sellerId,
            Role = new Role { Id = (int)RoleEnum.Seller }
        };

        var categories = new List<Category>
        {
            new Category { Id = 1 },
            new Category { Id = 2 }
        };

        _userRepositoryMock
            .Setup(x => x.ExistsAsync(u => u.Id == sellerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _userRepositoryMock
            .Setup(x => x.GetWithRolesAsync(sellerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(seller);

        _categoryRepositoryMock
            .Setup(x => x.GetById(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories[0]);

        _categoryRepositoryMock
            .Setup(x => x.GetById(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories[1]);

        _systemSettingsServiceMock
            .Setup(x => x.GetSettingValueByKeyAsync<int>(
                SystemSettingNames.OfferEditingTimeLimitInHours, It.IsAny<CancellationToken>()))
            .ReturnsAsync(24);

        _offerRepositoryMock
            .Setup(x => x.AddAndReturnAsync(It.IsAny<Offer>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Offer o, CancellationToken ct) => o);

        var result = await _offerManagementService.CreateOfferAsync(command, sellerId);

        _offerRepositoryMock.Verify(x =>
            x.AddAndReturnAsync(It.Is<Offer>(o =>
                o.Title == command.Title &&
                o.SellerId == sellerId &&
                o.StatusId == (int)OfferStatusesEnum.Pending &&
                o.RemainingQuantity == command.MaxQuantity),
            It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.Equal(command.Title, result.Title);
    }
    
    [Fact]
    public async Task CreateOfferAsync_ShouldThrow_WhenNotSeller()
    {   
        var sellerId = 1;

        var command = new CreateOfferCommand
        {
            Title = "Test",
            Description = "Test Description",
            OriginalPrice = 100,
            DiscountedPrice = 80,
            MaxQuantity = 10,
            ExpirationDate = DateTime.UtcNow.AddDays(10),
            CategoryIds = new List<int> { 1, 2 }
        };

        var seller = new User
        {
            Id = sellerId,
            Role = new Role { Id = (int)RoleEnum.Customer }
        };

        var categories = new List<Category>
        {
            new Category { Id = 1 },
            new Category { Id = 2 }
        };

        _userRepositoryMock
            .Setup(x => x.ExistsAsync(u => u.Id == sellerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _userRepositoryMock
            .Setup(x => x.GetWithRolesAsync(sellerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(seller);

        _categoryRepositoryMock
            .Setup(x => x.GetById(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories[0]);

        _categoryRepositoryMock
            .Setup(x => x.GetById(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories[1]);

        _systemSettingsServiceMock
            .Setup(x => x.GetSettingValueByKeyAsync<int>(
                SystemSettingNames.OfferEditingTimeLimitInHours, It.IsAny<CancellationToken>()))
            .ReturnsAsync(24);

        _offerRepositoryMock
            .Setup(x => x.AddAndReturnAsync(It.IsAny<Offer>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Offer o, CancellationToken ct) => o);

        var act = async () => await _offerManagementService.CreateOfferAsync(command, sellerId);

        await act.Should().ThrowAsync<ForbiddenException>();
        
        _offerRepositoryMock.Verify(x =>
            x.AddAndReturnAsync(It.IsAny<Offer>(),
            It.IsAny<CancellationToken>()),
            Times.Never);
    }
    
     [Fact]
    public async Task CreateOfferAsync_ShouldThrow_WhenCategoryNotFound()
    {   
        var sellerId = 1;

        var command = new CreateOfferCommand
        {
            Title = "Test",
            Description = "Test Description",
            OriginalPrice = 100,
            DiscountedPrice = 80,
            MaxQuantity = 10,
            ExpirationDate = DateTime.UtcNow.AddDays(10),
            CategoryIds = new List<int> { 1, 2 }
        };

        var seller = new User
        {
            Id = sellerId,
            Role = new Role { Id = (int)RoleEnum.Seller }
        };

        var categories = new List<Category>
        {
            new Category { Id = 1 },
            new Category { Id = 2 }
        };

        _userRepositoryMock
            .Setup(x => x.ExistsAsync(u => u.Id == sellerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _userRepositoryMock
            .Setup(x => x.GetWithRolesAsync(sellerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(seller);

        _categoryRepositoryMock
            .Setup(x => x.GetById(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories[0]);

        _categoryRepositoryMock
            .Setup(x => x.GetById(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category)null);

        _systemSettingsServiceMock
            .Setup(x => x.GetSettingValueByKeyAsync<int>(
                SystemSettingNames.OfferEditingTimeLimitInHours, It.IsAny<CancellationToken>()))
            .ReturnsAsync(24);

        _offerRepositoryMock
            .Setup(x => x.AddAndReturnAsync(It.IsAny<Offer>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Offer o, CancellationToken ct) => o);

        var act = async () => await _offerManagementService.CreateOfferAsync(command, sellerId);

        await act.Should().ThrowAsync<CategoryNotFoundException>();
        
        _offerRepositoryMock.Verify(x =>
            x.AddAndReturnAsync(It.IsAny<Offer>(),
            It.IsAny<CancellationToken>()),
            Times.Never);
    }
    
    [Fact]
    public async Task CreateOfferAsync_ShouldThrow_WhenUserNotFound()
    {
        _userRepositoryMock
            .Setup(x => x.ExistsAsync(It.IsAny<Expression<Func<User,bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = async () => await _offerManagementService.CreateOfferAsync(new CreateOfferCommand(), 1);

        await act.Should().ThrowAsync<UserNotFoundException>();
    }
    
    [Fact]
    public async Task UpdateOfferAsync_UpdateOffer_WhenValid()
    {
        var sellerId = 1;

        var offer = new Offer
        {
            Id = 10,
            SellerId = sellerId,
            StatusId = (int)OfferStatusesEnum.Pending,
            EditableUntil = DateTime.UtcNow.AddHours(5),
            Categories = new List<Category>()
        };

        var command = new UpdateOfferCommand
        {
            OfferId = 10,
            Title = "Updated",
            Description = "Updated desc",
            OriginalPrice = 200,
            DiscountedPrice = 150,
            MaxQuantity = 20,
            RemainingQuantity = 15,
            ExpirationDate = DateTime.UtcNow.AddDays(5),
            CategoryIds = new List<int> { 1 }
        };

        var categories = new List<Category>
        {
            new Category { Id = 1 }
        };

        _offerRepositoryMock
            .Setup(x => x.GetWithDetailsByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        _categoryRepositoryMock
            .Setup(x => x.GetByPredicateAsync(
                It.IsAny<Expression<Func<Category,bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var result = await _offerManagementService.UpdateOfferAsync(command, sellerId);

        
        Assert.Equal("Updated", result.Title);
        Assert.Equal(15, result.RemainingQuantity);

        _offerRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task UpdateOfferAsync_ShouldSetStatusToPending_WhenApproved()
    {
        var offer = new Offer
        {
            Id = 1,
            SellerId = 1,
            StatusId = (int)OfferStatusesEnum.Approved,
            EditableUntil = DateTime.UtcNow.AddHours(1),
            Categories = new List<Category>()
        };

        _offerRepositoryMock
            .Setup(x => x.GetWithDetailsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        _categoryRepositoryMock
            .Setup(x => x.GetByPredicateAsync(It.IsAny<Expression<Func<Category,bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category>());

        var command = new UpdateOfferCommand
        {
            OfferId = 1,
            CategoryIds = new List<int>()
        };

        await _offerManagementService.UpdateOfferAsync(command, 1);

        Assert.Equal((int)OfferStatusesEnum.Pending, offer.StatusId);
        Assert.Null(offer.ApprovedAt);
    }
    
    [Fact]
    public async Task UpdateOfferAsync_ShouldThrow_WhenNoOwnership()
    {
        var sellerId = 1;

        var offer = new Offer
        {
            Id = 10,
            SellerId = 2,
            StatusId = (int)OfferStatusesEnum.Pending,
            EditableUntil = DateTime.UtcNow.AddHours(5),
            Categories = new List<Category>()
        };

        var command = new UpdateOfferCommand
        {
            OfferId = 10,
            Title = "Updated",
            Description = "Updated desc",
            OriginalPrice = 200,
            DiscountedPrice = 150,
            MaxQuantity = 20,
            RemainingQuantity = 15,
            ExpirationDate = DateTime.UtcNow.AddDays(5),
            CategoryIds = new List<int> { 1 }
        };

        var categories = new List<Category>
        {
            new Category { Id = 1 }
        };

        _offerRepositoryMock
            .Setup(x => x.GetWithDetailsByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        _categoryRepositoryMock
            .Setup(x => x.GetByPredicateAsync(
                It.IsAny<Expression<Func<Category,bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var act = async () => await _offerManagementService.UpdateOfferAsync(command, sellerId);
        await act.Should().ThrowAsync<ForbiddenException>();
        
        
        _offerRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task UpdateOfferAsync_ShouldThrow_WhenOfferNotEditable()
    {
        var sellerId = 1;

        var offer = new Offer
        {
            Id = 10,
            SellerId = sellerId,
            StatusId = (int)OfferStatusesEnum.Pending,
            EditableUntil = DateTime.UtcNow.AddHours(-1),
            Categories = new List<Category>()
        };

        var command = new UpdateOfferCommand
        {
            OfferId = 10,
            Title = "Updated",
            Description = "Updated desc",
            OriginalPrice = 200,
            DiscountedPrice = 150,
            MaxQuantity = 20,
            RemainingQuantity = 15,
            ExpirationDate = DateTime.UtcNow.AddDays(5),
            CategoryIds = new List<int> { 1 }
        };

        var categories = new List<Category>
        {
            new Category { Id = 1 }
        };

        _offerRepositoryMock
            .Setup(x => x.GetWithDetailsByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        _categoryRepositoryMock
            .Setup(x => x.GetByPredicateAsync(
                It.IsAny<Expression<Func<Category,bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(categories);

        var act = async () => await _offerManagementService.UpdateOfferAsync(command, sellerId);
        await act.Should().ThrowAsync<ForbiddenException>();
        
        
        _offerRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task UpdateOfferAsync_ShouldThrow_WhenCategoryNotFound()
    {
        var sellerId = 1;

        var offer = new Offer
        {
            Id = 10,
            SellerId = sellerId,
            StatusId = (int)OfferStatusesEnum.Pending,
            EditableUntil = DateTime.UtcNow.AddHours(5),
            Categories = new List<Category>()
        };

        var command = new UpdateOfferCommand
        {
            OfferId = 10,
            Title = "Updated",
            Description = "Updated desc",
            OriginalPrice = 200,
            DiscountedPrice = 150,
            MaxQuantity = 20,
            RemainingQuantity = 15,
            ExpirationDate = DateTime.UtcNow.AddDays(5),
            CategoryIds = new List<int> { 1 }
        };

        _offerRepositoryMock
            .Setup(x => x.GetWithDetailsByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        _categoryRepositoryMock
            .Setup(x => x.GetByPredicateAsync(
                It.IsAny<Expression<Func<Category,bool>>>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Category>());

        var act = async () => await _offerManagementService.UpdateOfferAsync(command, sellerId);
        await act.Should().ThrowAsync<CategoryNotFoundException>();
        
        
        _offerRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DisableOfferAsync_ShouldDisableOffer_WhenValid()
    {
        var sellerId = 1;

        var offer = new Offer
        {
            Id = 10,
            SellerId = sellerId,
            StatusId = (int)OfferStatusesEnum.Pending,
            EditableUntil = DateTime.UtcNow.AddHours(5)
        };

        _offerRepositoryMock
            .Setup(x => x.GetById(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        await _offerManagementService.DisableOfferAsync(10, sellerId);

        _offerRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal((int)OfferStatusesEnum.Disabled, offer.StatusId);
    }
    
    [Fact]
    public async Task DisableOfferAsync_ShouldThrow_WhenNoOwnership()
    {
        var sellerId = 1;

        var offer = new Offer
        {
            Id = 10,
            SellerId = 2,
            StatusId = (int)OfferStatusesEnum.Pending,
            EditableUntil = DateTime.UtcNow.AddHours(5)
        };

        _offerRepositoryMock
            .Setup(x => x.GetById(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        var act = async () => await _offerManagementService.DisableOfferAsync(10, sellerId);
        await act.Should().ThrowAsync<ForbiddenException>();
        
        
        _offerRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task DisableOfferAsync_ShouldReturn_WhenOfferAlreadyExpired()
    {
        var sellerId = 1;

        var offer = new Offer
        {
            Id = 10,
            SellerId = sellerId,
            StatusId = (int)OfferStatusesEnum.Expired,
            EditableUntil = DateTime.UtcNow.AddHours(-1)
        };

        _offerRepositoryMock
            .Setup(x => x.GetById(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        // var act = async () => await _offerManagementService.DisableOfferAsync(10, sellerId);
        // await act.Should().ThrowAsync<ForbiddenException>();
        
        
        _offerRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task DisableOfferAsync_ShouldReturn_WhenOfferAlreadyDisabled()
    {
        var sellerId = 1;

        var offer = new Offer
        {
            Id = 10,
            SellerId = sellerId,
            StatusId = (int)OfferStatusesEnum.Disabled,
            EditableUntil = DateTime.UtcNow.AddHours(5)
        };

        _offerRepositoryMock
            .Setup(x => x.GetById(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(offer);

        // var act = async () => await _offerManagementService.DisableOfferAsync(10, sellerId);
        // await act.Should().ThrowAsync<ForbiddenException>();
        
        
        _offerRepositoryMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}