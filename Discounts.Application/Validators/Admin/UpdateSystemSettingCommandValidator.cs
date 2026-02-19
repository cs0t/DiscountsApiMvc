using Discounts.Application.Commands;
using FluentValidation;

namespace Discounts.Application.Validators.Admin;

public class UpdateSystemSettingCommandValidator : AbstractValidator<UpdateSystemSettingCommand>
{
    public UpdateSystemSettingCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id must be a positive integer");
        RuleFor(x => x.NewSettingValue).NotEmpty().WithMessage("NewSettingValue is required");
    }
}

