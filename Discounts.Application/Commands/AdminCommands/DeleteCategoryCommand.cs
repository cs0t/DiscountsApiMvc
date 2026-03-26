using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Commands.AdminCommands;

public sealed record DeleteCategoryCommand(int Id) : IRequest,IRequireRole
{
    public RoleEnum RoleRequired => RoleEnum.Administrator;
}
