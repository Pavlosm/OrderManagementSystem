namespace OrderManagementService.Core.Entities.OrderStatePattern;

public class ReadyForPickupState : BaseState
{
    public override OrderStatus Status => OrderStatus.ReadyForPickup;

    public ReadyForPickupState(OrderBasic orderBasic) : base(orderBasic)
    {
        ValidateOrderType();
    }
    
    public ReadyForPickupState(BaseState previous) : base(previous)
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

    public override (IOrderState newState, string? error) Transition(OrderStatus newStatus)
    {
        return newStatus switch
        {
            OrderStatus.PickedUp => (new PickedUpState(this), string.Empty),
            _ => base.Transition(newStatus)
        };
    }
}