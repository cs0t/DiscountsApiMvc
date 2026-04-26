using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Commands.SellerCommands.Offers;

public sealed record UpdateOfferCommand(
    int OfferId,
    string Title,
    string? Description,
    decimal OriginalPrice,
    decimal DiscountedPrice,
    int MaxQuantity,
    int RemainingQuantity,
    DateTime ExpirationDate,
    IReadOnlyCollection<int> CategoryIds) : IRequest<int>, IRequireRole
{
    public RoleEnum RoleRequired => RoleEnum.Seller;
}

