using Discounts.Application.Commands;

namespace Discounts.Application.Interfaces.AdminModuleContracts;

public interface IUserManagementService
{
    public Task<int> CreateUserAsync(int adminId,ManageUserCreationCommand createUserCommand,CancellationToken ct = default);
    public Task<int> UpdateUserAsync(int adminId,ManageUserUpdateCommand updateUserCommand,CancellationToken ct = default);
    public Task BlockUserAsync(int adminId,int userId,CancellationToken ct = default);
}