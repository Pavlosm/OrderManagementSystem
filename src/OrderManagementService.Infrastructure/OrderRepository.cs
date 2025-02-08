using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
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

    public async Task<Order?> GetByIdAsync(int id)
    {
        var order = await _context.Orders
            .Include(o => o.Items)
            .Include(o => o.DeliveryAddress)
            .Include(o => o.ContactDetails)
            .FirstOrDefaultAsync(o => o.Id == id);

        return order;
    }

    public async Task<OrderBasic?> GetBasicByIdAsync(int id)
    {
        var order = await _context.Orders
            .Select(o => new OrderBasic
            {
                Id = o.Id,
                Status = o.Status,
                Type = o.Type,
                CreatedAt = o.CreatedAt,
                CreatedBy = o.CreatedBy,
                LastUpdatedAt = o.LastUpdatedAt,
                LastUpdatedBy= o.LastUpdatedBy,
                DeliveryStaffId = o.DeliveryStaffId,
                FulfillmentTimeMinutes = o.FulfillmentTimeMinutes
            })
            .FirstOrDefaultAsync(o => o.Id == id);

        return order;
    }

    public async Task<List<OrderBasic>> GetAllBasicAsync()
    {
        var orders = await SearchAsync();
        return orders;
    }

    public async Task<List<OrderBasic>> GetByStatusAsync(OrderStatus status)
    {
        var orders = await SearchAsync(o => o.Status == status);
        return orders;
    }

    private async Task<List<OrderBasic>> SearchAsync(Expression<Func<OrderBasic, bool>>? predicate = null)
    {
        var query = _context.Orders
            .Select(o => new OrderBasic
            {
                Id = o.Id,
                Status = o.Status,
                Type = o.Type,
                CreatedAt = o.CreatedAt,
                CreatedBy = o.CreatedBy,
                LastUpdatedAt = o.LastUpdatedAt,
                LastUpdatedBy = o.LastUpdatedBy,
                DeliveryStaffId = o.DeliveryStaffId,
                FulfillmentTimeMinutes = o.FulfillmentTimeMinutes
            });

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        var orders = await query.ToListAsync();

        return orders;
    }

    public async Task<int> CreateAsync(Order order)
    {
        await using var dbContextTransaction = await _context.Database.BeginTransactionAsync();
        _context.Orders.Add(order);
        _context.OrderItems.AddRange(order.Items);
        _context.OrderDeliveryAddresses.Add(order.DeliveryAddress);
        _context.OrderContactDetails.Add(order.ContactDetails);
        await _context.SaveChangesAsync();
        await dbContextTransaction.CommitAsync();
        return order.Id;
    }

    public async Task<long> UpdateStatusAsync(int orderId, OrderStatus status, DateTime updatedAt, string updatedBy)
    {
        var order = new Order
        {
            Id = orderId, 
            Status = status, 
            LastUpdatedAt = updatedAt, 
            LastUpdatedBy = updatedBy
        };
        
        _context.Orders.Attach(order);
        _context.Entry(order).Property(o => o.Status).IsModified = true;
        _context.Entry(order).Property(o => o.LastUpdatedAt).IsModified = true;
        _context.Entry(order).Property(o => o.LastUpdatedBy).IsModified = true;
        
        var modifiedCount = await _context.SaveChangesAsync();
        
        return modifiedCount;
    }

    public async Task<long> SetDeliveryStaffAsync(int orderId, string deliveryStuffId, DateTime updatedAt, string updatedBy)
    {
        var order = new Order
        {
            Id = orderId, 
            DeliveryStaffId = deliveryStuffId, 
            LastUpdatedAt = updatedAt, 
            LastUpdatedBy = updatedBy
        };
        
        _context.Orders.Attach(order);
        _context.Entry(order).Property(o => o.DeliveryStaffId).IsModified = true;
        _context.Entry(order).Property(o => o.LastUpdatedAt).IsModified = true;
        _context.Entry(order).Property(o => o.LastUpdatedBy).IsModified = true;
        
        var modifiedCount = await _context.SaveChangesAsync();
        
        return modifiedCount;
    }
}