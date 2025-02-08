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