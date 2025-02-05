using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Interfaces;
using OrderManagementService.Core.Interfaces.Repositories;
using OrderManagementService.Core.Interfaces.Services;
using OrderManagementService.Core.Models;

namespace OrderManagementService.Core.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMenuItemService _menuItemService;
    private readonly IMapService _mapService;

    public OrderService(IOrderRepository orderRepository, IMenuItemService menuItemService, IMapService mapService)
    {
        _orderRepository = orderRepository;
        _menuItemService = menuItemService;
        _mapService = mapService;
    }
    
    public async Task<ServiceResult<Order>> PlaceOrderAsync(OrderPlacementRequest orderPlacementRequest)
    {
        try
        {
            if (orderPlacementRequest.OrderType == OrderType.Delivery)
            {
                if (orderPlacementRequest.DeliveryAddress == null)
                {
                    return ServiceResult<Order>.Fail(
                        ServiceErrorCode.BadRequest,
                        "Delivery address is required for delivery orders");
                }

                var validationResult = await _mapService.IsValidAddressAsync(orderPlacementRequest.DeliveryAddress);
                
                if (!validationResult.Success || validationResult.Data == false)
                {
                    return ServiceResult<Order>.Fail(
                        ServiceErrorCode.BadRequest,
                        "Invalid delivery address");
                }
            }
            
            if (orderPlacementRequest is { OrderType: OrderType.Pickup, DeliveryAddress: not null })
            {
                return ServiceResult<Order>.Fail(
                    ServiceErrorCode.BadRequest, 
                    "Delivery address is not required for pickup orders");
            }
            
            if (orderPlacementRequest.Items.Count == 0)
            {
                return ServiceResult<Order>.Fail(
                    ServiceErrorCode.BadRequest, 
                    "Order must have at least one item");
            }

            // validate basic menu items
            if (orderPlacementRequest.Items.Any(item => item.Value.Quantity <= 0))
            {
                return ServiceResult<Order>.Fail(
                    ServiceErrorCode.BadRequest, 
                    "Quantity must be greater than 0");
            }
            
            var menuItemIds = orderPlacementRequest.Items.Keys.ToList();
            var menuItemsResult = await _menuItemService.GetByIdsAsync(menuItemIds);
            if (!menuItemsResult.Success || menuItemsResult.Data == null)
            {
                return ServiceResult<Order>.Fail(menuItemsResult.Error!.Value);
            }
            var loadedMenuItems = menuItemsResult.Data.ToDictionary(i => i.Id, i => i);
            
            var order = new Order
            {
                Status = OrderStatus.Pending,
                Type = orderPlacementRequest.OrderType,
                Items = [],
                CreatedAt = DateTime.UtcNow
            };
            
            foreach (var (menuItemId, details) in orderPlacementRequest.Items)
            {
                if (!loadedMenuItems.TryGetValue(menuItemId, out var menuItem) || menuItem.Deleted)
                {
                    return ServiceResult<Order>.Fail(
                        ServiceErrorCode.BadRequest, 
                        $"Menu item with id {menuItemId} not found");
                }
                
                order.Items.Add(new OrderItem
                {
                    MenuItemId = menuItemId,
                    Quantity = details.Quantity,
                    UnitPrice = menuItem.Price,
                    SpecialInstructions = details.SpecialInstructions
                });
                
                order.TotalAmount += menuItem.Price * details.Quantity;
            }
            
            order.Id = await _orderRepository.CreateAsync(order);

            return ServiceResult<Order>.Ok(order);
        }
        catch (Exception ex)
        {
            return ServiceResult<Order>.Fail(ServiceErrorCode.Generic, ex.Message, ex);
        }
    }

    public async Task<ServiceResult<Order>> SetDeliveryStatus(int userId, OrderStatus status)
    {
        // TODO load user from repository
        // TODO check that the user is a delivery staff
        // TODO load order from repository
        // TODO check that the order is assigned to the user
        // TODO update order status - if possible
        throw new NotImplementedException();
    }
    // public async Task UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
    // {
    //     var order = await _orderRepository.GetByIdAsync(orderId) 
    //         ?? throw new InvalidOperationException("Order not found");
    //
    //     // Validate status transition
    //     if (!IsValidStatusTransition(order.Status, newStatus))
    //         throw new InvalidOperationException($"Invalid status transition from {order.Status} to {newStatus}");
    //
    //     order.Status = newStatus;
    //     await _orderRepository.UpdateAsync(order);
    // }

    public bool IsValidStatusTransition(OrderStatus currentStatus, OrderStatus newStatus)
    {
        return (currentStatus, newStatus) switch
        {
            (OrderStatus.Pending, OrderStatus.Preparing) => true,
            (OrderStatus.Preparing, OrderStatus.ReadyForPickup) => true,
            (OrderStatus.Preparing, OrderStatus.ReadyForDelivery) => true,
            (OrderStatus.ReadyForDelivery, OrderStatus.OutForDelivery) => true,
            (OrderStatus.OutForDelivery, OrderStatus.Delivered) => true,
            (OrderStatus.OutForDelivery, OrderStatus.UnableToDeliver) => true,
            (OrderStatus.Pending, OrderStatus.Cancelled) => true,
            _ => false
        };
    }
} 