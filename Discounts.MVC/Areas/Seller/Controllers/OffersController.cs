using System.Security.Claims;
using Discounts.Application.Commands;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Interfaces.SellerModuleServiceContracts;
using Discounts.Application.Queries;
using Discounts.MVC.Validation;
using Discounts.MVC.ViewModels.Seller;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Discounts.MVC.Areas.Seller.Controllers;

[Area("Seller")]
[Authorize(Roles = "Seller")]
public class OffersController : Controller
{
    private readonly IOfferManagementService _offerService;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICommandValidationService _commandValidator;

    public OffersController(
        IOfferManagementService offerService,
        ICategoryRepository categoryRepository,
        ICommandValidationService commandValidator)
    {
        _offerService = offerService;
        _categoryRepository = categoryRepository;
        _commandValidator = commandValidator;
    }

    private int GetSellerId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index(int page = 1, int pageSize = 8, CancellationToken ct = default)
    {
        var query = new OfferListQuery { PageNumber = page, PageSize = pageSize };
        var pagedResult = await _offerService.GetSellerOffersAsync(query, GetSellerId(), ct);
        ViewBag.CurrentPage = page;
        return View(pagedResult);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        try
        {
            var offer = await _offerService.GetOfferDetailsAsync(id, GetSellerId(), ct);
            return View(offer);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var model = new CreateOfferViewModel
        {
            ExpirationDate = DateTime.UtcNow.AddDays(30),
            AvailableCategories = await GetCategorySelectList(ct)
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateOfferViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableCategories = await GetCategorySelectList(ct);
            return View(model);
        }

        try
        {
            var command = new CreateOfferCommand
            {
                Title = model.Title,
                Description = model.Description,
                OriginalPrice = model.OriginalPrice,
                DiscountedPrice = model.DiscountedPrice,
                MaxQuantity = model.MaxQuantity,
                RemainingQuantity = model.RemainingQuantity,
                ExpirationDate = model.ExpirationDate,
                CategoryIds = model.CategoryIds
            };

            if (!await _commandValidator.ValidateAndAddErrorsAsync(command, ModelState, ct))
            {
                model.AvailableCategories = await GetCategorySelectList(ct);
                return View(model);
            }

            await _offerService.CreateOfferAsync(command, GetSellerId(), ct);
            TempData["Success"] = "Offer created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            model.AvailableCategories = await GetCategorySelectList(ct);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        try
        {
            var offer = await _offerService.GetOfferDetailsAsync(id, GetSellerId(), ct);
            var model = new EditOfferViewModel
            {
                OfferId = offer.Id,
                Title = offer.Title,
                Description = offer.Description,
                OriginalPrice = offer.OriginalPrice,
                DiscountedPrice = offer.DiscountedPrice,
                MaxQuantity = offer.MaxQuantity,
                RemainingQuantity = offer.RemainingQuantity,
                ExpirationDate = offer.ExpirationDate,
                CategoryIds = offer.Categories.Select(c => c.Id).ToList(),
                AvailableCategories = await GetCategorySelectList(ct)
            };

            return View(model);
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditOfferViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableCategories = await GetCategorySelectList(ct);
            return View(model);
        }

        try
        {
            var command = new UpdateOfferCommand
            {
                OfferId = model.OfferId,
                Title = model.Title,
                Description = model.Description,
                OriginalPrice = model.OriginalPrice,
                DiscountedPrice = model.DiscountedPrice,
                MaxQuantity = model.MaxQuantity,
                RemainingQuantity = model.RemainingQuantity,
                ExpirationDate = model.ExpirationDate,
                CategoryIds = model.CategoryIds
            };

            if (!await _commandValidator.ValidateAndAddErrorsAsync(command, ModelState, ct))
            {
                model.AvailableCategories = await GetCategorySelectList(ct);
                return View(model);
            }

            await _offerService.UpdateOfferAsync(command, GetSellerId(), ct);
            TempData["Success"] = "Offer updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            model.AvailableCategories = await GetCategorySelectList(ct);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Disable(int offerId, CancellationToken ct)
    {
        try
        {
            await _offerService.DisableOfferAsync(offerId, GetSellerId(), ct);
            TempData["Success"] = "Offer disabled successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task<List<SelectListItem>> GetCategorySelectList(CancellationToken ct)
    {
        var categories = await _categoryRepository.GetAllCategoriesAsync(ct);
        return categories.Select(c => new SelectListItem(c.Name, c.Id.ToString())).ToList();
    }
}


