using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Application.Models;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Queries.Admin;

public class GetCategoriesPagedAdminQuery(int pageNumber = 1, int pageSize = 8) 
    : IRequest<PagedResult<Category>>, IPagedQuery, IRequireRole
{
    public int PageNumber =>  pageNumber;
    public int PageSize => pageSize;
    public RoleEnum RoleRequired => RoleEnum.Administrator;
}