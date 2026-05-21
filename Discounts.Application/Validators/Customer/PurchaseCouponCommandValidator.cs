using Discounts.Application.Commands.CustomerCommands.Coupons;
using FluentValidation;

namespace Discounts.Application.Validators.Customer;

public class PurchaseCouponCommandValidator : AbstractValidator<PurchaseCouponCommand>
{
    public PurchaseCouponCommandValidator()
    {
        RuleFor(x => x.OfferId)
            .GreaterThan(0).WithMessage("OfferId must be a positive integer.");
    }
}

