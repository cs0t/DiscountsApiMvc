using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using Dsicounts.Application.Exceptions;
using Dsicounts.Application.Exceptions.OfferExceptions;
using Dsicounts.Application.Exceptions.UserExceptions;
using Dsicounts.Application.Interfaces.RepositoryContracts;
using Dsicounts.Application.Interfaces.SellerModuleServiceContracts;
using Dsicounts.Application.Models;
using Dsicounts.Application.Queries;

namespace Dsicounts.Application.Services.SellerModuleServices;

public class OfferManagementService : IOfferManagementService
{
    private readonly IOfferRepository _offerRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICategoryRepository _categoryRepository;

    public OfferManagementService(
        IOfferRepository offerRepository, 
        IUserRepository userRepository,
        ICategoryRepository categoryRepository)
    {
        _offerRepository = offerRepository;
        _userRepository = userRepository;
        _categoryRepository = categoryRepository;
    }

    public async Task<int?> CreateOfferAsync(Offer offer, int sellerId, CancellationToken ct = default)
    {
        //check if user is authorized to create offer
        if (!(await _userRepository.ExistsAsync(u => u.Id == sellerId, ct)))
        {
            throw new UserNotFoundException($"User with id {sellerId} not found !");
        }

        var user = await _userRepository.GetWithRolesAsync(sellerId, ct);
        if (user is null || user.Role.Id != (int)RoleEnum.Seller)
        {
            throw new UnauthorizedException("User is not authorized to create offers.");
        }
        //validate categories
        if (offer.Categories is not null && offer.Categories.Count > 0)

        //prepare offer for creation
        offer.SellerId = sellerId;
        offer.StatusId = (int)OfferStatusesEnum.Pending;
        offer.CreatedAt = DateTime.UtcNow;
        //set editableuntil to 24 hours after creation
        offer.EditableUntil = offer.CreatedAt.AddHours(24);
        await _offerRepository.SaveChangesAsync(ct);
        return offer.Id;
    }

    public async Task UpdateOfferAsync(Offer offer, int sellerId, CancellationToken ct = default)
    {
        var existingOffer = await _offerRepository.GetWithDetailsByIdAsync(offer.Id, ct);
        if (existingOffer is null)
        {
            throw new OfferNotFoundException($"Offer with id {offer.Id} not found !");
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
        existingOffer.Title = offer.Title;
        existingOffer.Description = offer.Description;
        existingOffer.OriginalPrice = offer.OriginalPrice;
        existingOffer.DiscountedPrice = offer.DiscountedPrice;
        existingOffer.MaxQuantity = offer.MaxQuantity;
        existingOffer.RemainingQuantity = offer.RemainingQuantity;
        existingOffer.ExpirationDate = offer.ExpirationDate;
        existingOffer.Categories = offer.Categories;

        if (existingOffer.StatusId == (int)OfferStatusesEnum.Approved)
        {
            existingOffer.StatusId = (int)OfferStatusesEnum.Pending;
            existingOffer.ApprovedAt = null;
        }

        existingOffer.UpdatedAt = dateNow;
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