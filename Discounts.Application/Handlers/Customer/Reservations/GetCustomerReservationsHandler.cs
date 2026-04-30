using Discounts.Application.Interfaces.AuthContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Models;
using Discounts.Application.Queries.Customer;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Handlers.Customer.Reservations;

public class GetCustomerReservationsHandler(
    ICurrentUserService currentUserService,
    IReservationRepository reservationRepository)
    : IRequestHandler<GetCustomerReservationsQuery, PagedResult<Reservation>>
{
    public Task<PagedResult<Reservation>> Handle(GetCustomerReservationsQuery request, CancellationToken ct = default)
    {
        var customerId = currentUserService.UserId;
        return reservationRepository.GetActiveReservationsByUserIdAsync(customerId, request.PageNumber, request.PageSize, ct);
    }
}

