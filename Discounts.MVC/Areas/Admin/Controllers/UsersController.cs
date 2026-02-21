using System.Security.Claims;
using Discounts.Application.Commands;
using Discounts.Application.Interfaces.AdminModuleContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Models;
using Discounts.MVC.ViewModels.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Discounts.MVC.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly IUserManagementService _userService;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;

    public UsersController(
        IUserManagementService userService,
        IUserRepository userRepository,
        IRoleRepository roleRepository)
    {
        _userService = userService;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
    }

    private int GetAdminId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public async Task<IActionResult> Index(int page = 1, int pageSize = 8, CancellationToken ct = default)
    {
        var paged = await _userService.GetSettingsPagedForAdminAsync(GetAdminId(), page, pageSize, ct);
        var dtoItems = paged.Items.Select(u => new UserDto
        {
            Id = u.Id,
            UserName = u.UserName,
            Email = u.Email,
            Role = u.Role.Name
        }).ToList();
        var result = new PagedResult<UserDto>(dtoItems, paged.TotalCount, paged.PageNumber, paged.PageSize);
        return View(result);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var model = new CreateUserViewModel
        {
            Roles = await GetRoleSelectList(ct)
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            model.Roles = await GetRoleSelectList(ct);
            return View(model);
        }

        try
        {
            var command = new ManageUserCreationCommand
            {
                UserName = model.UserName,
                Email = model.Email,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                RoleId = model.RoleId
            };
            await _userService.CreateUserAsync(GetAdminId(), command, ct);
            TempData["Success"] = "User created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            model.Roles = await GetRoleSelectList(ct);
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var user = await _userRepository.GetWithRolesAsync(id, ct);
        if (user == null) return NotFound();

        var model = new EditUserViewModel
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            RoleId = user.RoleId,
            Roles = await GetRoleSelectList(ct)
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditUserViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            model.Roles = await GetRoleSelectList(ct);
            return View(model);
        }

        try
        {
            var command = new ManageUserUpdateCommand
            {
                UserId = model.UserId,
                UserName = model.UserName,
                Email = model.Email,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                RoleId = model.RoleId
            };
            await _userService.UpdateUserAsync(GetAdminId(), command, ct);
            TempData["Success"] = "User updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            model.Roles = await GetRoleSelectList(ct);
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Block(int userId, CancellationToken ct)
    {
        try
        {
            await _userService.BlockUserAsync(GetAdminId(), userId, ct);
            TempData["Success"] = "User blocked successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task<List<SelectListItem>> GetRoleSelectList(CancellationToken ct)
    {
        var roles = await _roleRepository.GetAll(ct);
        return roles.Select(r => new SelectListItem(r.Name, r.Id.ToString())).ToList();
    }
}

