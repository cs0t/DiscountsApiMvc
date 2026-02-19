using System.Security.Claims;
using Discounts.Application.Interfaces.CustomerModuleContracts;
using Discounts.Application.Models;
using Discounts.Application.Queries;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public class CustomerController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ICustomerService _customerService;

    public CustomerController(IMapper mapper, ICustomerService customerService)
    {
        _mapper = mapper;
        _customerService = customerService;
    }

    [HttpPost("reserve/{offerId:int}")]
    public async Task<IActionResult> CreateReservation(int offerId, CancellationToken ct = default)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _customerService.CreateReservationAsync(userId, offerId, ct);
        return NoContent();
    }

    [HttpPost("purchase/{offerId:int}")]
    public async Task<IActionResult> PurchaseCoupon(int offerId, CancellationToken ct = default)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _customerService.PurchaseCouponAsync(userId, offerId, ct);
        return NoContent();
    }

    [HttpGet("coupons")]
    public async Task<ActionResult<PagedResult<CouponDetailsDto>>> GetCoupons(int page = 1, int pageSize = 8, CancellationToken ct = default)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var paged = await _customerService.GetCustomerCouponsAsync(userId, page, pageSize, ct);
        var items = paged.Items.Select(c => _mapper.Map<CouponDetailsDto>(c)).ToList();
        var result = new PagedResult<CouponDetailsDto>(items, paged.TotalCount, paged.PageNumber, paged.PageSize);
        return Ok(result);
    }

    [HttpGet("offers")]
    // [AllowAnonymous]
    public async Task<ActionResult<PagedResult<OfferDetailsDto>>> GetApprovedOffers([FromQuery] OfferListQuery query, CancellationToken ct = default)
    {
        // int userId = 0;
        // var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // if (!string.IsNullOrEmpty(idClaim))
        //     userId = int.Parse(idClaim);

        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var paged = await _customerService.GetApprovedOffersAsync(query, userId, ct);
        var items = paged.Items.Select(o => _mapper.Map<OfferDetailsDto>(o)).ToList();
        var result = new PagedResult<OfferDetailsDto>(items, paged.TotalCount, paged.PageNumber, paged.PageSize);
        return Ok(result);
    }
}