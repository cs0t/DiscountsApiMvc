using Discounts.Application.Commands;
using FluentValidation;

namespace Discounts.Application.Validators.Admin;

public class ManageUserCreationCommandValidator : AbstractValidator<ManageUserCreationCommand>
{
    public ManageUserCreationCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Email is required");
        RuleFor(x => x.UserName).NotEmpty().MinimumLength(3).MaximumLength(20)
            .WithMessage("Username must be between 3 and 20 characters");
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6)
            .WithMessage("Password must be at least 6 characters long");
        RuleFor(x => x.ConfirmPassword).NotEmpty().Equal(x => x.Password)
            .WithMessage("Confirm Password must match Password");
        RuleFor(x => x.RoleId).GreaterThan(0).WithMessage("RoleId must be a positive integer");
    }
}

