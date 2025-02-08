using Microsoft.EntityFrameworkCore;
using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Interfaces.Repositories;

namespace OrderManagementService.Infrastructure;

public class MenuItemRepository : IMenuItemRepository
{
    private readonly ApplicationDbContext _context;

    public MenuItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MenuItem?> GetByIdAsync(int id)
    {
        return await _context.MenuItems.FindAsync(id);
    }

    public async Task<List<MenuItem>> GetByIdsAsync(List<int> ids)
    {
        return await _context
            .MenuItems
            .Where(menuItem => ids.Contains(menuItem.Id))
            .ToListAsync();
    }

    public async Task<List<MenuItem>> GetAllAsync(bool includeDeleted)
    {
        return includeDeleted
            ? await _context.Set<MenuItem>().ToListAsync()
            : await _context.Set<MenuItem>().Where(menuItem => !menuItem.Deleted).ToListAsync();
    }

    public async Task<int> CreateAsync(MenuItem menuItem)
    {
        _context.MenuItems.Add(menuItem);
        await _context.SaveChangesAsync();
        return menuItem.Id;
    }

    public async Task<long> UpdateAsync(MenuItem menuItem)
    {
        _context.MenuItems.Attach(menuItem);
        var modifiedCount = await _context.SaveChangesAsync();
        return modifiedCount;
    }
}