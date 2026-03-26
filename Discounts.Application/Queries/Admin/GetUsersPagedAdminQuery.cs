using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Application.Models;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Queries.Admin;

public sealed record GetUsersPagedAdminQuery(int pageNumber = 1, int pageSize = 8) 
    : IRequest<PagedResult<User>>, IRequireRole, IPagedQuery
{
    public RoleEnum RoleRequired =>  RoleEnum.Administrator;
    public int PageNumber => pageNumber;
    public int PageSize =>  pageSize;
}