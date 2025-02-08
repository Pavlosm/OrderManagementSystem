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

    private static readonly Expression<Func<Order, OrderBasic>> MapToBasicExpr = o => new OrderBasic
    {
        Id = o.Id,
        Status = o.Status,
        Type = o.Type,
        CreatedAt = o.CreatedAt,
        CreatedBy = o.CreatedBy,
        LastUpdatedAt = o.LastUpdatedAt,
        LastUpdatedBy = o.LastUpdatedBy,
        DeliveryStaffId = o.DeliveryStaffId,
        FulfillmentTimeMinutes = o.FulfillmentTimeMinutes,
        RowVersion = o.RowVersion
    };

    public async Task<OrderBasic?> GetBasicByIdAsync(int id)
    {
        var order = await _context.Orders
            .Select(MapToBasicExpr)
            .FirstOrDefaultAsync(o => o.Id == id);

        return order;
    }

    public async Task<List<OrderBasic>> GetAllBasicAsync()
    {
        var orders = await SearchAsync();
        return orders;
    }

    public async Task<List<OrderBasic>> GetByStatusAndTypeAsync(OrderStatus? status, OrderType? orderType)
    {
        return status switch
        {
            null when orderType == null => await SearchAsync(),
            null => await SearchAsync(o => o.Type == orderType),
            _ => orderType switch
            {
                null => await SearchAsync(o => o.Status == status),
                _ => await SearchAsync(o => o.Status == status && o.Type == orderType)
            }
        };
    }

    private async Task<List<OrderBasic>> SearchAsync(Expression<Func<OrderBasic, bool>>? predicate = null)
    {
        var query = _context.Orders.Select(MapToBasicExpr);

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        var orders = await query.ToListAsync();
        return orders;
    }

    public async Task<int> CreateAsync(Order order)
    {
        // no transaction needed as per documentation, SaveChangesAsync will handle it
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
        return order.Id;
    }

    public async Task<long> UpdateStatusAsync(
        int orderId, 
        OrderStatus status, 
        DateTime updatedAt, 
        string updatedBy,
        byte[] rowVersion)
    {
        var order = new Order
        {
            Id = orderId, 
            Status = status, 
            LastUpdatedAt = updatedAt, 
            LastUpdatedBy = updatedBy,
            RowVersion = rowVersion
        };
        
        _context.Orders.Attach(order);
        _context.Entry(order).Property(o => o.Status).IsModified = true;
        _context.Entry(order).Property(o => o.LastUpdatedAt).IsModified = true;
        _context.Entry(order).Property(o => o.LastUpdatedBy).IsModified = true;
        
        var modifiedCount = await _context.SaveChangesAsync();
        
        return modifiedCount;
    }

    public async Task<long> SetDeliveryStaffAsync(
        int orderId,
        string deliveryStuffId, 
        DateTime updatedAt, 
        string updatedBy,
        byte[] rowVersion)
    {
        var order = new Order
        {
            Id = orderId, 
            DeliveryStaffId = deliveryStuffId, 
            LastUpdatedAt = updatedAt, 
            LastUpdatedBy = updatedBy,
            RowVersion = rowVersion
        };
        
        _context.Orders.Attach(order);
        _context.Entry(order).Property(o => o.DeliveryStaffId).IsModified = true;
        _context.Entry(order).Property(o => o.LastUpdatedAt).IsModified = true;
        _context.Entry(order).Property(o => o.LastUpdatedBy).IsModified = true;
        
        var modifiedCount = await _context.SaveChangesAsync();
        
        return modifiedCount;
    }
}