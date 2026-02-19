using System.Security.Claims;
using Discounts.API.Requests.AdminRequests;
using Discounts.Application.Commands;
using Discounts.Application.Interfaces.AdminModuleContracts;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ICategoryManagementService _categoryService;
    private readonly ISystemSettingsManagementService _settingsService;
    private readonly IUserManagementService _userService;
    private readonly IOfferManagementAdminService _offerAdminService;

    public AdminController(
        IMapper mapper,
        ICategoryManagementService categoryService,
        ISystemSettingsManagementService settingsService,
        IUserManagementService userService,
        IOfferManagementAdminService offerAdminService)
    {
        _mapper = mapper;
        _categoryService = categoryService;
        _settingsService = settingsService;
        _userService = userService;
        _offerAdminService = offerAdminService;
    }

    private int GetAdminId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // Categories
    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request, CancellationToken ct = default)
    {
        var command = _mapper.Map<CreateCategoryCommand>(request);
        var id = await _categoryService.CreateCategoryAsync(GetAdminId(), command, ct);
        return CreatedAtAction(null, new { id }, id);
    }

    [HttpPut("categories")]
    public async Task<IActionResult> UpdateCategory([FromBody] UpdateCategoryRequest request, CancellationToken ct = default)
    {
        var command = _mapper.Map<UpdateCategoryCommand>(request);
        await _categoryService.UpdateCategoryAsync(GetAdminId(), command, ct);
        return NoContent();
    }

    // System settings
    [HttpPost("system-settings")]
    public async Task<IActionResult> CreateSystemSetting([FromBody] CreateSystemSettingRequest request, CancellationToken ct = default)
    {
        var command = _mapper.Map<CreateSystemSettingCommand>(request);
        var id = await _settingsService.CreateSystemSettingAsync(GetAdminId(), command, ct);
        return CreatedAtAction(null, new { id }, id);
    }

    [HttpPut("system-settings")]
    public async Task<IActionResult> UpdateSystemSetting([FromBody] UpdateSystemSettingRequest request, CancellationToken ct = default)
    {
        var command = _mapper.Map<UpdateSystemSettingCommand>(request);
        await _settingsService.UpdateSystemSettingAsync(GetAdminId(), command, ct);
        return NoContent();
    }

    // Users
    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] ManageUserCreationRequest request, CancellationToken ct = default)
    {
        var command = _mapper.Map<ManageUserCreationCommand>(request);
        var id = await _userService.CreateUserAsync(GetAdminId(), command, ct);
        return CreatedAtAction(null, new { id }, id);
    }

    [HttpPut("users")]
    public async Task<IActionResult> UpdateUser([FromBody] ManageUserUpdateRequest request, CancellationToken ct = default)
    {
        var command = _mapper.Map<ManageUserUpdateCommand>(request);
        await _userService.UpdateUserAsync(GetAdminId(), command, ct);
        return NoContent();
    }

    [HttpPut("users/block/{userId:int}")]
    public async Task<IActionResult> BlockUser(int userId, CancellationToken ct = default)
    {
        await _userService.BlockUserAsync(GetAdminId(), userId, ct);
        return NoContent();
    }

    // Offers
    [HttpPost("offers/approve/{offerId:int}")]
    public async Task<IActionResult> ApproveOffer(int offerId, CancellationToken ct = default)
    {
        await _offerAdminService.ApproveOfferAsync(GetAdminId(), offerId, ct);
        return NoContent();
    }

    [HttpPost("offers/reject/{offerId:int}")]
    public async Task<IActionResult> RejectOffer(int offerId, [FromBody] RejectOfferRequest request, CancellationToken ct = default)
    {
        var command = new RejectOfferCommand { OfferId = offerId, Reason = request.Reason };
        await _offerAdminService.RejectOfferAsync(GetAdminId(), command, ct);
        return NoContent();
    }
}
