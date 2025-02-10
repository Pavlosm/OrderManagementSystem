using System.Text.Json;
using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Entities.OrderStatePattern;
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
    private readonly IStateFactory _stateFactory;
    private readonly IOrderPublisherService _orderPublisherService;

    public OrderService(
        IOrderRepository orderRepository, 
        IMenuItemService menuItemService, 
        IMapService mapService,
        IStateFactory stateFactory, 
        IOrderPublisherService orderPublisherService)
    {
        _orderRepository = orderRepository;
        _menuItemService = menuItemService;
        _mapService = mapService;
        _stateFactory = stateFactory;
        _orderPublisherService = orderPublisherService;
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

    public async Task<ServiceResult<List<OrderBasic>>> FindOrdersForDeliveryAsync(string userId)
    {
        try
        {
            var orders = await _orderRepository.FindOrdersForDeliveryAsync(userId);
            return ServiceResult<List<OrderBasic>>.Ok(orders);
        }
        catch (Exception ex)
        {
            return ServiceResult<List<OrderBasic>>.Fail(ServiceErrorCode.Generic, ex.Message, ex);
        }
    }

    public async Task<ServiceResult<Order>> PlaceOrderAsync(string userId, OrderPlacementRequest orderPlacementRequest)
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
                CreatedBy = userId,
                ContactDetails = orderPlacementRequest.ContactDetails,
                DeliveryAddress = orderPlacementRequest.OrderType == OrderType.Delivery 
                    ? orderPlacementRequest.DeliveryAddress
                    : null,
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
            var @event = new OrderDomainEventOutbox
            {
                OrderId = order.Id,
                Type = MessageType.Created,
                Message = JsonSerializer.Serialize(order, new JsonSerializerOptions { WriteIndented = true }),
                CreatedAt = DateTime.UtcNow
            };
            order.UnpublishedEvents.Add(@event);
            order.Id = await _orderRepository.CreateAsync(order);
            await _orderPublisherService.PublishOrderAsync(@event);

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
        
            var (state, error) = _stateFactory.CreateState(order).Transition(statusId);
            if (!string.IsNullOrEmpty(error))
            {
                return VoidServiceResult.Fail(ServiceErrorCode.BadRequest, error);
            }

            var @event = CreateDomainEvent(orderId, userId, order, state);
            
            var updatedCount = await _orderRepository.UpdateStatusAsync(
                orderId, 
                state, 
                userId, 
                order.RowVersion, 
                @event);
            
            if (updatedCount > 0)
            {
                await _orderPublisherService.PublishOrderAsync(@event);
            }
        
            return updatedCount switch
            {
                0 => VoidServiceResult.Fail(ServiceErrorCode.NotFound, "Update failed, order not found"),
                > 2 => VoidServiceResult.Fail(ServiceErrorCode.Generic, "This should never happen"),
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
        
            var (state, error) = _stateFactory.CreateState(order).SetDeliveryStaffId(deliveryStaffId);
            if (!string.IsNullOrEmpty(error))
            {
                return VoidServiceResult.Fail(ServiceErrorCode.BadRequest, error);
            }
            
            var @event = CreateDomainEvent(orderId, userId, order, state); // should create a proper event here

            var updatedCount = await _orderRepository.SetDeliveryStaffAsync(
                orderId, 
                state, 
                userId, 
                order.RowVersion,
                @event);
            
            if (updatedCount > 0)
            {
                await _orderPublisherService.PublishOrderAsync(@event);
            }
        
            return updatedCount switch
            {
                0 => VoidServiceResult.Fail(ServiceErrorCode.NotFound, "Update failed, order not found"),
                > 2 => VoidServiceResult.Fail(ServiceErrorCode.Generic, "This should never happen"),
                _ => VoidServiceResult.Ok()
            };
        }
        catch (Exception ex)
        {
            return VoidServiceResult.Fail(ServiceErrorCode.Generic, ex.Message, ex);
        }
    }

    public async Task<VoidServiceResult> PublishUnPublishedDomainEventsAsync()
    {
        try
        {
            const int maxDop = 10;
            var events = await _orderRepository.GetOrderDomainEventOutboxAsync();
            var chunkSize = Math.Max(1, events.Count / maxDop);
            var chunks = events.OrderBy(e => e.CreatedAt).Chunk(chunkSize);
            var tasks = chunks.Select(async chunk =>
            {
                foreach (var @event in chunk)
                {
                    await _orderPublisherService.PublishOrderAsync(@event);
                }
            });
            await Task.WhenAll(tasks);
            return VoidServiceResult.Ok();
        }
        catch (Exception e)
        {
            return VoidServiceResult.Fail(ServiceErrorCode.Generic, "Could not publish domain event",  e);
        }
    }

    public async Task<ServiceResult<Statistics>> GetStatisticsPerDayAsync()
    {
        try
        {
            var statistics = await _orderRepository.GetStatisticsPerDayAsync();
            return ServiceResult<Statistics>.Ok(statistics);
        }
        catch (Exception e)
        {
            return ServiceResult<Statistics>.Fail(ServiceErrorCode.Generic, "Could not publish domain event",  e);
        }
    }

    private static OrderDomainEventOutbox CreateDomainEvent(int orderId,  string userId, OrderBasic order, IOrderState state)
    {
        // should create a proper event here
        order.Status = state.Status;
        order.DeliveryStaffId = state.DeliveryStaffId;
        order.LastUpdatedAt = state.UpdatedAt;
        order.LastUpdatedBy = userId;
        
        var @event = new OrderDomainEventOutbox
        {
            OrderId = orderId,
            Type = MessageType.Updated,
            Message = JsonSerializer.Serialize(order, new JsonSerializerOptions { WriteIndented = true }),
            CreatedAt = order.LastUpdatedAt ?? DateTime.UtcNow
        };
        return @event;
    }
} 