using Discounts.Application.Commands;
using FluentValidation;

namespace Discounts.Application.Validators.Admin;

public class RejectOfferCommandValidator : AbstractValidator<RejectOfferCommand>
{
    public RejectOfferCommandValidator()
    {
        RuleFor(x => x.OfferId)
            .GreaterThan(0).WithMessage("OfferId must be greater than 0.");
        RuleFor(x => x.Reason).NotEmpty().WithMessage("Reason is required.")
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters.");
    }
}