using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Interfaces;
using OrderManagementService.Core.Interfaces.Services;
using OrderManagementService.Core.Models;
using OrderManagementService.Infrastructure;

namespace OrderManagementService.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[AllowAnonymous]
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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrderAsync(int id)
    {
        var orderResult = await _orderService.GetFullOrderAsync(id);
        if (orderResult.Success)
        {
            return Ok(orderResult.Data);
        }

        if (orderResult.Error.Value.Code == ServiceErrorCode.NotFound)
        {
            return NotFound();
        }
        
        _logger.LogError(
            orderResult.Error.Value.Exception,
            "An error occurred while trying to get order: {message}", orderResult.Error.Value.Message);
        
        return StatusCode(500, "An error occurred while trying to get order");
    }
    
    [HttpGet]
    public async Task<IActionResult> GetOrdersAsync(
        [FromQuery] OrderStatus? status,
        [FromQuery] OrderType? type)
    {
        if (status == null && type == null)
        {
            return BadRequest("At least one filter parameter is required");
        }
        
        var ordersResult = await _orderService.FilterOrdersAsync(status, type);
        if (ordersResult.Success)
        {
            return Ok(ordersResult.Data);
        }
        
        _logger.LogError(
            ordersResult.Error.Value.Exception,
            "An error occurred while trying to get orders: {message}", ordersResult.Error.Value.Message);
        
        return StatusCode(500, "An error occurred while trying to get orders");
    }

    [HttpPost("place")]
    public async Task<IActionResult> PlaceAsync([FromBody] OrderPlacementRequest placementRequest)
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
    
    [HttpPatch("{id:int}/status")]
    public async Task<IActionResult> UpdateOrderStatusAsync(int id, [FromBody] OrderStatus status)
    {
        //var user = User.FindFirst(ClaimTypes.NameIdentifier);
        var updateResult = await _orderService.UpdateStatusAsync(
            DbInit.DbAdminId,
            id,
            status);
        
        if (updateResult.Success)
        {
            _logger.LogInformation("Order {id} status updated successfully", id);
            return NoContent();
        }

        switch (updateResult.Error!.Value.Code)
        {
            case ServiceErrorCode.NotFound:
                return NotFound();
            case ServiceErrorCode.BadRequest:
                return BadRequest(updateResult.Error.Value.Message);
            default:
                _logger.LogError(
                    updateResult.Error.Value.Exception,
                    "An error occurred while trying to update order status: {message}", 
                    updateResult.Error.Value.Message);
                return StatusCode(500, "An error occurred while trying to update order status");
        }
    }
    
    [HttpPatch("{id:int}/delivery")]
    public async Task<IActionResult> SetDeliveryStatusAsync(int id, [FromBody] string deliveryStaffId)
    {
        //var user = User.FindFirst(ClaimTypes.NameIdentifier);
        var updateResult = await _orderService.SetDeliveryStatus(
            DbInit.DbAdminId,
            id,
            deliveryStaffId);
        
        if (updateResult.Success)
        {
            _logger.LogInformation("Order {id} delivery status updated successfully", id);
            return NoContent();
        }

        switch (updateResult.Error!.Value.Code)
        {
            case ServiceErrorCode.NotFound:
                return NotFound();
            case ServiceErrorCode.BadRequest:
                return BadRequest(updateResult.Error.Value.Message);
            default:
                _logger.LogError(
                    updateResult.Error.Value.Exception,
                    "An error occurred while trying to update order delivery status: {message}", 
                    updateResult.Error.Value.Message);
                return StatusCode(500, "An error occurred while trying to update order delivery status");
        }
    }
}