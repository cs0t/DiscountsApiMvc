using Discounts.Application.Commands;
using Discounts.Application.Exceptions;
using Discounts.Application.Exceptions.CategoryExceptions;
using Discounts.Application.Exceptions.OfferExceptions;
using Discounts.Application.Exceptions.UserExceptions;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Interfaces.SellerModuleServiceContracts;
using Discounts.Application.Interfaces.SystemSettingsContracts;
using Discounts.Application.Models;
using Discounts.Application.Queries;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;

namespace Discounts.Application.Services.SellerModuleServices;

public class OfferManagementService : IOfferManagementService
{
    private readonly IOfferRepository _offerRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ISystemSettingsService _systemSettingsService; 

    public OfferManagementService(
        IOfferRepository offerRepository, 
        IUserRepository userRepository,
        ICategoryRepository categoryRepository,
        ISystemSettingsService systemSettingsService)
    {
        _offerRepository = offerRepository;
        _userRepository = userRepository;
        _categoryRepository = categoryRepository;
        _systemSettingsService = systemSettingsService;
    }

    public async Task<int?> CreateOfferAsync(CreateOfferCommand offerCommand, int sellerId, CancellationToken ct = default)
    {
        //check if user is authorized to create offer
        if (!await _userRepository.ExistsAsync(u => u.Id == sellerId, ct))
        {
            throw new UserNotFoundException($"User with id {sellerId} not found !");
        }

        var user = await _userRepository.GetWithRolesAsync(sellerId, ct);
        if (user is null || user.Role.Id != (int)RoleEnum.Seller)
        {
            throw new UnauthorizedException("User is not authorized to create offers.");
        }
        //validate categories
        foreach (var categoryId in offerCommand.CategoryIds)
        {
            if(!await _categoryRepository.ExistsAsync(cat => cat.Id == categoryId, ct))
            {
                throw new CategoryNotFoundException($"Category with id {categoryId} not found !");
            }
        }
        
        var now = DateTime.UtcNow;
        
        var editableHoursLimit = await _systemSettingsService
            .GetSettingValueByKeyAsync<int>(SystemSettingNames.OfferEditingTimeLimitInHours,ct);
        
        var offer = new Offer
        {
            SellerId = sellerId,
            StatusId = (int)OfferStatusesEnum.Pending,
            CreatedAt = now,
            //editableuntil to 24 hours after creation
            EditableUntil = now.AddHours(editableHoursLimit),
            Title = offerCommand.Title,
            Description = offerCommand.Description,
            OriginalPrice = offerCommand.OriginalPrice,
            DiscountedPrice = offerCommand.DiscountedPrice,
            MaxQuantity = offerCommand.MaxQuantity,
            RemainingQuantity = offerCommand.MaxQuantity,
            ExpirationDate = offerCommand.ExpirationDate,
            Categories = offerCommand.CategoryIds.Select(id => new Category { Id = id }).ToList()
        };
        
        //await _categoryRepository.AttachCategoriesByIdsAsync(offer.Categories.Select(c => c.Id).ToList(), ct);
        await _offerRepository.Add(offer, ct);
        await _offerRepository.SaveChangesAsync(ct);
        return offer.Id;
    }

    public async Task UpdateOfferAsync(UpdateOfferCommand offerCommand, int sellerId, CancellationToken ct = default)
    {
        var existingOffer = await _offerRepository.GetWithDetailsByIdAsync(offerCommand.OfferId, ct);
        if (existingOffer is null)
        {
            throw new OfferNotFoundException($"Offer with id {offerCommand.OfferId} not found !");
        }

        //check ownership
        if (existingOffer.SellerId != sellerId)
        {
            throw new UnauthorizedException("User is not authorized to update this offer.");
        }

        //check statuses
        if (existingOffer.StatusId != (int)OfferStatusesEnum.Pending
            && existingOffer.StatusId != (int)OfferStatusesEnum.Approved)
        {
            throw new ForbiddenException("Only pending or approved offers can be updated in given time !");
        }

        //check if editable
        var dateNow = DateTime.UtcNow;
        if (existingOffer.EditableUntil < dateNow)
        {
            throw new ForbiddenException("Offer can no longer be edited !");
        }

        //update offer
        existingOffer.Title = offerCommand.Title;
        existingOffer.Description = offerCommand.Description;
        existingOffer.OriginalPrice = offerCommand.OriginalPrice;
        existingOffer.DiscountedPrice = offerCommand.DiscountedPrice;
        existingOffer.MaxQuantity = offerCommand.MaxQuantity;
        existingOffer.RemainingQuantity = offerCommand.RemainingQuantity;
        existingOffer.ExpirationDate = offerCommand.ExpirationDate;
        existingOffer.Categories = offerCommand.CategoryIds.Select(id => new Category { Id = id }).ToList();

        if (existingOffer.StatusId == (int)OfferStatusesEnum.Approved)
        {
            existingOffer.StatusId = (int)OfferStatusesEnum.Pending;
            existingOffer.ApprovedAt = null;
        }

        existingOffer.UpdatedAt = dateNow;
        
        _offerRepository.Update(existingOffer);
        await _offerRepository.SaveChangesAsync(ct);
    }

    public async Task DisableOfferAsync(int offerId, int sellerId, CancellationToken ct = default)
    {
        var existingOffer = await _offerRepository.GetById(offerId, ct);
        if (existingOffer is null)
        {
            throw new OfferNotFoundException($"Offer with id {offerId} not found !");
        }
        
        if (existingOffer.SellerId != sellerId)
        {
            throw new UnauthorizedException("User is not authorized to disable this offer.");
        }

        if (existingOffer.StatusId == (int)OfferStatusesEnum.Expired ||
            existingOffer.StatusId == (int)OfferStatusesEnum.Disabled)
            return;
        
        existingOffer.StatusId = (int)OfferStatusesEnum.Disabled;
        existingOffer.DisabledAt = DateTime.UtcNow;
        _offerRepository.Update(existingOffer);
        await _offerRepository.SaveChangesAsync(ct);
    }

    

    public async Task<PagedResult<Offer>> GetMerchantOffersAsync(OfferListQuery query, int sellerId, CancellationToken ct = default)
    {
        //check if user is authorized to view offers
        var existingUser = await _userRepository.GetWithRolesAsync(sellerId, ct);
        if (existingUser is null || existingUser.Role.Id != (int)RoleEnum.Seller)
        {
            throw new UnauthorizedException("User is not authorized to view offers !");
        }
        
        return await _offerRepository.GetBySellerAsync(query, sellerId, ct);
    }
}