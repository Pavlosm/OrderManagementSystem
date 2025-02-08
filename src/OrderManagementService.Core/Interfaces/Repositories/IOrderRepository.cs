using OrderManagementService.Core.Entities;

namespace OrderManagementService.Core.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task<OrderBasic?> GetBasicByIdAsync(int id);
    Task<List<OrderBasic>> GetAllBasicAsync();
    Task<List<OrderBasic>> GetByStatusAndTypeAsync(OrderStatus? status, OrderType? type);
    Task<int> CreateAsync(Order order);
    
    Task<long> UpdateStatusAsync(
        int orderId, 
        OrderStatus status,
        int? fulfillmentTimeMinutes,
        DateTime updatedAt, 
        string updatedBy,
        byte[] rowVersion);
    
    Task<long> SetDeliveryStaffAsync(
        int orderId, 
        string deliveryStuffId, 
        DateTime updatedAt,
        string updatedBy,
        byte[] orderStateRowVersion);
} 