using System.Security.Claims;
using Discounts.Application.Commands;
using Discounts.Application.Interfaces.AdminModuleContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.MVC.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.MVC.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class CategoriesController : Controller
{
    private readonly ICategoryManagementService _categoryService;
    private readonly ICategoryRepository _categoryRepository;

    public CategoriesController(
        ICategoryManagementService categoryService,
        ICategoryRepository categoryRepository)
    {
        _categoryService = categoryService;
        _categoryRepository = categoryRepository;
    }

    private int GetAdminId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index(int page = 1, int pageSize = 8, CancellationToken ct = default)
    {
        var paged = await _categoryService.GetCategoriesPagedForAdminAsync(GetAdminId(), page, pageSize, ct);
        return View(paged);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateCategoryViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateCategoryViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var command = new CreateCategoryCommand
            {
                Name = model.Name,
                Description = model.Description
            };
            await _categoryService.CreateCategoryAsync(GetAdminId(), command, ct);
            TempData["Success"] = "Category created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var category = await _categoryRepository.GetById(id, ct);
        if (category == null) return NotFound();

        var model = new EditCategoryViewModel
        {
            Id = category.Id,
            NewName = category.Name,
            NewDescription = category.Description
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditCategoryViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var command = new UpdateCategoryCommand
            {
                Id = model.Id,
                NewName = model.NewName,
                NewDescription = model.NewDescription
            };
            await _categoryService.UpdateCategoryAsync(GetAdminId(), command, ct);
            TempData["Success"] = "Category updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int categoryId, CancellationToken ct)
    {
        try
        {
            await _categoryService.DeleteCategoryAsync(GetAdminId(), categoryId, ct);
            TempData["Success"] = "Category deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }
}
