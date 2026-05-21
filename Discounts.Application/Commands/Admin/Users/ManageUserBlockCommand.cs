using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Commands.Admin.Users;

public sealed record ManageUserBlockCommand(int Id) : IRequest, IRequireRole, IEntityByIdCommand
{
    public RoleEnum RoleRequired =>  RoleEnum.Administrator;
}
