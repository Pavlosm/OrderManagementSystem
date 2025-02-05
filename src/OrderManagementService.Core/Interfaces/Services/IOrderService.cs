using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Models;

namespace OrderManagementService.Core.Interfaces.Services;

public interface IOrderService
{
    Task<ServiceResult<Order>> PlaceOrderAsync(OrderPlacementRequest order);
    // Task UpdateOrderStatusAsync(string orderId, int userId, OrderStatus newStatus);
    // Task AssignDeliveryStaff(string orderId, int userId, int staffId);
}