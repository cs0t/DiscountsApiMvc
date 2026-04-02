using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Application.Models;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Queries.Admin;

public sealed record GetCategoriesPagedAdminQuery(int PageNumber = 1, int PageSize = 8) 
    : IRequest<PagedResult<Category>>, IPagedQuery, IRequireRole
{
    public RoleEnum RoleRequired => RoleEnum.Administrator;
}