using Discounts.Application.Commands.SellerCommands.Offers;
using Discounts.Application.Exceptions.CategoryExceptions;
using Discounts.Application.Interfaces.AuthContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Interfaces.SystemSettingsContracts;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Handlers.Seller.Offers;

public class CreateOfferHandler
    (IOfferRepository offerRepository,
        ICategoryRepository categoryRepository,
        ICurrentUserService  currentUserService,
        ISystemSettingsService systemSettingsService)
    : IRequestHandler<CreateOfferCommand,Offer>
{
    public async Task<Offer> Handle(CreateOfferCommand request, CancellationToken ct = default)
    {
        //validate categories
        var categories = new List<Category>();
        foreach (var categoryId in request.CategoryIds)
        {
            var category = await categoryRepository.GetById(categoryId, ct);
            if(category is null)
            {
                throw new CategoryNotFoundException($"Category with id {categoryId} not found !");
            }
            categories.Add(category);   
        }
        
        var now = DateTime.UtcNow;
        
        var editableHoursLimit = await systemSettingsService
            .GetSettingValueByKeyAsync<int>(SystemSettingNames.OfferEditingTimeLimitInHours,ct);
        
        var offer = new Offer
        {
            SellerId = currentUserService.UserId,
            StatusId = (int)OfferStatusesEnum.Pending,
            CreatedAt = now,
            //editableuntil to 24 hours after creation
            EditableUntil = now.AddHours(editableHoursLimit),
            Title = request.Title,
            Description = request.Description,
            OriginalPrice = request.OriginalPrice,
            DiscountedPrice = request.DiscountedPrice,
            MaxQuantity = request.MaxQuantity,
            RemainingQuantity = request.MaxQuantity,
            ExpirationDate = request.ExpirationDate,
            Categories = categories
        };
        
        var createdOffer = await offerRepository.AddAndReturnAsync(offer, ct);
        return createdOffer;
    }
}