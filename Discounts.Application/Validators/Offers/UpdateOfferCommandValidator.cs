using Discounts.Application.Commands;
using FluentValidation;

namespace Discounts.Application.Validators.Offers;

public class UpdateOfferCommandValidator:AbstractValidator<UpdateOfferCommand>
{
    public UpdateOfferCommandValidator()
    {
        RuleFor(x => x.OfferId).GreaterThan(0);
        RuleFor(command => command.Title).NotEmpty().MaximumLength(200);
        RuleFor(command => command.Description).MaximumLength(1000);
        RuleFor(command => command.OriginalPrice).GreaterThan(0);
        RuleFor(command => command.DiscountedPrice).GreaterThan(0).LessThan(command => command.OriginalPrice);
        RuleFor(command => command.MaxQuantity).GreaterThan(0);
        RuleFor(command => command.RemainingQuantity).GreaterThanOrEqualTo(0).LessThanOrEqualTo(command => command.MaxQuantity);
        RuleFor(command => command.ExpirationDate).GreaterThan(DateTime.UtcNow);
        RuleFor(command => command.CategoryIds).NotEmpty();
    }
}