using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Application.Models;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Queries.Customer;

public sealed record GetOfferStateForCustomerQuery(int OfferId)
    : IRequest<CustomerOfferState>, IRequireRole
{
    public RoleEnum RoleRequired => RoleEnum.Customer;
}

