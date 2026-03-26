using Discounts.Application.Queries;
using FluentValidation;

namespace Discounts.Application.Validators.Common;

public class PagedRequestValidator<T> : AbstractValidator<T> where T:IPagedQuery
{
    public PagedRequestValidator()
    {
        RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(50);
    }
}