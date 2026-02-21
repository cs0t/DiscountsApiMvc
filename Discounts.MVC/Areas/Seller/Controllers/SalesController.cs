using System.Security.Claims;
using Discounts.Application.Interfaces.SellerModuleServiceContracts;
using Discounts.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.MVC.Areas.Seller.Controllers;

[Area("Seller")]
[Authorize(Roles = "Seller")]
public class SalesController : Controller
{
    private readonly ISellerSalesService _salesService;

    public SalesController(ISellerSalesService salesService)
    {
        _salesService = salesService;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var coupons = await _salesService.GetSalesHistoryAsync(userId, ct);

        var salesDtos = coupons.Select(c => new SellerSaleDto
        {
            CouponCode = c.Code,
            CustomerUsername = c.Customer?.UserName ?? "N/A",
            PurchasedAt = c.PurchasedAt,
            CouponStatus = c.Status?.Name ?? "N/A"
        }).ToList();

        return View(salesDtos);
    }
}

