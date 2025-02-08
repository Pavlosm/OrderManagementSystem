namespace OrderManagementService.Core.Entities.OrderStatePattern;

public class PendingState : BaseState
{
    public override OrderStatus Status => OrderStatus.Pending;
    public PendingState(OrderBasic orderBasic) : base(orderBasic) { }
    
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