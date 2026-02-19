using Discounts.Application.Commands;
using FluentValidation;

namespace Discounts.Application.Validators.Admin;

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id must be a positive integer");
        RuleFor(x => x.NewName).NotEmpty().WithMessage("NewName is required");
        RuleFor(x=> x.NewDescription).MinimumLength(5).When(x=> !string.IsNullOrEmpty(x.NewDescription))
            .WithMessage("NewDescription must be at least 5 characters long !");
    }
}

