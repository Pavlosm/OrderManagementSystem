﻿namespace OrderManagementService.Core.Entities.OrderStatePattern;

public class ReadyForDeliveryState : BaseState
{
    public override OrderStatus Status => OrderStatus.ReadyForDelivery;

    public ReadyForDeliveryState(OrderBasic orderBasic) : base(orderBasic)
    {
        ValidateOrderType();
    }
    
    public ReadyForDeliveryState(BaseState previous) : base(previous)
    {
        ValidateOrderType();
    }

    private void ValidateOrderType()
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
            OrderStatus.OutForDelivery => (new OutForDeliveryState(this), string.Empty),
            _ => base.Transition(newStatus)
        };
    } 
    
    public override (IOrderState newState, string? error) SetDeliveryStaffId(string deliveryStaffId)
    {
        if (Type != OrderType.Delivery)
        {
            return (this, "Delivery staff is not required for pickup orders");
        }
        
        DeliveryStaffId = deliveryStaffId;
        
        return (this, null);
    }
}