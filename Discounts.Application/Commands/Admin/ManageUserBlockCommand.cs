using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Commands.Admin;

public sealed record ManageUserBlockCommand(int Id) : IRequest, IRequireRole
{
    public RoleEnum RoleRequired =>  RoleEnum.Administrator;
}
