using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Commands.CustomerCommands.Reservations;

public sealed record CancelReservationCommand(int Id) : IRequest, IRequireRole, IEntityByIdCommand, ITransactionalCommand
{
    public RoleEnum RoleRequired => RoleEnum.Customer;
}

