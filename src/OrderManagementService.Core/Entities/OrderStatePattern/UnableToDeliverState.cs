namespace OrderManagementService.Core.Entities.OrderStatePattern;

public class UnableToDeliverState : BaseState
{
    public UnableToDeliverState(OrderBasic orderBasic) : base(orderBasic, OrderStatus.UnableToDeliver) { }
    public UnableToDeliverState(BaseState previous) : base(previous, OrderStatus.UnableToDeliver) { }
}