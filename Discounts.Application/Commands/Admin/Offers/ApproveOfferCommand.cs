using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Commands.Admin.Offers;

public sealed record ApproveOfferCommand(int id) : IRequest, IRequireRole, IEntityByIdCommand
{
    public RoleEnum RoleRequired => RoleEnum.Administrator;
    public int Id => id;
}