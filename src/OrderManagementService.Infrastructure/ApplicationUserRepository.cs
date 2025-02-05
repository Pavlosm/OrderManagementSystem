using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Interfaces.Repositories;

namespace OrderManagementService.Infrastructure;

public class ApplicationUserRepository : IApplicationUserRepository
{
    public Task CreateAsync(ApplicationUser user)
    {
        throw new System.NotImplementedException();
    }

    public Task<ApplicationUser?> GetByIdAsync(int id)
    {
        throw new System.NotImplementedException();
    }
}