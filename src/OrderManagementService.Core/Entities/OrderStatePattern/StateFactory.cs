namespace OrderManagementService.Core.Entities.OrderStatePattern;

public class StateFactoryService
{
    public static IOrderState CreateState(OrderBasic orderBasic)
    {
        return orderBasic.Status switch
        {
            OrderStatus.Pending => new PendingState(orderBasic),
            OrderStatus.Preparing => new PreparingState(orderBasic),
            OrderStatus.ReadyForPickup => new ReadyForPickupState(orderBasic),
            OrderStatus.ReadyForDelivery => new ReadyForDeliveryState(orderBasic),
            OrderStatus.OutForDelivery => new OutForDeliveryState(orderBasic),
            OrderStatus.Delivered => new DeliveredState(orderBasic),
            OrderStatus.Cancelled => new CancelledState(orderBasic),
            OrderStatus.UnableToDeliver => new UnableToDeliverState(orderBasic),
            OrderStatus.PickedUp => new PickedUpState(orderBasic),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
}