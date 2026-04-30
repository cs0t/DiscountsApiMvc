using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Application.Models;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Queries.Customer;

public sealed record GetCustomerReservationsQuery(int PageNumber = 1, int PageSize = 8)
    : IRequest<PagedResult<Reservation>>, IRequireRole, IPagedQuery
{
    public RoleEnum RoleRequired => RoleEnum.Customer;
}

