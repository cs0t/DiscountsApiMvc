using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Commands.SellerCommands.Offers;

public sealed record DisableOfferCommand(int Id) : IRequest, IRequireRole, IEntityByIdCommand
{
    public RoleEnum RoleRequired => RoleEnum.Seller;
}

