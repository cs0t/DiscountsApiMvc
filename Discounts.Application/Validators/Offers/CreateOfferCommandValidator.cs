using Discounts.Application.Commands.SellerCommands.Offers;
using FluentValidation;

namespace Discounts.Application.Validators.Offers;

// Kept for backward compatibility — prefer Discounts.Application.Validators.Seller.CreateOfferCommandValidator
public class CreateOfferCommandValidator : AbstractValidator<CreateOfferCommand>
{
    public CreateOfferCommandValidator()
    {
        RuleFor(command => command.Title).NotEmpty().MaximumLength(200)
            .WithMessage("Title is required and must not exceed 200 characters.");
        RuleFor(command => command.Description).MaximumLength(1000)
            .WithMessage("Description must not exceed 1000 characters.");
        RuleFor(command => command.OriginalPrice).GreaterThan(0)
            .WithMessage("OriginalPrice must be greater than 0.");
        RuleFor(command => command.DiscountedPrice).GreaterThan(0)
            .LessThan(command => command.OriginalPrice)
            .WithMessage("DiscountedPrice must be greater than 0 and less than OriginalPrice.");
        RuleFor(command => command.MaxQuantity).GreaterThan(0)
            .WithMessage("MaxQuantity must be greater than 0.");
        RuleFor(command => command.ExpirationDate).GreaterThan(DateTime.UtcNow)
            .WithMessage("ExpirationDate must be a future date.");
        RuleFor(command => command.CategoryIds).NotEmpty()
            .WithMessage("At least one category must be selected.");
    }
}