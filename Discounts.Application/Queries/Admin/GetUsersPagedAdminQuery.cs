using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Application.Models;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Queries.Admin;

public sealed record GetUsersPagedAdminQuery(int PageNumber = 1, int PageSize = 8) 
    : IRequest<PagedResult<User>>, IRequireRole, IPagedQuery
{
    public RoleEnum RoleRequired =>  RoleEnum.Administrator;
}