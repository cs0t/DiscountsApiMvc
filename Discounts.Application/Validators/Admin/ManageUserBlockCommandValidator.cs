using Discounts.Application.Commands.Admin;
using FluentValidation;

namespace Discounts.Application.Validators.Admin;

public class ManageUserBlockCommandValidator  :AbstractValidator<ManageUserBlockCommand>
{
    public ManageUserBlockCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
    }
}