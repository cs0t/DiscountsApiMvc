using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Application.Models;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Queries.Seller;

public sealed record GetSellerDashboardStatsQuery : IRequest<SellerDashboardStats>, IRequireRole
{
    public RoleEnum RoleRequired => RoleEnum.Seller;
}

