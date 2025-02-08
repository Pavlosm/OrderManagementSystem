using OrderManagementService.Core.Entities;

namespace OrderManagementService.Core.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task<OrderBasic?> GetBasicByIdAsync(int id);
    Task<List<OrderBasic>> GetAllBasicAsync();
    Task<List<OrderBasic>> GetByStatusAsync(OrderStatus status);
    Task<int> CreateAsync(Order order);
    Task<long> UpdateStatusAsync(int orderId, OrderStatus status, DateTime updatedAt, string updatedBy);
    Task<long> SetDeliveryStaffAsync(int orderId, string deliveryStuffId, DateTime updatedAt, string updatedBy);
} 