namespace OrderManagementService.Core.Entities.OrderStatePattern;

public class CancelledState : BaseState
{
    public override OrderStatus Status => OrderStatus.Cancelled;
    public CancelledState(OrderBasic orderBasic) : base(orderBasic) { }
    public CancelledState(BaseState other) : base(other) { }
}