namespace OrderManagementService.Core.Entities.OrderStatePattern;

public class OutForDeliveryState : BaseState
{
    public OutForDeliveryState(OrderBasic orderBasic) : base(orderBasic, OrderStatus.OutForDelivery)
    {
        ValidateDeliveryType();
    }
    
    public OutForDeliveryState(BaseState previous) : base(previous, OrderStatus.OutForDelivery)
    {
        ValidateDeliveryType();
    }

    private void ValidateDeliveryType()
    {
        if (Type != OrderType.Delivery)
        {
            throw new InvalidOperationException("Delivery staff is not required for pickup orders");
        }
    }

    public override (IOrderState newState, string? error) Transition(OrderStatus newStatus)
    {
        return newStatus switch
        {
            OrderStatus.Delivered => (new DeliveredState(this), string.Empty),
            OrderStatus.UnableToDeliver => (new UnableToDeliverState(this), string.Empty),
            _ => base.Transition(newStatus)
        };
    }
}