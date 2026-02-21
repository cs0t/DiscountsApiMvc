using System.Security.Claims;
using Discounts.Application.Interfaces.SellerModuleServiceContracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.MVC.Areas.Seller.Controllers;

[Area("Seller")]
[Authorize(Roles = "Seller")]
public class DashboardController : Controller
{
    private readonly ISellerDashboardService _dashboardService;

    public DashboardController(ISellerDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var stats = await _dashboardService.GetDashboardStatsAsync(userId, ct);
        return View(stats);
    }
}

