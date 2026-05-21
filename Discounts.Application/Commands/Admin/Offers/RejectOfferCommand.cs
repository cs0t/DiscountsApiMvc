using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Commands.Admin.Offers;

public sealed record RejectOfferCommand(int OfferId, string Reason) : IRequest, IRequireRole
{
    public RoleEnum RoleRequired => RoleEnum.Administrator;
}