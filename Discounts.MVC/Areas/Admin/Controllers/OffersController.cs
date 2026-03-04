using System.Security.Claims;
using Discounts.Application.Commands;
using Discounts.Application.Interfaces.AdminModuleContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Models;
using Discounts.MVC.Validation;
using Discounts.MVC.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.MVC.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class OffersController : Controller
{
    private readonly IOfferManagementAdminService _offerAdminService;
    private readonly IOfferRepository _offerRepository;
    private readonly ICommandValidationService _commandValidator;

    public OffersController(
        IOfferManagementAdminService offerAdminService,
        IOfferRepository offerRepository,
        ICommandValidationService commandValidator)
    {
        _offerAdminService = offerAdminService;
        _offerRepository = offerRepository;
        _commandValidator = commandValidator;
    }

    private int GetAdminId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index(int page = 1, int pageSize = 8, CancellationToken ct = default)
    {
        var paged = await _offerAdminService.GetOffersPagedForAdminAsync(GetAdminId(), page, pageSize, ct);
        var dtoItems = paged.Items.Select(o => new OfferDetailsDto
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
            ApprovedAt = o.ApprovedAt,
            RejectedAt = o.RejectedAt,
            RejectionReason = o.RejectionReason,
            EditableUntil = o.EditableUntil,
            SellerId = o.SellerId,
            SellerUserName = o.Seller.UserName,
            Categories = o.Categories.Select(c => c.Name).ToList()
        }).ToList();
        var result = new PagedResult<OfferDetailsDto>(dtoItems, paged.TotalCount, paged.PageNumber, paged.PageSize);
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var offer = await _offerRepository.GetWithDetailsByIdAsync(id, ct);
        if (offer == null) return NotFound();
        return View(offer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int offerId, CancellationToken ct)
    {
        try
        {
            await _offerAdminService.ApproveOfferAsync(GetAdminId(), offerId, ct);
            TempData["Success"] = "Offer approved successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Reject(int id, CancellationToken ct)
    {
        var offer = await _offerRepository.GetWithDetailsByIdAsync(id, ct);
        if (offer == null) return NotFound();

        var model = new RejectOfferViewModel
        {
            OfferId = offer.Id,
            OfferTitle = offer.Title
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(RejectOfferViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var command = new RejectOfferCommand
            {
                OfferId = model.OfferId,
                Reason = model.Reason
            };

            if (!await _commandValidator.ValidateAndAddErrorsAsync(command, ModelState, ct))
                return View(model);

            await _offerAdminService.RejectOfferAsync(GetAdminId(), command, ct);
            TempData["Success"] = "Offer rejected successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }
}

