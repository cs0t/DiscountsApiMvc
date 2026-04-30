using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Application.Models;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Queries.Customer;

public sealed record GetCustomerCouponsQuery(int PageNumber = 1, int PageSize = 8)
    : IRequest<PagedResult<Coupon>>, IRequireRole, IPagedQuery
{
    public RoleEnum RoleRequired => RoleEnum.Customer;
}

