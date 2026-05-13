using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Commands.SellerCommands.Offers;

public sealed record CreateOfferCommand(
    string Title,
    string? Description,
    decimal OriginalPrice,
    decimal DiscountedPrice,
    int MaxQuantity,
    DateTime ExpirationDate,
    IReadOnlyCollection<int> CategoryIds) : IRequireRole, IRequest<Offer>
{
    public RoleEnum RoleRequired => RoleEnum.Seller;
}

