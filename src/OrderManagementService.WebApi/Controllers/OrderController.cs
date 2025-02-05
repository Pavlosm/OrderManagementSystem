using Microsoft.AspNetCore.Mvc;
using OrderManagementService.Core.Interfaces;
using OrderManagementService.Core.Interfaces.Services;
using OrderManagementService.Core.Models;
using OrderManagementService.Core.Services;

namespace OrderManagementService.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly ILogger<MenuItemController> _logger;
    private readonly IOrderService _orderService;

    public OrderController(
        IOrderService orderService,
        ILogger<MenuItemController> logger)
    {
        _orderService = orderService;
        _logger = logger;
    }

    [HttpPost("place")]
    public async Task<IActionResult> Get([FromBody] OrderPlacementRequest placementRequest)
    {
        var placementResult = await _orderService.PlaceOrderAsync(placementRequest);
        
        if (placementResult.Success)
        {
            _logger.LogInformation("Order {id} placed successfully", placementResult.Data!.Id);
            return Ok(placementResult.Data);
        }

        if (placementResult.Error!.Value.Code == ServiceErrorCode.BadRequest)
        {
            return BadRequest(placementResult.Error.Value.Message);
        }
        
        _logger.LogError(
            placementResult.Error.Value.Exception,
            "An error occurred while trying to place order: {message}", 
            placementResult.Error.Value.Message);
        
        return StatusCode(500, "An error occurred while trying to place order");
    }
}