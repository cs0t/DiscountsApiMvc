using System.Security.Claims;
using Discounts.Application.Interfaces.CustomerModuleContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Models;
using Discounts.Application.Queries;
using Discounts.MVC.ViewModels.Customer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Discounts.MVC.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize(Roles = "Customer")]
public class OffersController : Controller
{
    private readonly ICustomerService _customerService;
    private readonly ICategoryRepository _categoryRepository;

    public OffersController(
        ICustomerService customerService,
        ICategoryRepository categoryRepository)
    {
        _customerService = customerService;
        _categoryRepository = categoryRepository;
    }

    private int GetCustomerId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index(OfferBrowseViewModel filter, CancellationToken ct)
    {
        var query = new OfferListQuery
        {
            PriceStart = filter.PriceStart,
            PriceEnd = filter.PriceEnd,
            CategoryIds = filter.CategoryIds,
            PageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber,
            PageSize = filter.PageSize < 1 ? 8 : filter.PageSize
        };

        var pagedOffers = await _customerService.GetApprovedOffersAsync(query, GetCustomerId(), ct);

        var offerDtos = pagedOffers.Items.Select(o => new OfferDetailsDto
        {
            Id = o.Id,
            Title = o.Title,
            Description = o.Description,
            OriginalPrice = o.OriginalPrice,
            DiscountedPrice = o.DiscountedPrice,
            MaxQuantity = o.MaxQuantity,
            RemainingQuantity = o.RemainingQuantity,
            Status = o.Status.Name,
            CreatedAt = o.CreatedAt,
            ExpirationDate = o.ExpirationDate,
            SellerId = o.SellerId,
            SellerUserName = o.Seller.UserName,
            RowVersion = Convert.ToBase64String(o.RowVersion),
            Categories = o.Categories.Select(c => c.Name).ToList()
        }).ToList();

        var pagedDtos = new PagedResult<OfferDetailsDto>(
            offerDtos, pagedOffers.TotalCount, pagedOffers.PageNumber, pagedOffers.PageSize);

        var categories = await _categoryRepository.GetAllCategoriesAsync(ct);
        filter.AvailableCategories = categories
            .Select(c => new SelectListItem(c.Name, c.Id.ToString())).ToList();

        ViewBag.Filter = filter;
        return View(pagedDtos);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        try
        {
            var state = await _customerService.GetOfferStateForCustomerAsync(id, GetCustomerId(), ct);

            var dto = new CustomerOfferStateDto
            {
                Offer = new OfferDetailsDto
                {
                    Id = state.Offer.Id,
                    Title = state.Offer.Title,
                    Description = state.Offer.Description,
                    OriginalPrice = state.Offer.OriginalPrice,
                    DiscountedPrice = state.Offer.DiscountedPrice,
                    MaxQuantity = state.Offer.MaxQuantity,
                    RemainingQuantity = state.Offer.RemainingQuantity,
                    Status = state.Offer.Status.Name,
                    CreatedAt = state.Offer.CreatedAt,
                    ExpirationDate = state.Offer.ExpirationDate,
                    ApprovedAt = state.Offer.ApprovedAt,
                    RejectedAt = state.Offer.RejectedAt,
                    RejectionReason = state.Offer.RejectionReason,
                    EditableUntil = state.Offer.EditableUntil,
                    SellerId = state.Offer.SellerId,
                    SellerUserName = state.Offer.Seller.UserName,
                    RowVersion = Convert.ToBase64String(state.Offer.RowVersion),
                    Categories = state.Offer.Categories.Select(c => c.Name).ToList()
                },
                HasActiveReservation = state.HasActiveReservation,
                ReservationValidUntil = state.ReservationValidUntil,
                CanReserve = state.CanReserve,
                CanPurchase = state.CanPurchase
            };

            return View(dto);
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reserve(int offerId, string rowVersion, CancellationToken ct)
    {
        try
        {
            var rv = Convert.FromBase64String(rowVersion);
            await _customerService.CreateReservationAsync(GetCustomerId(), offerId, rv, ct);
            TempData["Success"] = "Reservation created successfully! You can now purchase the coupon from your reservations.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Details), new { id = offerId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Purchase(int offerId, CancellationToken ct)
    {
        try
        {
            await _customerService.PurchaseCouponAsync(GetCustomerId(), offerId, ct);
            TempData["Success"] = "Coupon purchased successfully!";
            return RedirectToAction("Index", "Coupons");
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction(nameof(Details), new { id = offerId });
        }
    }
}
