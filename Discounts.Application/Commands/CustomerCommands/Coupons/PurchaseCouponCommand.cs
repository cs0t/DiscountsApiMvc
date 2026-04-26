using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Commands.CustomerCommands.Coupons;

public sealed record PurchaseCouponCommand(int OfferId) : IRequest, IRequireRole, ITransactionalCommand
{
    public RoleEnum RoleRequired => RoleEnum.Customer;
}

