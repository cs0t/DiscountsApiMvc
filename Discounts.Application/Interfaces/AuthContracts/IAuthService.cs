using Discounts.Application.Commands;
using Discounts.Application.Models;

namespace Discounts.Application.Interfaces.AuthContracts;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginCommand loginCommand, CancellationToken ct = default);
    Task<LoginResponse> RegisterAsync(RegisterCommand registerCommand, CancellationToken ct = default);
    Task<UserDto> GetCurrentUserAsync(int userId, CancellationToken ct = default);
}