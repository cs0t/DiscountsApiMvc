using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Application.Models;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Queries.Admin;

public sealed record GetOffersPagedAdminQuery(int PageNumber = 1, int PageSize = 8) 
    : IRequest<PagedResult<Offer>>, IRequireRole, IPagedQuery
{
    public RoleEnum RoleRequired => RoleEnum.Administrator;
}