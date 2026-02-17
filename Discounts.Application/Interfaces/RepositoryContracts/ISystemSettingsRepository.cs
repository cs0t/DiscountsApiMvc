using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.RepositoryContracts;

public interface ISystemSettingsRepository : IRepository<SystemSettings>
{
     public Task<SystemSettings?> GetSettingByKeyAsync(string key,CancellationToken ct = default);
     public Task UpdateSettingAsync(string key, string newValue, CancellationToken ct = default);
}