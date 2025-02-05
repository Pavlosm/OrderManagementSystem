using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Interfaces.Repositories;

namespace OrderManagementService.Infrastructure;

public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public Task<Order?> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Order>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId)
    {
        throw new NotImplementedException();
    }

    public async Task<int> CreateAsync(Order order)
    {
        await using var dbContextTransaction = await _context.Database.BeginTransactionAsync();
        _context.Orders.Add(order);
        _context.OrderItems.AddRange(order.Items);
        await _context.SaveChangesAsync();
        await dbContextTransaction.CommitAsync();
        return order.Id;
    }

    public Task UpdateAsync(Order order)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}