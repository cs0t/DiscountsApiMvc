using Discounts.Application.Exceptions.SystemSettingsExceptions;
using Discounts.Application.Exceptions.UserExceptions;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Interfaces.SystemSettingsContracts;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;

namespace Discounts.Application.Services.SystemSettingsServices;

public class SystemSettingsService : ISystemSettingsService
{
    private readonly ISystemSettingsRepository _systemSettingsRepository;
    private readonly IUserRepository _userRepository;
    
    public SystemSettingsService(
        ISystemSettingsRepository systemSettingsRepository,
        IUserRepository userRepository)
    {
        _systemSettingsRepository = systemSettingsRepository;
        _userRepository = userRepository;
    }
    
    public async Task<T> GetSettingValueByKeyAsync<T>(string key, CancellationToken ct = default)
    {
        var setting = await _systemSettingsRepository.GetSettingByKeyAsync(key, ct); 
        if(setting is null)
            throw new SettingNotFoundException($"Setting with key '{key}' not found.");
        
        var settingValue = setting.SettingValue;
        
        return typeof(T) == typeof(string)  ?  (T)(object)settingValue : (T)Convert.ChangeType(settingValue,typeof(T));
    }

    public async Task UpdateSettingAsync(int userid, string key, string value, CancellationToken ct = default)
    {
        var user  = await _userRepository.GetWithRolesAsync(userid, ct);
        if (user is null || user.RoleId != (int)RoleEnum.Administrator)
        {
            throw new UnauthorizedException("User is not authorized to update system settings !");
        }
        await _systemSettingsRepository.UpdateSettingAsync(key, value, ct);
    }

    public async Task<int> CreateSettingAsync(int userid, string key, string value, CancellationToken ct = default)
    {
        var user  = await _userRepository.GetWithRolesAsync(userid, ct);
        if (user is null || user.RoleId != (int)RoleEnum.Administrator)
        {
            throw new UnauthorizedException("User is not authorized to update system settings !");
        }
        var newSetting = new SystemSettings
        {
            Key = key,
            SettingValue = value
        };
        await _systemSettingsRepository.Add(newSetting, ct);
        return newSetting.Id;
    }
}