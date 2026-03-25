using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Commands.AdminCommands;

public sealed record CreateCategoryCommand(string Name, string? Description) : IRequest<int>, IRequireRole
{
    public RoleEnum RoleRequired => RoleEnum.Administrator;
}