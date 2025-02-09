using OrderManagementService.Core.Entities;

namespace OrderManagementService.Core.Interfaces.Repositories;

public interface IMenuItemRepository
{
    Task<MenuItem?> GetByIdAsync(int id);
    Task<List<MenuItem>> GetByIdsAsync(List<int> ids);
    Task<List<MenuItem>> GetAllAsync(bool includeDeleted);
    Task<int> CreateAsync(MenuItem menuItem);
    Task<long> UpdateAsync(MenuItem menuItem);
}