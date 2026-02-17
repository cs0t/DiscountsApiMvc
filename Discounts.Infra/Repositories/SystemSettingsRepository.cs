using Discounts.Domain.Entities;
using Discounts.Infra.Persistence;
using Discounts.Application.Interfaces.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infra.Repositories;

public class SystemSettingsRepository : Repository<SystemSettings>, ISystemSettingsRepository
{
    public SystemSettingsRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public Task<SystemSettings?> GetSettingByKeyAsync(string key,CancellationToken ct = default)
    {
        return _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == key, ct);
    }

    public async Task UpdateSettingAsync(string key, string newValue, CancellationToken ct = default)
    {
        var setting = await _context.SystemSettings
            .FirstOrDefaultAsync(s =>s.Key == key,ct);

        if (setting is not null)
        {
            setting.Key = newValue;
            Update(setting);
            await _context.SaveChangesAsync(ct);
        }
    }
}