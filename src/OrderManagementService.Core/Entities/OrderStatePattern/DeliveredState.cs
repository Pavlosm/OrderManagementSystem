namespace OrderManagementService.Core.Entities.OrderStatePattern;

public class DeliveredState : BaseState
{
    public override OrderStatus Status => OrderStatus.Delivered;

    public DeliveredState(OrderBasic orderBasic) : base(orderBasic)
    {
        ValidateDeliveryType();
    }
    
    public DeliveredState(BaseState previous) : base(previous)
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
}