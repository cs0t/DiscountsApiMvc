using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Models;
using Discounts.Application.Queries.Admin;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Handlers.Admin.Users;

public class GetUsersPagedAdminHandler(IUserRepository repository) 
    : IRequestHandler<GetUsersPagedAdminQuery,PagedResult<User>>
{
    public Task<PagedResult<User>> Handle(GetUsersPagedAdminQuery query, CancellationToken ct =  default)
    {
        return  repository.GetPagedAsync(query.PageNumber, query.PageSize, ct);
    }
}