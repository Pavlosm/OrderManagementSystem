namespace OrderManagementService.Core.Entities.OrderStatePattern;

public class DeliveredState : BaseState
{
    public DeliveredState(OrderBasic orderBasic) : base(orderBasic, OrderStatus.Delivered)
    {
        ValidateDeliveryType();
    }
    
    public DeliveredState(BaseState previous) : base(previous, OrderStatus.Delivered)
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