using System.Security.Claims;
using Discounts.Application.Interfaces.CustomerModuleContracts;
using Discounts.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.MVC.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize(Roles = "Customer")]
public class ReservationsController : Controller
{
    private readonly ICustomerService _customerService;

    public ReservationsController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    private int GetCustomerId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index(int page = 1, int pageSize = 8, CancellationToken ct = default)
    {
        var pagedReservations = await _customerService.GetCustomerReservationsAsync(GetCustomerId(), page, pageSize, ct);

        var reservationDtos = pagedReservations.Items.Select(r => new ReservationDto
        {
            Id = r.Id,
            OfferId = r.OfferId,
            OfferTitle = r.Offer.Title,
            ReservedAt = r.ReservedAt,
            ValidUntil = r.ValidUntil,
            IsActive = r.IsActive
        }).ToList();

        var pagedDtos = new PagedResult<ReservationDto>(
            reservationDtos, pagedReservations.TotalCount, pagedReservations.PageNumber, pagedReservations.PageSize);

        return View(pagedDtos);
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
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int reservationId, CancellationToken ct)
    {
        try
        {
            await _customerService.CancelReservationAsync(reservationId, GetCustomerId(), ct);
            TempData["Success"] = "Reservation cancelled successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
}


