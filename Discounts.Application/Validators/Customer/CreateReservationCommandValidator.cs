using Discounts.Application.Commands.CustomerCommands.Reservations;
using FluentValidation;

namespace Discounts.Application.Validators.Customer;

public class CreateReservationCommandValidator : AbstractValidator<CreateReservationCommand>
{
    public CreateReservationCommandValidator()
    {
        RuleFor(x => x.OfferId)
            .GreaterThan(0).WithMessage("OfferId must be a positive integer.");
        RuleFor(x => x.RowVersion)
            .NotEmpty().WithMessage("RowVersion is required for concurrency control.");
    }
}

