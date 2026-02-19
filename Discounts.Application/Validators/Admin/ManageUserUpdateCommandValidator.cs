using Discounts.Application.Commands;
using FluentValidation;

namespace Discounts.Application.Validators.Admin;

public class ManageUserUpdateCommandValidator : AbstractValidator<ManageUserUpdateCommand>
{
    public ManageUserUpdateCommandValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0).WithMessage("UserId must be a positive integer");

        When(x => !string.IsNullOrEmpty(x.Email), () =>
        {
            RuleFor(x => x.Email).EmailAddress().WithMessage("Email must be valid");
        });

        When(x => !string.IsNullOrEmpty(x.UserName), () =>
        {
            RuleFor(x => x.UserName).MinimumLength(3).MaximumLength(20)
                .WithMessage("Username must be between 3 and 20 characters");
        });

        When(x => !string.IsNullOrEmpty(x.Password) || !string.IsNullOrEmpty(x.ConfirmPassword), () =>
        {
            RuleFor(x => x.Password).MinimumLength(6).WithMessage("Password must be at least 6 characters long");
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password).WithMessage("Confirm Password must match Password");
        });

        When(x => x.RoleId.HasValue, () =>
        {
            RuleFor(x => x.RoleId).GreaterThan(0).WithMessage("RoleId must be a positive integer");
        });
    }
}

