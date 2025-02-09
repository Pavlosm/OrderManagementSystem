using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

        if (orderResult.Error!.Value.Code == ServiceErrorCode.NotFound)
        {
            return NotFound();
        }
        
        _logger.LogError(
            orderResult.Error!.Value.Exception,
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
            ordersResult.Error!.Value.Exception,
            "An error occurred while trying to get orders: {message}", ordersResult.Error.Value.Message);
        
        return StatusCode(500, "An error occurred while trying to get orders");
    }

    [HttpPost("place")]
    public async Task<IActionResult> PlaceAsync([FromBody] OrderPlacementRequest placementRequest)
    {
        var placementResult = await _orderService.PlaceOrderAsync(DbInit.DbAdminId, placementRequest);
        
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
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IActionResult> UpdateOrderStatusAsync(int id, [FromBody] OrderStatus status)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized("Could not find user id in claims");
        }
     
        var roles = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        switch (status)
        {
            case OrderStatus.Preparing:
            case OrderStatus.ReadyForPickup:
            case OrderStatus.ReadyForDelivery:
            case OrderStatus.PickedUp:
                if (roles?.Value != Role.ReasturantStuff && roles?.Value != Role.Admin)
                {
                    return Forbid();
                }
                break;
            case OrderStatus.UnableToDeliver:
            case OrderStatus.OutForDelivery:
            case OrderStatus.Delivered:
                if (roles?.Value != Role.DeliveryStaff && roles?.Value != Role.Admin)
                {
                    return Forbid();
                }
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }
        
        var updateResult = await _orderService.UpdateStatusAsync(userId.Value, id, status);
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
    
    [HttpPatch("{id:int}/delivery-stuff")]
    [Authorize(Roles = $"{Role.Admin},{Role.ReasturantStuff}")]
    public async Task<IActionResult> SetDeliveryStaffAsync(int id, [FromBody] string deliveryStaffId)
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized("Could not find user id in claims");
        }
        
        // TODO validate deliveryStaffId
        
        var updateResult = await _orderService.SetDeliveryStatus(
            userId.Value,
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
    
    [HttpGet("delivery-stuff/my-orders")]
    [Authorize(Roles = Role.DeliveryStaff)]
    public async Task<IActionResult> GetMyOrdersAsync()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return Unauthorized("Could not find user id in claims");
        }
        
        var ordersResult = await _orderService.FilterOrdersAsync(null, null);
        if (ordersResult.Success)
        {
            return Ok(ordersResult.Data);
        }
        
        _logger.LogError(
            ordersResult.Error!.Value.Exception,
            "An error occurred while trying to get orders: {message}", ordersResult.Error.Value.Message);
        
        return StatusCode(500, "An error occurred while trying to get orders");
    }
    
    [HttpGet("statistics")]
    [Authorize(Roles = Role.Admin)]
    public async Task<IActionResult> GetStatisticsAsync([FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var statisticsResult = await _orderService.GetStatisticsPerDayAsync();
        if (statisticsResult.Success)
        {
            return Ok(statisticsResult.Data);
        }
        
        _logger.LogError(
            statisticsResult.Error!.Value.Exception,
            "An error occurred while trying to get statistics: {message}", statisticsResult.Error.Value.Message);
        
        return StatusCode(500, "An error occurred while trying to get statistics");
    }
}