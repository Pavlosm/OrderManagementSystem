namespace OrderManagementService.Core.Entities.OrderStatePattern;

public class PreparingState : BaseState
{
    public override OrderStatus Status => OrderStatus.Preparing;
    public PreparingState(OrderBasic orderBasic) : base(orderBasic) { }
    public PreparingState(BaseState previous) : base(previous) { }
    
    public override (IOrderState newState, string? error) Transition(OrderStatus newStatus)
    {
        return newStatus switch
        {
            OrderStatus.Cancelled => (new CancelledState(this), string.Empty),
            OrderStatus.ReadyForDelivery when Type == OrderType.Delivery => 
                (new ReadyForDeliveryState(this), string.Empty),
            OrderStatus.ReadyForPickup when Type == OrderType.Pickup => 
                (new ReadyForPickupState(this), string.Empty),
            _ => base.Transition(newStatus)
        };
    }
    
}