using Discounts.Application.Commands;
using FluentValidation;

namespace Discounts.Application.Validators.Common;

public class EntityByIdCommandValidator<T> 
    : AbstractValidator<T> where T : IEntityByIdCommand 
{
    public  EntityByIdCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id must  be greater than 0 !");
    }
}