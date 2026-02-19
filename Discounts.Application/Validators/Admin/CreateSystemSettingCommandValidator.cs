using Discounts.Application.Commands;
using FluentValidation;

namespace Discounts.Application.Validators.Admin;

public class CreateSystemSettingCommandValidator : AbstractValidator<CreateSystemSettingCommand>
{
    public CreateSystemSettingCommandValidator()
    {
        RuleFor(x => x.Key).NotEmpty().WithMessage("Key is required");
        RuleFor(x => x.SettingValue).NotEmpty().WithMessage("Setting value is required");
    }
}

