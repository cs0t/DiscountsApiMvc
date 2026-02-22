using Discounts.Application.Commands;
using Discounts.Application.Exceptions;
using Discounts.Application.Exceptions.UserExceptions;
using Discounts.Application.Interfaces.AdminModuleContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Models;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;

namespace Discounts.Application.Services.AdminModuleServices;

public class UserManagementService : IUserManagementService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public UserManagementService(IUserRepository userRepository, IRoleRepository roleRepository)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }
    
    public async Task<int> CreateUserAsync(int adminId, ManageUserCreationCommand createUserCommand, CancellationToken ct = default)
    {
        var admin = await _userRepository.GetWithRolesAsync(adminId, ct);
        
        if(admin is null)
            throw new UserNotFoundException("Admin not found");

        if (admin.RoleId != (int)RoleEnum.Administrator)
            throw new ForbiddenException("This user doesn`t have admin permissions !");
        
        if(createUserCommand.Password != createUserCommand.ConfirmPassword)
            throw new ApplicationException("Passwords do not match !");

        if (await _userRepository
                .ExistsAsync(u =>
                    u.Email == createUserCommand.Email || u.UserName == createUserCommand.UserName, ct))
        {
            throw new ApplicationException("A user with the same email or username already exists !");
        }
        
        if (!await _roleRepository.ExistsAsync(r=>r.Id == createUserCommand.RoleId, ct))
        {
            throw new ApplicationException("Role not found !");
        }
        
        var user = new User
        {
            Email = createUserCommand.Email,
            UserName = createUserCommand.UserName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserCommand.Password),
            RoleId = createUserCommand.RoleId,
        };
        
        await _userRepository.Add(user, ct);
        await _userRepository.SaveChangesAsync(ct);
        return user.Id;
    }

    public async Task<int> UpdateUserAsync(int adminId, ManageUserUpdateCommand updateUserCommand, CancellationToken ct = default)
    {
        var admin = await _userRepository.GetWithRolesAsync(adminId, ct);
        if (admin is null)
            throw new UserNotFoundException("Admin not found");

        if (admin.RoleId != (int)RoleEnum.Administrator)
            throw new ForbiddenException("This user doesn`t have admin permissions !");

        var user = await _userRepository.GetById(updateUserCommand.UserId, ct);
        if (user is null)
            throw new UserNotFoundException("User not found");

        if (!string.IsNullOrEmpty(updateUserCommand.Password) || !string.IsNullOrEmpty(updateUserCommand.ConfirmPassword))
        {
            if (updateUserCommand.Password != updateUserCommand.ConfirmPassword)
                throw new ApplicationException("Passwords do not match !");
        }

        if (!string.IsNullOrEmpty(updateUserCommand.Email))
        {
            var emailExists = await _userRepository.ExistsAsync(u => u.Email == updateUserCommand.Email && u.Id != updateUserCommand.UserId, ct);
            if (emailExists)
                throw new ApplicationException("A user with the same email already exists !");
        }

        if (!string.IsNullOrEmpty(updateUserCommand.UserName))
        {
            var usernameExists = await _userRepository.ExistsAsync(u => u.UserName == updateUserCommand.UserName && u.Id != updateUserCommand.UserId, ct);
            if (usernameExists)
                throw new ApplicationException("A user with the same username already exists !");
        }

        if (updateUserCommand.RoleId.HasValue)
        {
            if (!await _roleRepository.ExistsAsync(r => r.Id == updateUserCommand.RoleId.Value, ct))
            {
                throw new ApplicationException("Role not found !");
            }
            user.RoleId = updateUserCommand.RoleId.Value;
        }

        if (!string.IsNullOrEmpty(updateUserCommand.UserName))
            user.UserName = updateUserCommand.UserName!;

        if (!string.IsNullOrEmpty(updateUserCommand.Email))
            user.Email = updateUserCommand.Email!;

        if (!string.IsNullOrEmpty(updateUserCommand.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(updateUserCommand.Password);

        await _userRepository.SaveChangesAsync(ct);
        return user.Id;
    }
    public async Task BlockUserAsync(int adminId, int userId, CancellationToken ct = default)
    {
        var admin = await _userRepository.GetWithRolesAsync(adminId, ct);
        if (admin is null)
            throw new UserNotFoundException("Admin not found");

        if (admin.RoleId != (int)RoleEnum.Administrator)
            throw new ForbiddenException("This user doesn`t have admin permissions !");

        var user = await _userRepository.GetWithRolesAsync(userId, ct);
        
        if (user is null)
            throw new UserNotFoundException("User not found");
        
        if(user.RoleId == (int)RoleEnum.Administrator)
            throw new ForbiddenException("You cannot block another administrator !");
        
        user.RoleId = (int)RoleEnum.Blocked;
        await _userRepository.SaveChangesAsync(ct);
    }
    
    public async Task<PagedResult<User>> GetSettingsPagedForAdminAsync(int adminId,int pageNumber = 1, int pageSize = 8,
        CancellationToken ct = default)
    {
        var admin = await _userRepository.GetWithRolesAsync(adminId, ct);
            
        if(admin is null)
            throw new UserNotFoundException("Admin not found");
            
        if (admin.RoleId != (int)RoleEnum.Administrator)
            throw new ForbiddenException("This user doesn`t have admin permissions !");
        return await _userRepository.GetPagedAsync(pageNumber, pageSize, ct);
    }
}