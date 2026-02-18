using System.Security.Claims;
using Discounts.API.Requests.SellerRequests;
using Discounts.Application.Commands;
using Discounts.Application.Interfaces.SellerModuleServiceContracts;
using Discounts.Application.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Seller")]
public class SellerController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ISellerDashboardService _dashboardService;
    private readonly IOfferManagementService _offerManagementService;
    private readonly ISellerSalesService _salesService;
    
    public SellerController(
        IMapper mapper,
        ISellerDashboardService dashboardService,
        IOfferManagementService offerManagementService,
        ISellerSalesService salesService)
    {
        _mapper = mapper;
        _dashboardService = dashboardService;
        _offerManagementService = offerManagementService;
        _salesService = salesService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOffer(
        [FromBody] CreateOfferRequest request, 
        CancellationToken ct = default)
    {
        var command = _mapper.Map<CreateOfferCommand>(request);
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var offer = await _offerManagementService.CreateOfferAsync(command, userId,ct);
        var offerdto = _mapper.Map<OfferDetailsDto>(offer);
        return CreatedAtAction(nameof(GetOfferById), new { offerId = offer.Id }, offerdto);
    }
    
    [HttpPut]
    public async Task<IActionResult> UpdateOffer(
        [FromBody] UpdateOfferRequest request, 
        CancellationToken ct = default)
    {
        var command = _mapper.Map<UpdateOfferCommand>(request);
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var offer = await _offerManagementService.UpdateOfferAsync(command, userId,ct);
        return Ok(_mapper.Map<OfferDetailsDto>(offer));
    }
    
    [HttpPut("disable/{offerId:int}")]
    public async Task<IActionResult> DisableOffer(int offerId, CancellationToken ct = default)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _offerManagementService.DisableOfferAsync(offerId, userId,ct);
        return NoContent();
    }
    
    [HttpGet("dashboard-stats")]
    public async Task<IActionResult> GetDashboardStats(CancellationToken ct = default)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var stats = await _dashboardService.GetDashboardStatsAsync(userId, ct);
        return Ok(stats);
    }

    [HttpGet("sales-history")]
    public async Task<IActionResult> GetSalesHistory(CancellationToken ct = default)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var salesHistory = await _salesService.GetSalesHistoryAsync(userId, ct);
        var res = _mapper.Map<List<SellerSaleDto>>(salesHistory);
        return Ok(res);
    }

    [HttpGet("get-offer/{offerId:int}")]
    public async Task<IActionResult> GetOfferById(int offerId, CancellationToken ct = default)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var offer = await _offerManagementService.GetOfferDetailsAsync(offerId, userId, ct);
        return Ok(_mapper.Map<OfferDetailsDto>(offer));
    }
}