namespace OrderManagementService.Core.Entities.OrderStatePattern;

public class PickedUpState : BaseState
{
    public PickedUpState(OrderBasic orderBasic) : base(orderBasic, OrderStatus.UnableToDeliver)
    {
        ValidateOrderType();
    }
    
    public PickedUpState(BaseState previous) : base(previous, OrderStatus.UnableToDeliver)
    {
        ValidateOrderType();
    }

    private void ValidateOrderType()
    {
        if (Type != OrderType.Pickup)
        {
            throw new InvalidOperationException("Delivery staff is not required for pickup orders");
        }
    }
}