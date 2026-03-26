using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Models;
using Discounts.Application.Queries.Admin;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Handlers.Admin;

public class GetCategoriesPagedForAdminHandler(ICategoryRepository categoryRepository) 
    : IRequestHandler<GetCategoriesPagedAdminQuery,PagedResult<Category>>
{
    public Task<PagedResult<Category>> Handle(GetCategoriesPagedAdminQuery query, CancellationToken ct = default)
    {
        return categoryRepository.GetPagedAsync(query.PageNumber, query.PageSize, ct);
    }
}