using Discounts.Application.Commands;

namespace Discounts.Application.Interfaces.AdminModuleContracts;

public interface ICategoryManagementService
{
    public Task<int> CreateCategoryAsync(int adminId, CreateCategoryCommand command, CancellationToken ct = default);
    public Task UpdateCategoryAsync(int adminId, UpdateCategoryCommand command, CancellationToken ct = default);
}