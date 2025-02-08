namespace OrderManagementService.Core.Entities.OrderStatePattern;

public class PreparingState : BaseState
{
    public PreparingState(OrderBasic orderBasic) : base(orderBasic, OrderStatus.Preparing) { }
    public PreparingState(BaseState previous) : base(previous, OrderStatus.Preparing) { }
    
    public override (IOrderState newState, string? error) Transition(OrderStatus newStatus)
    {
        return newStatus switch
        {
            OrderStatus.Cancelled => (new CancelledState(this), string.Empty),
            OrderStatus.ReadyForDelivery when Type == OrderType.Delivery => 
                (new ReadyForDeliveryState(this), string.Empty),
            OrderStatus.ReadyForDelivery when Type == OrderType.Pickup => 
                (new ReadyForPickupState(this), string.Empty),
            _ => base.Transition(newStatus)
        };
    }
}