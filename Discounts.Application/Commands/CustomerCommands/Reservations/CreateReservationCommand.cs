using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Commands.CustomerCommands.Reservations;

public sealed record CreateReservationCommand(int OfferId, byte[] RowVersion) : IRequest, IRequireRole, ITransactionalCommand
{
    public RoleEnum RoleRequired => RoleEnum.Customer;
}

