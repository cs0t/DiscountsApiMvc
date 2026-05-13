using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Queries.Seller;

public sealed record GetOfferDetailsSellerQuery(int OfferId) : IRequest<Offer>, IRequireRole
{
    public RoleEnum RoleRequired => RoleEnum.Seller;
}

