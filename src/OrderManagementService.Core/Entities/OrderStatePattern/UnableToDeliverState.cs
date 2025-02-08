namespace OrderManagementService.Core.Entities.OrderStatePattern;

public class UnableToDeliverState : BaseState
{
    public override OrderStatus Status => OrderStatus.UnableToDeliver;

    public UnableToDeliverState(OrderBasic orderBasic) : base(orderBasic)
    {
        ValidateDeliveryType();
    }

    public UnableToDeliverState(BaseState previous) : base(previous)
    {
        ValidateDeliveryType();
        FulfilmentTimeMinutes = UpdatedAt.HasValue 
            ? (int)UpdatedAt.Value.Subtract(CreatedAt).TotalMinutes 
            : (int)DateTime.UtcNow.Subtract(CreatedAt).TotalMinutes;
    }
    
    private void ValidateDeliveryType()
    {
        if (Type != OrderType.Delivery)
        {
            throw new InvalidOperationException("Delivery staff is not required for pickup orders");
        }
    }
}