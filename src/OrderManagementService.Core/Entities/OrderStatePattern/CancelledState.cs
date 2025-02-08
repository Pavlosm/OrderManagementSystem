namespace OrderManagementService.Core.Entities.OrderStatePattern;

public class CancelledState : BaseState
{
    public CancelledState(OrderBasic orderBasic) 
        : base(orderBasic, OrderStatus.Cancelled) { }
    public CancelledState(BaseState other) 
        : base(other, OrderStatus.Cancelled) { }
}