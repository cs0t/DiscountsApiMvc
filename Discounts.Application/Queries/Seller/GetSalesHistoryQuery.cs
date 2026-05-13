using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Queries.Seller;

public sealed record GetSalesHistoryQuery : IRequest<List<Coupon>>, IRequireRole
{
    public RoleEnum RoleRequired => RoleEnum.Seller;
}

