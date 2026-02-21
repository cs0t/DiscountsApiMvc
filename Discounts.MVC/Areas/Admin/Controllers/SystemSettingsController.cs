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
public class SystemSettingsController : Controller
{
    private readonly ISystemSettingsManagementService _settingsService;
    private readonly ISystemSettingsRepository _settingsRepository;

    public SystemSettingsController(
        ISystemSettingsManagementService settingsService,
        ISystemSettingsRepository settingsRepository)
    {
        _settingsService = settingsService;
        _settingsRepository = settingsRepository;
    }

    private int GetAdminId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index(int page = 1, int pageSize = 8, CancellationToken ct = default)
    {
        var paged = await _settingsService.GetSettingsPagedForAdminAsync(GetAdminId(), page, pageSize, ct);
        return View(paged);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateSystemSettingViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSystemSettingViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var command = new CreateSystemSettingCommand
            {
                Key = model.Key,
                SettingValue = model.SettingValue
            };
            await _settingsService.CreateSystemSettingAsync(GetAdminId(), command, ct);
            TempData["Success"] = "Setting created successfully.";
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
        var setting = await _settingsRepository.GetById(id, ct);
        if (setting == null) return NotFound();

        var model = new EditSystemSettingViewModel
        {
            Id = setting.Id,
            Key = setting.Key,
            NewSettingValue = setting.SettingValue
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditSystemSettingViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var command = new UpdateSystemSettingCommand
            {
                Id = model.Id,
                NewSettingValue = model.NewSettingValue
            };
            await _settingsService.UpdateSystemSettingAsync(GetAdminId(), command, ct);
            TempData["Success"] = "Setting updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }
}

