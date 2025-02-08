using System.ComponentModel.DataAnnotations;
using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Interfaces;
using OrderManagementService.Core.Interfaces.Repositories;
using OrderManagementService.Core.Interfaces.Services;
using OrderManagementService.Core.Models;

namespace OrderManagementService.Core.Services;

public class MenuItemService : IMenuItemService
{
    private readonly IMenuItemRepository _menuItemRepository;

    public MenuItemService(IMenuItemRepository menuItemRepository)
    {
        _menuItemRepository = menuItemRepository;
    }
    
    public async Task<ServiceResult<MenuItem>> CreateAsync(string userId, MenuItemRequestData data)
    {
        try
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(data);
            
            if (!Validator.TryValidateObject(data, validationContext, validationResults, true))
            {
                var errors = string.Join(", ", validationResults.Select(vr => vr.ErrorMessage));
                return ServiceResult<MenuItem>.Fail(ServiceErrorCode.BadRequest, errors);
            }
            
            var menuItem = new MenuItem
            {
                Name = data.Name,
                Price = data.Price,
                Deleted = data.Deleted,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            menuItem.Id = await _menuItemRepository.CreateAsync(menuItem);
        
            // TODO invalidate cache if any
            // TODO publish any domain events
            
            return ServiceResult<MenuItem>.Ok(menuItem);
        }
        catch (DatabaseException e)
        {
            return ServiceResult<MenuItem>.Fail(ServiceErrorCode.Generic, e.Message);
        }
        catch (Exception e)
        {
            return ServiceResult<MenuItem>.Fail(ServiceErrorCode.Generic, e.Message, e);
        }
    }
    
    public async Task<ServiceResult<MenuItem>> UpdateAsync(string userId, int id, MenuItemRequestData data)
    {
        try
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(id);

            if (menuItem == null)
            {
                return ServiceResult<MenuItem>.Fail(ServiceErrorCode.NotFound, $"Menu item {id} not found");
            }

            menuItem.Name = data.Name;
            menuItem.Price = data.Price;
            menuItem.Deleted = data.Deleted;
            menuItem.LastUpdatedBy = userId;
            menuItem.LastUpdatedAt = DateTime.UtcNow;

            var modifiedCount = await _menuItemRepository.UpdateAsync(menuItem);
            
            if (modifiedCount == 0)
            {
                return ServiceResult<MenuItem>.Fail(ServiceErrorCode.NotFound, $"Menu item {id} not found");
            }

            // TODO invalidate cache if any
            // TODO publish any domain events
            
            return modifiedCount switch
            {
                1 => ServiceResult<MenuItem>.Ok(menuItem),
                _ => ServiceResult<MenuItem>.Fail(
                    ServiceErrorCode.Generic, 
                    $"Unexpected error updating menu item {id}")
            };
        }
        catch (Exception e)
        {
            return ServiceResult<MenuItem>.Fail(ServiceErrorCode.Generic, e.Message, e);
        }
    }

    public async Task<ServiceResult<MenuItem>> GetByIdAsync(int id)
    {
        try
        {
            var menuItem = await _menuItemRepository.GetByIdAsync(id);
            
            return menuItem == null 
                ? ServiceResult<MenuItem>.Fail(ServiceErrorCode.NotFound, $"Menu item {id} not found") 
                : ServiceResult<MenuItem>.Ok(menuItem);
        }
        catch (Exception e)
        {
            return ServiceResult<MenuItem>.Fail(ServiceErrorCode.Generic, e.Message, e);
        }
    }

    public async Task<ServiceResult<List<MenuItem>>> GetByIdsAsync(List<int> ids)
    {
        try
        {
            var menuItems = await _menuItemRepository.GetByIdsAsync(ids);
            return ServiceResult<List<MenuItem>>.Ok(menuItems);
        }
        catch (Exception e)
        {
            return ServiceResult<List<MenuItem>>.Fail(ServiceErrorCode.Generic, e.Message, e);
        }
    }

    public async Task<ServiceResult<List<MenuItem>>> GetAllMenuItemsAsync(bool inclideDeleted)
    {
        try
        {
            var menuItems = await _menuItemRepository.GetAllAsync(inclideDeleted);
            return ServiceResult<List<MenuItem>>.Ok(menuItems);
        }
        catch (Exception e)
        {
            return ServiceResult<List<MenuItem>>.Fail(ServiceErrorCode.Generic, e.Message, e);
        }
    }
} 