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

    public async Task<ServiceResult<Order>> GetFullOrderAsync(int id)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(id);
            return order == null 
                ? ServiceResult<Order>.Fail(ServiceErrorCode.NotFound, "Order not found") 
                : ServiceResult<Order>.Ok(order);
        }
        catch (Exception ex)
        {
            return ServiceResult<Order>.Fail(ServiceErrorCode.Generic, ex.Message, ex);
        }
    }

    public async Task<ServiceResult<List<OrderBasic>>> FilterOrdersAsync(OrderStatus? status, OrderType? type)
    {
        try
        {
            var orders = await _orderRepository.GetByStatusAndTypeAsync(status, type);
            return ServiceResult<List<OrderBasic>>.Ok(orders);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<OrderBasic>>.Fail(ServiceErrorCode.Generic, ex.Message, ex);
        }
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
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "TODO",
                ContactDetails = orderPlacementRequest.ContactDetails,
                DeliveryAddress = orderPlacementRequest.DeliveryAddress,
                SpecialInstructions = orderPlacementRequest.SpecialInstructions,
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

    public async Task<VoidServiceResult> UpdateStatusAsync(string userId, int orderId, OrderStatus statusId)
    {
        try
        {
            var order = await _orderRepository.GetBasicByIdAsync(orderId);
            if (order == null)
            {
                return VoidServiceResult.Fail(ServiceErrorCode.NotFound, "Order not found");
            }
        
            var (orderState, error) = OrderState.Create(order);
            if (orderState == null)
            {
                return VoidServiceResult.Fail(ServiceErrorCode.BadRequest, error);
            }

            var (success, err) = orderState.ChangeStatus(statusId);
            if (!success)
            {
                return VoidServiceResult.Fail(ServiceErrorCode.BadRequest, err ?? "Unknown error");
            }
        
            var updatedCount = await _orderRepository.UpdateStatusAsync(
                orderId, 
                orderState.Status, 
                orderState.UpdatedAt ?? DateTime.UtcNow, 
                userId,
                order.RowVersion);
            
            if (updatedCount > 0)
            {
                // TODO publish event
            }
        
            return updatedCount switch
            {
                0 => VoidServiceResult.Fail(ServiceErrorCode.NotFound, "Update failed, order not found"),
                > 1 => VoidServiceResult.Fail(ServiceErrorCode.Generic, "This should never happen"),
                _ => VoidServiceResult.Ok()
            };
        }
        catch (Exception ex)
        {
            return VoidServiceResult.Fail(ServiceErrorCode.Generic, ex.Message, ex);
        }
    }
    
    public async Task<VoidServiceResult> SetDeliveryStatus(string userId, int orderId, string deliveryStaffId)
    {
        try
        {
            var order = await _orderRepository.GetBasicByIdAsync(orderId);
            if (order == null)
            {
                return VoidServiceResult.Fail(ServiceErrorCode.NotFound, "Order not found");
            }
        
            var (orderState, error) = OrderState.Create(order);
            if (orderState == null)
            {
                return VoidServiceResult.Fail(ServiceErrorCode.BadRequest, error);
            }

            var (success, err) = orderState.SetDeliveryStaffId(deliveryStaffId);
            if (!success)
            {
                return VoidServiceResult.Fail(ServiceErrorCode.BadRequest, err ?? "Unknown error");
            }
        
            var updatedCount = await _orderRepository.SetDeliveryStaffAsync(
                orderId, 
                orderState.DeliveryStaffId ?? string.Empty, 
                orderState.UpdatedAt ?? DateTime.UtcNow,
                userId,
                order.RowVersion);
            
            if (updatedCount > 0)
            {
                // TODO publish event
            }
        
            return updatedCount switch
            {
                0 => VoidServiceResult.Fail(ServiceErrorCode.NotFound, "Update failed, order not found"),
                > 1 => VoidServiceResult.Fail(ServiceErrorCode.Generic, "This should never happen"),
                _ => VoidServiceResult.Ok()
            };
        }
        catch (Exception ex)
        {
            return VoidServiceResult.Fail(ServiceErrorCode.Generic, ex.Message, ex);
        }
    }
} 