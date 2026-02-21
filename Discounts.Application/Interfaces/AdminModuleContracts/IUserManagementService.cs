using Discounts.Application.Commands;
using Discounts.Application.Models;
using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.AdminModuleContracts;

public interface IUserManagementService
{
    public Task<int> CreateUserAsync(int adminId,ManageUserCreationCommand createUserCommand,CancellationToken ct = default);
    public Task<int> UpdateUserAsync(int adminId,ManageUserUpdateCommand updateUserCommand,CancellationToken ct = default);
    public Task BlockUserAsync(int adminId,int userId,CancellationToken ct = default);

    Task<PagedResult<User>> GetSettingsPagedForAdminAsync(int adminId, int pageNumber = 1, int pageSize = 8,
        CancellationToken ct = default);
}