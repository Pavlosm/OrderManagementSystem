using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Entities.OrderStatePattern;
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
        RowVersion = o.RowVersion
    };

    public async Task<OrderBasic?> GetBasicByIdAsync(int id)
    {
        var order = await _context.Orders
            .Select(MapToBasicExpr)
            .FirstOrDefaultAsync(o => o.Id == id);

        return order;
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
        IOrderState state, 
        string updatedBy,
        byte[] rowVersion,
        OrderDomainEventOutbox eventOutbox)
    {
        var order = new Order
        {
            Id = orderId, 
            Status = state.Status, 
            LastUpdatedAt = state.UpdatedAt, 
            LastUpdatedBy = updatedBy,
            RowVersion = rowVersion
        };
        
        _context.Orders.Attach(order);
        _context.Entry(order).Property(o => o.Status).IsModified = true;
        _context.Entry(order).Property(o => o.LastUpdatedAt).IsModified = true;
        _context.Entry(order).Property(o => o.LastUpdatedBy).IsModified = true;
        _context.OrderDomainEvents.Add(eventOutbox);
        var modifiedCount = await _context.SaveChangesAsync();
        
        return modifiedCount;
    }

    public async Task<long> SetDeliveryStaffAsync(
        int orderId,
        IOrderState state, 
        string updatedBy,
        byte[] rowVersion,
        OrderDomainEventOutbox eventOutbox)
    {
        var order = new Order
        {
            Id = orderId, 
            DeliveryStaffId = state.DeliveryStaffId, 
            LastUpdatedAt = state.UpdatedAt, 
            LastUpdatedBy = updatedBy,
            RowVersion = rowVersion
        };
        
        _context.Orders.Attach(order);
        _context.Entry(order).Property(o => o.DeliveryStaffId).IsModified = true;
        _context.Entry(order).Property(o => o.LastUpdatedAt).IsModified = true;
        _context.Entry(order).Property(o => o.LastUpdatedBy).IsModified = true;
        _context.OrderDomainEvents.Add(eventOutbox);
        var modifiedCount = await _context.SaveChangesAsync();
        
        return modifiedCount;
    }

    public async Task<List<OrderDomainEventOutbox>> GetOrderDomainEventOutboxAsync()
    {
        return await _context.OrderDomainEvents.ToListAsync();
    }

    public async Task DeleteDomainEventAsync(OrderDomainEventOutbox message)
    {
        _context.OrderDomainEvents.Remove(message);
        await _context.SaveChangesAsync();
    }

    public async Task<List<OrderBasic>> FindOrdersForDeliveryAsync(string userId)
    {
        return await _context.Orders
            .Where(o => o.Status == OrderStatus.ReadyForDelivery && o.DeliveryStaffId == userId)
            .Select(MapToBasicExpr)
            .ToListAsync();
    }
    
    public async Task<Statistics> GetStatisticsPerDayAsync()
    {
        var orderStats = await _context.Database.SqlQuery<OrderStatistics>(
            @$"SELECT
                  max(CreatedAt) as SessionStart,
                  count(1) as CompletedOrdersCount,
                  AVG(DATEDIFF(MINUTE, LastUpdatedAt, CreatedAt)) AS AvgTimeToCompletionMin
               FROM [dbo].[Orders]
               WHERE [dbo].[Orders].[Status] = 5
               GROUP BY
                  datepart(year, CreatedAt),
                  datepart(month, CreatedAt),
                  datepart(week, CreatedAt),
                  datepart(day, CreatedAt)").ToListAsync();
        
        var menuItemStatistics = await _context.Database.SqlQuery<MenuItemStatistics>(
            @$"SELECT 
	            max(o.CreatedAt) as SessionStart,
	            m.Name AS ItemName,
	            COUNT(1) AS UnitsSold
            FROM [dbo].[Orders] AS o
            INNER JOIN [dbo].[OrderItems] AS i ON o.Id = i.OrderId
            INNER JOIN [dbo].MenuItem AS m ON i.MenuItemId = m.Id
            WHERE o.Status = 5  
            GROUP BY 
	            datepart(year, o.CreatedAt),
	            datepart(month, o.CreatedAt),
	            datepart(week, o.CreatedAt),
	            datepart(day, o.CreatedAt),
	            i.MenuItemId,
	            m.Name
            ").ToListAsync();

        
        
        return new Statistics
        {
            OrderStatistics = orderStats,
            MenuItemStatistics = menuItemStatistics
        };
    }
}