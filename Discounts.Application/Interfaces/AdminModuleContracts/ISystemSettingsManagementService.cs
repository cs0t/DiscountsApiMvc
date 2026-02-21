using Discounts.Application.Commands;
using Discounts.Application.Models;
using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.AdminModuleContracts;

public interface ISystemSettingsManagementService
{
    public Task<int> CreateSystemSettingAsync(int adminId,CreateSystemSettingCommand command, CancellationToken ct = default);
    public Task UpdateSystemSettingAsync(int adminId,UpdateSystemSettingCommand command, CancellationToken ct = default);

    Task<PagedResult<SystemSettings>> GetSettingsPagedForAdminAsync(int adminId, int pageNumber = 1, int pageSize = 8,
        CancellationToken ct = default);
    
}