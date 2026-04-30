using Discounts.Application.Commands.SellerCommands.Offers;
using FluentValidation;

namespace Discounts.Application.Validators.Seller;

public class UpdateOfferCommandValidator : AbstractValidator<UpdateOfferCommand>
{
    public UpdateOfferCommandValidator()
    {
        RuleFor(x => x.OfferId).GreaterThan(0)
            .WithMessage("OfferId must be a positive integer.");
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200)
            .WithMessage("Title is required and must not exceed 200 characters.");
        RuleFor(x => x.Description).MaximumLength(1000)
            .WithMessage("Description must not exceed 1000 characters.");
        RuleFor(x => x.OriginalPrice).GreaterThan(0)
            .WithMessage("OriginalPrice must be greater than 0.");
        RuleFor(x => x.DiscountedPrice).GreaterThan(0)
            .LessThan(x => x.OriginalPrice)
            .WithMessage("DiscountedPrice must be greater than 0 and less than OriginalPrice.");
        RuleFor(x => x.MaxQuantity).GreaterThan(0)
            .WithMessage("MaxQuantity must be greater than 0.");
        RuleFor(x => x.RemainingQuantity).GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(x => x.MaxQuantity)
            .WithMessage("RemainingQuantity must be between 0 and MaxQuantity.");
        RuleFor(x => x.ExpirationDate).GreaterThan(DateTime.UtcNow)
            .WithMessage("ExpirationDate must be a future date.");
        RuleFor(x => x.CategoryIds).NotEmpty()
            .WithMessage("At least one category must be selected.");
    }
}

