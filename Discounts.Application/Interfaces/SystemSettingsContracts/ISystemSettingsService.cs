using Discounts.Application.Interfaces.RepositoryContracts;

namespace Discounts.Application.Interfaces.SystemSettingsContracts;

public interface ISystemSettingsService
{
    Task<T> GetSettingValueByKeyAsync<T>(string key, CancellationToken ct = default);
    Task UpdateSettingAsync(int userId, string key, string value, CancellationToken ct = default);
    Task<int> CreateSettingAsync(int userId,string key, string value, CancellationToken ct = default);
}