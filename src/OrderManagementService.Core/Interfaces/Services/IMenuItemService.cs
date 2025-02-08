using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Models;

namespace OrderManagementService.Core.Interfaces.Services;

public interface IMenuItemService
{
    Task<ServiceResult<MenuItem>> CreateAsync(string userId, MenuItemRequestData data);
    Task<ServiceResult<MenuItem>> UpdateAsync(string userId, int id, MenuItemRequestData data);
    Task<ServiceResult<MenuItem>> GetByIdAsync(int id);
    Task<ServiceResult<List<MenuItem>>> GetByIdsAsync(List<int> ids);
    Task<ServiceResult<List<MenuItem>>> GetAllMenuItemsAsync(bool includeDeleted);
}