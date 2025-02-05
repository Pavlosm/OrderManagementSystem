using Microsoft.AspNetCore.Mvc;
using OrderManagementService.Core.Interfaces;
using OrderManagementService.Core.Interfaces.Services;
using OrderManagementService.Core.Models;

namespace OrderManagementService.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenuItemController : ControllerBase
{
    private readonly ILogger<MenuItemController> _logger;
    private readonly IMenuItemService _menuItemService;

    public MenuItemController(ILogger<MenuItemController> logger, IMenuItemService menuItemService)
    {
        _logger = logger;
        _menuItemService = menuItemService;
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var itemResult = await _menuItemService.GetByIdAsync(id);
        
        if (itemResult.Success)
        {
            return Ok(itemResult.Data);
        }
        
        if (itemResult.Error?.Code == ServiceErrorCode.NotFound)
        {
            return NotFound(itemResult.Error.Value.Message);
        }
        
        const string msg = "An error occurred while trying to get menu item";
        _logger.LogError(
            itemResult.Error.Value.Exception, 
            $"{msg} {itemResult.Error.Value.Message}");
        
        return StatusCode(500, msg);
    }
    
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] bool includeDeleted = false)
    {
        var itemResult = await _menuItemService.GetAllMenuItemsAsync(includeDeleted);
        
        if (itemResult.Success)
        {
            return Ok(itemResult.Data);
        }
        
        if (itemResult.Error.Value.Code == ServiceErrorCode.NotFound)
        {
            return NotFound(itemResult.Error.Value.Message);
        }
        
        const string msg = "An error occurred while trying to get menu item";
        
        _logger.LogError(
            itemResult.Error.Value.Exception, 
            $"{msg} {itemResult.Error.Value.Message}");
        
        return StatusCode(500, msg);
    }
    
    [HttpPost("")]
    public async Task<IActionResult> CreateAsync([FromBody] MenuItemRequestData data)
    {
        var createResult = await _menuItemService.CreateAsync(Guid.NewGuid().ToString(), data);
        if (createResult.Success)
        {
            return Ok(createResult.Data);
        }
        
        if (createResult.Error.Value.Code == ServiceErrorCode.BadRequest)
        {
            return BadRequest(createResult.Error.Value.Message);
        }
        
        const string msg = "An error occurred while trying to craete a menu item";
        
        _logger.LogError(
            createResult.Error.Value.Exception, 
            $"{msg} {createResult.Error.Value.Message}");

        return StatusCode(500, msg);
    }
    
    [HttpPost("{id:int}")]
    public async Task<IActionResult> CreateAsync(int id, [FromBody] MenuItemRequestData data)
    {
        var createResult = await _menuItemService.UpdateAsync(Guid.NewGuid().ToString(), id, data);
        if (createResult.Success)
        {
            return Ok(createResult.Data);
        }
        
        switch (createResult.Error.Value.Code)
        {
            case ServiceErrorCode.NotFound:
                return NotFound(createResult.Error.Value.Message);
            case ServiceErrorCode.BadRequest:
                return BadRequest(createResult.Error.Value.Message);
            default:
            {
                const string msg = "An error occurred while trying to craete a menu item";
        
                _logger.LogError(
                    createResult.Error.Value.Exception, 
                    $"{msg} {createResult.Error.Value.Message}");

                return StatusCode(500, msg);
            }
        }
    }
}