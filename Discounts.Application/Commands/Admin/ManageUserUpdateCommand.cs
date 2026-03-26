using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Commands.Admin;

public sealed record ManageUserUpdateCommand(
    int UserId,
    string? UserName,
    string? Email,
    string? Password,
    string? ConfirmPassword,
    int? RoleId) : IRequest<int>, IRequireRole
{
    public RoleEnum RoleRequired =>   RoleEnum.Administrator;
}

