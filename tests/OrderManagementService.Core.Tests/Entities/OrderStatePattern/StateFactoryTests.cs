using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Entities.OrderStatePattern;

namespace OrderManagementService.Core.Tests.Entities.OrderStatePattern;

public class StateFactoryTests
{
    [Theory]
    [InlineData(OrderStatus.Pending, typeof(PendingState))]
    [InlineData(OrderStatus.Preparing, typeof(PreparingState))]
    [InlineData(OrderStatus.ReadyForDelivery, typeof(ReadyForDeliveryState))]
    [InlineData(OrderStatus.OutForDelivery, typeof(OutForDeliveryState))]
    [InlineData(OrderStatus.Delivered, typeof(DeliveredState))]
    [InlineData(OrderStatus.Cancelled, typeof(CancelledState))]
    [InlineData(OrderStatus.UnableToDeliver, typeof(UnableToDeliverState))]
    public void CreateState_ForDeliveryType_WithCorrectStatus(OrderStatus status, Type type)
    {
        var order = Helpers.CreateOrder(OrderType.Delivery);
        order.Status = status;
        var state = new StateFactory().CreateState(order);
        Assert.IsType(type, state);
        Assert.Equal(order.Status, state.Status);
        Assert.Equal(order.DeliveryStaffId, state.DeliveryStaffId);
        Assert.Equal(order.LastUpdatedAt, state.UpdatedAt);
    }
    
    [Theory]
    [InlineData(OrderStatus.ReadyForPickup)]
    [InlineData(OrderStatus.PickedUp)]
    public void CreateState_ForDeliveryType_InvalidStatus(OrderStatus status)
    {
        var order = Helpers.CreateOrder(OrderType.Delivery);
        order.Status = status;
        Assert.Throws<InvalidOperationException>(() => new StateFactory().CreateState(order));
    }
    
    [Theory]
    [InlineData(OrderStatus.Pending, typeof(PendingState))]
    [InlineData(OrderStatus.Preparing, typeof(PreparingState))]
    [InlineData(OrderStatus.ReadyForPickup, typeof(ReadyForPickupState))]
    [InlineData(OrderStatus.Cancelled, typeof(CancelledState))]
    [InlineData(OrderStatus.PickedUp, typeof(PickedUpState))]
    public void CreateState_ForPickupType_WithCorrectStatus(OrderStatus status, Type type)
    {
        var order = Helpers.CreateOrder(OrderType.Pickup);
        order.Status = status;
        var state = new StateFactory().CreateState(order);
        Assert.IsType(type, state);
        Assert.Equal(order.Status, state.Status);
        Assert.Equal(order.DeliveryStaffId, state.DeliveryStaffId);
        Assert.Equal(order.LastUpdatedAt, state.UpdatedAt);
    }
    
    [Theory]
    [InlineData(OrderStatus.ReadyForDelivery)]
    [InlineData(OrderStatus.OutForDelivery)]
    [InlineData(OrderStatus.Delivered)]
    [InlineData(OrderStatus.UnableToDeliver)]
    public void CreateState_ForPickupType_InvalidStatus(OrderStatus status)
    {
        var order = Helpers.CreateOrder(OrderType.Pickup);
        order.Status = status;
        Assert.Throws<InvalidOperationException>(() => new StateFactory().CreateState(order));
    }
}