using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Models;

namespace OrderManagementService.Core.Interfaces.Services;

public interface IOrderService
{
    Task<ServiceResult<Order>> GetFullOrderAsync(int id);
    Task<ServiceResult<List<OrderBasic>>> FilterOrdersAsync(OrderStatus? status, OrderType? type);
    Task<ServiceResult<Order>> PlaceOrderAsync(string userId, OrderPlacementRequest orderPlacementRequest);
    Task<VoidServiceResult> UpdateStatusAsync(string userId, int orderId, OrderStatus statusId);
    Task<VoidServiceResult> SetDeliveryStatus(string userId, int orderId, string deliveryStaffId);
}