namespace OrderManagementService.Core.Entities.OrderStatePattern;

public abstract class BaseState : IOrderState
{
    public OrderStatus Status { get; }
    
    public readonly OrderType Type;
    public readonly DateTime CreatedAt;
    public int? FulfilmentTimeMinutes { get; protected set; }
    public string? DeliveryStaffId { get; protected set; }
    public DateTime? UpdatedAt { get; }
    
    public BaseState(OrderBasic orderBasic, OrderStatus status) 
    {
        Type = orderBasic.Type;
        DeliveryStaffId = orderBasic.DeliveryStaffId;
        UpdatedAt = orderBasic.LastUpdatedAt;
        CreatedAt = orderBasic.CreatedAt;
        Status = status;
    }
    
    public BaseState(BaseState previous, OrderStatus status)
    {
        Type = previous.Type;
        DeliveryStaffId = previous.DeliveryStaffId;
        CreatedAt = previous.CreatedAt;
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public virtual (IOrderState newState, string? error) Transition(OrderStatus newStatus)
    {
        return (this, $"Invalid state transition from {Status} to {newStatus} for type {Type}");
    }

    public virtual (IOrderState newState, string? error) SetDeliveryStaffId(string deliveryStaffId)
    {
        return (this, $"Delivery staff is not required for pickup orders based on the state {Status}");
    }
}