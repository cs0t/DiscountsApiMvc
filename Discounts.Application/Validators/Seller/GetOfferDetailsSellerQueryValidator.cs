using Discounts.Application.Queries.Seller;
using FluentValidation;

namespace Discounts.Application.Validators.Seller;

public class GetOfferDetailsSellerQueryValidator : AbstractValidator<GetOfferDetailsSellerQuery>
{
    public GetOfferDetailsSellerQueryValidator()
    {
        RuleFor(x => x.OfferId)
            .GreaterThan(0).WithMessage("OfferId must be greater than 0.");
    }
}

