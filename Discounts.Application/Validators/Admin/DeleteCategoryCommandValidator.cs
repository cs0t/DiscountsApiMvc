using Discounts.Application.Commands.AdminCommands;
using FluentValidation;

namespace Discounts.Application.Validators.Admin;

public class DeleteCategoryCommandValidator : AbstractValidator<DeleteCategoryCommand>
{
    public DeleteCategoryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().GreaterThan(0).WithMessage("Category id must be greater than 0 !");
    }
}