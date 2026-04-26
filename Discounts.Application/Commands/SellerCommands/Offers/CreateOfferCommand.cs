using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Commands.SellerCommands.Offers;

public sealed record CreateOfferCommand(
    string Title,
    string? Description,
    decimal OriginalPrice,
    decimal DiscountedPrice,
    int MaxQuantity,
    DateTime ExpirationDate,
    IReadOnlyCollection<int> CategoryIds) : IRequest<int>, IRequireRole
{
    public RoleEnum RoleRequired => RoleEnum.Seller;
}

