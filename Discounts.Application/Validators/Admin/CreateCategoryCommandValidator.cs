using Discounts.Application.Commands;
using FluentValidation;

namespace Discounts.Application.Validators.Admin;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage("NewName is required");
        RuleFor(x=> x.Description).MinimumLength(5).When(x=> !string.IsNullOrEmpty(x.Description))
            .WithMessage("NewDescription must be at least 5 characters long !");
    }
}