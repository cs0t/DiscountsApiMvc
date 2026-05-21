using Discounts.Application.Commands.SellerCommands.Offers;
using Discounts.Application.Exceptions;
using Discounts.Application.Exceptions.CategoryExceptions;
using Discounts.Application.Exceptions.OfferExceptions;
using Discounts.Application.Interfaces.AuthContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Handlers.Seller.Offers;

public class UpdateOfferHandler(
    IOfferRepository offerRepository,
    ICategoryRepository categoryRepository,
    ICurrentUserService currentUserService)
    : IRequestHandler<UpdateOfferCommand, int>
{
    public async Task<int> Handle(UpdateOfferCommand request, CancellationToken ct = default)
    {
        var existingOffer = await offerRepository.GetWithDetailsByIdAsync(request.OfferId, ct);
        if (existingOffer is null)
            throw new OfferNotFoundException($"Offer with id {request.OfferId} not found !");

        if (existingOffer.SellerId != currentUserService.UserId)
            throw new ForbiddenException("User is not authorized to update this offer.");

        if (existingOffer.StatusId != (int)OfferStatusesEnum.Pending
            && existingOffer.StatusId != (int)OfferStatusesEnum.Approved)
            throw new ForbiddenException("Only pending or approved offers can be updated in given time !");

        var dateNow = DateTime.UtcNow;
        if (existingOffer.EditableUntil < dateNow)
            throw new ForbiddenException("Offer can no longer be edited !");

        var categories = await categoryRepository
            .GetByPredicateAsync(c => request.CategoryIds.Contains(c.Id), ct);

        if (categories.Count != request.CategoryIds.Count)
            throw new CategoryNotFoundException("One or more categories were not found !");

        existingOffer.Categories.Clear();
        foreach (var category in categories)
            existingOffer.Categories.Add(category);

        existingOffer.Title = request.Title;
        existingOffer.Description = request.Description;
        existingOffer.OriginalPrice = request.OriginalPrice;
        existingOffer.DiscountedPrice = request.DiscountedPrice;
        existingOffer.MaxQuantity = request.MaxQuantity;
        existingOffer.RemainingQuantity = request.RemainingQuantity;
        existingOffer.ExpirationDate = request.ExpirationDate;
        existingOffer.UpdatedAt = dateNow;

        if (existingOffer.StatusId == (int)OfferStatusesEnum.Approved)
        {
            existingOffer.StatusId = (int)OfferStatusesEnum.Pending;
            existingOffer.ApprovedAt = null;
        }

        await offerRepository.SaveChangesAsync(ct);
        return existingOffer.Id;
    }
}