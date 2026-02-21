using System.Security.Claims;
using Discounts.API.Requests.CustomerRequests;
using Discounts.Application.Commands;
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

    [HttpPost("reserve")]
    public async Task<IActionResult> CreateReservation([FromBody]CreateReservationRequest request, CancellationToken ct = default)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var offerId = request.OfferId;
        var rowVersion = Convert.FromBase64String(request.RowVersion);
        await _customerService.CreateReservationAsync(userId, offerId, rowVersion,ct);
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

    [HttpGet("reservations")]
    public async Task<ActionResult<PagedResult<ReservationDto>>> GetReservations(int page = 1, int pageSize = 8, CancellationToken ct = default)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var paged = await _customerService.GetCustomerReservationsAsync(userId, page, pageSize, ct);
        var items = paged.Items.Select(r => _mapper.Map<ReservationDto>(r)).ToList();
        var result = new PagedResult<ReservationDto>(items, paged.TotalCount, paged.PageNumber, paged.PageSize);
        return Ok(result);
    }

    [HttpPost("reservations/cancel/{reservationId:int}")]
    public async Task<IActionResult> CancelReservation(int reservationId, CancellationToken ct = default)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _customerService.CancelReservationAsync(reservationId, userId, ct);
        return NoContent();
    }

    [HttpGet("offers/{offerId:int}/state")]
    public async Task<ActionResult<CustomerOfferStateDto>> GetOfferState(int offerId, CancellationToken ct = default)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var state = await _customerService.GetOfferStateForCustomerAsync(offerId, userId, ct);
        var dto = _mapper.Map<CustomerOfferStateDto>(state);
        dto.Offer = _mapper.Map<OfferDetailsDto>(state.Offer);
        return Ok(dto);
    }

    [HttpGet("offers")]
    // [AllowAnonymous]
    public async Task<ActionResult<PagedResult<OfferDetailsDto>>> GetApprovedOffers([FromQuery] OfferListQuery query, CancellationToken ct = default)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var paged = await _customerService.GetApprovedOffersAsync(query, userId, ct);
        var items = paged.Items.Select(o => _mapper.Map<OfferDetailsDto>(o)).ToList();
        var result = new PagedResult<OfferDetailsDto>(items, paged.TotalCount, paged.PageNumber, paged.PageSize);
        return Ok(result);
    }
}