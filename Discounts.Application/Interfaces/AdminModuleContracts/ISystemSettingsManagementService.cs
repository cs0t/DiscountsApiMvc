using Discounts.Application.Commands;

namespace Discounts.Application.Interfaces.AdminModuleContracts;

public interface ISystemSettingsManagementService
{
    public Task<int> CreateSystemSettingAsync(int adminId,CreateSystemSettingCommand command, CancellationToken ct = default);
    public Task UpdateSystemSettingAsync(int adminId,UpdateSystemSettingCommand command, CancellationToken ct = default);
}