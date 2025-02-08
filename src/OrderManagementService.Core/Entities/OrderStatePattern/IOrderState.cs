namespace OrderManagementService.Core.Entities.OrderStatePattern;

public interface IOrderState
{
    public string? DeliveryStaffId { get; }
    public int? FulfilmentTimeMinutes { get; }
    public DateTime? UpdatedAt { get; }
    public OrderStatus Status { get; }
    
    public (IOrderState newState, string? error) Transition(OrderStatus newStatus);
    public (IOrderState newState, string? error) SetDeliveryStaffId(string deliveryStaffId);
}