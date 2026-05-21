using Discounts.Domain.Constants;

namespace Discounts.Application.Interfaces.AuthContracts;

public interface ICurrentUserService
{
    int UserId { get; }
    bool IsAuthenticated { get; }
    string? Role { get; }
}