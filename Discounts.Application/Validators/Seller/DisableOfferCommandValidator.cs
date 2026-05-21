using Discounts.Application.Commands.SellerCommands.Offers;
using Discounts.Application.Validators.Common;
using FluentValidation;

namespace Discounts.Application.Validators.Seller;

public class DisableOfferCommandValidator : EntityByIdCommandValidator<DisableOfferCommand>;

