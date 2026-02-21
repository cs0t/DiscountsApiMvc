using Discounts.Application.Commands;
using Discounts.Application.Models;
using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.AdminModuleContracts;

public interface ICategoryManagementService
{
    public Task<int> CreateCategoryAsync(int adminId, CreateCategoryCommand command, CancellationToken ct = default);
    public Task UpdateCategoryAsync(int adminId, UpdateCategoryCommand command, CancellationToken ct = default);

    Task<PagedResult<Category>> GetCategoriesPagedForAdminAsync(int adminId, int pageNumber = 1, int pageSize = 8,
        CancellationToken ct = default);

    Task DeleteCategoryAsync(int adminId, int categoryId, CancellationToken ct = default);
}