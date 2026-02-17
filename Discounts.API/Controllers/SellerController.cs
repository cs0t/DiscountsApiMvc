using Discounts.API.Requests.SellerRequests;
using Discounts.Application.Commands;
using Discounts.Application.Interfaces.SellerModuleServiceContracts;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SellerController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly ISellerDashboardService _dashboardService;
    private readonly IOfferManagementService _offerManagementService;
    
    public SellerController(
        IMapper mapper,
        ISellerDashboardService dashboardService,
        IOfferManagementService offerManagementService)
    {
        _mapper = mapper;
        _dashboardService = dashboardService;
        _offerManagementService = offerManagementService;
    }
    
    // [HttpPost]
    // public async Task<IActionResult> CreateOffer([FromBody] CreateOfferRequest request)
    // {
    //     // Map the request to a command
    //     var command = _mapper.Map<CreateOfferCommand>(request);
    //     
    //     // Send the command to the mediator
    //     //var result = await _mediator.Send(command);
    //     await 
    //     // Return the result
    //     return Ok(result);
    // }
}