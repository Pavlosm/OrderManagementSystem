﻿namespace OrderManagementService.Core.Entities.OrderStatePattern;

public class PickedUpState : BaseState
{
    public override OrderStatus Status => OrderStatus.PickedUp;

    public PickedUpState(OrderBasic orderBasic) : base(orderBasic)
    {
        ValidateOrderType();
    }
    
    public PickedUpState(BaseState previous) : base(previous)
    {
        ValidateOrderType();
        FulfilmentTimeMinutes = UpdatedAt.HasValue 
            ? (int)UpdatedAt.Value.Subtract(CreatedAt).TotalMinutes 
            : (int)DateTime.UtcNow.Subtract(CreatedAt).TotalMinutes;
    }

    private void ValidateOrderType()
    {
        if (Type != OrderType.Pickup)
        {
            throw new InvalidOperationException("Delivery staff is not required for pickup orders");
        }
    }
}