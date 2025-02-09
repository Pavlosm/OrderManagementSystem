using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Entities.OrderStatePattern;

namespace OrderManagementService.Core.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task<OrderBasic?> GetBasicByIdAsync(int id);
    Task<List<OrderBasic>> GetByStatusAndTypeAsync(OrderStatus? status, OrderType? type);
    Task<int> CreateAsync(Order order);
    
    Task<long> UpdateStatusAsync(
        int orderId, 
        IOrderState state,
        string updatedBy,
        byte[] rowVersion,
        OrderDomainEventOutbox eventOutbox);
    
    Task<long> SetDeliveryStaffAsync(
        int orderId, 
        IOrderState state,
        string updatedBy,
        byte[] orderStateRowVersion,
        OrderDomainEventOutbox eventOutbox);
    
    Task<List<OrderDomainEventOutbox>> GetOrderDomainEventOutboxAsync();

    Task DeleteDomainEventAsync(OrderDomainEventOutbox message);
    Task<List<OrderBasic>> FindOrdersForDeliveryAsync(string userId);

    Task<Statistics> GetStatisticsPerDayAsync();
} 