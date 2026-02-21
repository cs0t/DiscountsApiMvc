using System.Security.Claims;
using Discounts.Application.Interfaces.CustomerModuleContracts;
using Discounts.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.MVC.Areas.Customer.Controllers;

[Area("Customer")]
[Authorize(Roles = "Customer")]
public class CouponsController : Controller
{
    private readonly ICustomerService _customerService;

    public CouponsController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    private int GetCustomerId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index(int page = 1, int pageSize = 8, CancellationToken ct = default)
    {
        var pagedCoupons = await _customerService.GetCustomerCouponsAsync(GetCustomerId(), page, pageSize, ct);

        var couponDtos = pagedCoupons.Items.Select(c => new CouponDetailsDto
        {
            Code = c.Code,
            OfferTitle = c.Offer?.Title ?? "N/A",
            OfferDescription = c.Offer?.Description ?? "",
            OriginalPrice = c.Offer?.OriginalPrice ?? 0,
            DiscountPrice = c.Offer?.DiscountedPrice ?? 0,
            CouponStatus = c.Status?.Name ?? "N/A",
            PurchasedAt = c.PurchasedAt,
            ExpirationDate = c.ExpirationDate
        }).ToList();

        var pagedDtos = new PagedResult<CouponDetailsDto>(
            couponDtos, pagedCoupons.TotalCount, pagedCoupons.PageNumber, pagedCoupons.PageSize);

        ViewBag.CurrentPage = page;
        return View(pagedDtos);
    }
}

