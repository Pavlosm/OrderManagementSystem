using OrderManagementService.Core.Entities;

namespace OrderManagementService.Core.Interfaces.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task<IEnumerable<Order>> GetAllAsync();
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status);
    Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId);
    Task<int> CreateAsync(Order order);
    Task DeleteAsync(Guid id);
    Task UpdateStatusAsync(int orderId, OrderStatus status, DateTime updatedAt, Guid updatedBy);
    Task UpdateStatusAsync(int orderId, Guid deliveryStuffId, DateTime updatedAt, Guid updatedBy);
} 