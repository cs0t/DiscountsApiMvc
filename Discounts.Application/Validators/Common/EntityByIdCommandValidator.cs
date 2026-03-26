using Discounts.Application.Commands;
using FluentValidation;

namespace Discounts.Application.Validators.Common;

public class EntityByIdCommandValidator : AbstractValidator<IEntityByIdCommand>
{
    public  EntityByIdCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id is required !");
    }
}