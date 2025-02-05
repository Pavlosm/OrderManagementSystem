using System.Collections.Generic;
using System.Threading.Tasks;
using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Models;

namespace OrderManagementService.Core.Interfaces.Repositories;

public interface IMenuItemRepository
{
    Task<MenuItem?> GetByIdAsync(int id);
    Task<List<MenuItem>> GetByIdsAsync(List<int> ids);
    Task<List<MenuItem>> GetAllAsync(bool includeDeleted = false);
    Task<int> CreateAsync(MenuItem menuItem);
    Task<long> UpdateAsync(MenuItem menuItem);
}