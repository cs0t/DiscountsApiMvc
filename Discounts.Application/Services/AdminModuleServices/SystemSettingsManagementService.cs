using Discounts.Application.Commands;
using Discounts.Application.Exceptions;
using Discounts.Application.Exceptions.SystemSettingsExceptions;
using Discounts.Application.Exceptions.UserExceptions;
using Discounts.Application.Interfaces.AdminModuleContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Models;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;

namespace Discounts.Application.Services.AdminModuleServices;

public class SystemSettingsManagementService : ISystemSettingsManagementService
{
    
    private readonly IUserRepository _userRepository;
    private readonly ISystemSettingsRepository _settingRepository;

    public SystemSettingsManagementService(IUserRepository userRepository, ISystemSettingsRepository settingRepository)
    {
        _userRepository = userRepository;
        _settingRepository = settingRepository;
    }
    
    public async Task<int> CreateSystemSettingAsync(int adminId, CreateSystemSettingCommand command, CancellationToken ct = default)
    {
        var admin = await _userRepository.GetWithRolesAsync(adminId, ct);
        
        if(admin is null)
            throw new UserNotFoundException("Admin not found");

        if (admin.RoleId != (int)RoleEnum.Administrator)
            throw new ForbiddenException("This user doesn`t have admin permissions !");

        if (await _settingRepository.ExistsAsync(c=>c.Key == command.Key, ct))
            throw new ApplicationException("This Setting already exists !");

        var setting = new SystemSettings
        {
            Key = command.Key,
            SettingValue = command.SettingValue,
        };
        await _settingRepository.Add(setting, ct);
        await _settingRepository.SaveChangesAsync(ct);
        return setting.Id;
    }

    public async Task UpdateSystemSettingAsync(int adminId, UpdateSystemSettingCommand command, CancellationToken ct = default)
    {
        var admin = await _userRepository.GetWithRolesAsync(adminId, ct);
        
        if(admin is null)
            throw new UserNotFoundException("Admin not found");

        if (admin.RoleId != (int)RoleEnum.Administrator)
            throw new ForbiddenException("This user doesn`t have admin permissions !");
        
        var setting = await _settingRepository.GetById(command.Id, ct);
        
        if (setting is null)
            throw new ApplicationException("Setting not found !");

        setting.SettingValue = command.NewSettingValue;
        await _settingRepository.SaveChangesAsync(ct);
    }
    
    public async Task<PagedResult<SystemSettings>> GetSettingsPagedForAdminAsync(int adminId,int pageNumber = 1, int pageSize = 8,
        CancellationToken ct = default)
    {
        var admin = await _userRepository.GetWithRolesAsync(adminId, ct);
            
        if(admin is null)
            throw new UserNotFoundException("Admin not found");
            
        if (admin.RoleId != (int)RoleEnum.Administrator)
            throw new ForbiddenException("This user doesn`t have admin permissions !");
        return await _settingRepository.GetPagedAsync(pageNumber, pageSize, ct);
    }
}