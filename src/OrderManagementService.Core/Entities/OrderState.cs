namespace OrderManagementService.Core.Entities;

public class OrderState
{
    private static readonly Dictionary<OrderType, Dictionary<OrderStatus, HashSet<OrderStatus>>> TransitionsPerType = new()
    {
        {
            OrderType.Pickup, new Dictionary<OrderStatus, HashSet<OrderStatus>>
            {
                { OrderStatus.Pending, [OrderStatus.Preparing, OrderStatus.Cancelled] },
                { OrderStatus.Preparing, [OrderStatus.ReadyForPickup, OrderStatus.Cancelled] },
                { OrderStatus.ReadyForPickup, [OrderStatus.PickedUp] }
            }
        },
            { OrderType.Delivery , new Dictionary<OrderStatus, HashSet<OrderStatus>>
            {
                { OrderStatus.Pending, [OrderStatus.Preparing, OrderStatus.Cancelled] },
                { OrderStatus.Preparing, [OrderStatus.ReadyForDelivery, OrderStatus.Cancelled] },
                { OrderStatus.ReadyForDelivery, [OrderStatus.OutForDelivery] },
                { OrderStatus.OutForDelivery, [OrderStatus.Delivered, OrderStatus.UnableToDeliver] }
            }
        }
    };
    
    private static readonly IDictionary<OrderType, HashSet<OrderStatus>> AllowedTypesPerStatus =
        TransitionsPerType.ToDictionary(
            t => t.Key, 
            t => t.Value.Keys.Concat(t.Value.SelectMany(v => v.Value)).ToHashSet());

    private static readonly HashSet<OrderStatus> CompletedOrderStatus =
        [OrderStatus.Cancelled, OrderStatus.Delivered, OrderStatus.PickedUp, OrderStatus.UnableToDeliver];

    public OrderStatus Status { get; private set; }
    public readonly OrderType Type;
    public string? DeliveryStaffId { get; private set; }
    public int? FulfillmentTimeMinutes { get; private set; }
   
    private readonly DateTime _createdAt;
    
    public DateTime? UpdatedAt { get; private set; }
    
    private OrderState(OrderStatus status, OrderType type, DateTime createdAt, DateTime? updatedAt)
    {
        Status = status;
        Type = type;
        _createdAt = createdAt;
        UpdatedAt = updatedAt;
    }
    
    public static (OrderState? order, string error) Create(OrderBasic order)
    {
        if (!AllowedTypesPerStatus.TryGetValue(order.Type, out var allowedStatuses))
        {
            return (null, $"Order type {order.Type} is not configured");
        }

        if (!allowedStatuses.Contains(order.Status))
        {
            return (null, $"Invalid status {order.Status} for order type");
        }
        
        var orderState = new OrderState(order.Status, order.Type, order.CreatedAt, order.LastUpdatedAt);
        
        if (CompletedOrderStatus.Contains(order.Status))
        {
            orderState.FulfillmentTimeMinutes = order.FulfillmentTimeMinutes;
        }

        if (order.DeliveryStaffId != null)
        {
            var (success, error) = orderState.SetDeliveryStaffId(order.DeliveryStaffId);
            if (!success)
            {
                return (null, error ?? "Unknown error");
            }
        }
        
        return (orderState, string.Empty);
    }

    public (bool success, string? error) ChangeStatus(OrderStatus newStatus)
    {
        if (newStatus == Status)
        {
            return (false, "Status is already set to the requested value");
        }
        
        if (!TransitionsPerType[Type][Status].Contains(newStatus))
        {
            return (false, $"Invalid state transition from {Status} to {newStatus}");
        }
        
        Status = newStatus;
        
        UpdatedAt = DateTime.UtcNow;
        
        if (CompletedOrderStatus.Contains(newStatus))
        {
            FulfillmentTimeMinutes = (int)UpdatedAt.Value.Subtract(_createdAt).TotalMinutes;
        }
        
        return (true, string.Empty);
    }

    public (bool success, string? error) SetDeliveryStaffId(string deliveryStaffId)
    {
        if (Type != OrderType.Delivery)
        {
            return (false, "Delivery staff is not required for pickup orders");
        }
        
        if (Status != OrderStatus.ReadyForDelivery)
        {
            return (false, "Delivery staff can only be assigned to orders with status ReadyForDelivery");
        }

        if (DeliveryStaffId == deliveryStaffId)
        {
            return (false, "Delivery staff is already assigned to the order");
        }
        
        DeliveryStaffId = deliveryStaffId;
        UpdatedAt = DateTime.UtcNow;
        return (true, null);
    }
}