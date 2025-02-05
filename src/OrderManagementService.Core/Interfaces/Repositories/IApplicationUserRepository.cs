using OrderManagementService.Core.Entities;

namespace OrderManagementService.Core.Interfaces.Repositories;

public interface IApplicationUserRepository
{
    Task CreateAsync(ApplicationUser user);
    Task<ApplicationUser?> GetByIdAsync(int id);
}