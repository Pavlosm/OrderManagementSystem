namespace OrderManagementService.Core.Entities.OrderStatePattern;

public class PendingState : BaseState
{
    public PendingState(OrderBasic orderBasic) : base(orderBasic, OrderStatus.Pending) { }
    public PendingState(BaseState previous) : base(previous, OrderStatus.Pending) { }
    
    public override (IOrderState newState, string? error) Transition(OrderStatus newStatus)
    {
        return newStatus switch
        {
            OrderStatus.Cancelled => (new CancelledState(this), string.Empty),
            OrderStatus.Preparing => (new PreparingState(this), string.Empty),
            _ => base.Transition(newStatus)
        };
    }
}