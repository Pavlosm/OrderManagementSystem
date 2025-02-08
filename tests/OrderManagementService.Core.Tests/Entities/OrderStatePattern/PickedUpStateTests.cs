using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Entities.OrderStatePattern;

namespace OrderManagementService.Core.Tests.Entities.OrderStatePattern;

public class PickedUpStateTests
{
    private const OrderStatus Status = OrderStatus.PickedUp;
    
    [Theory]
    [InlineData(OrderType.Pickup, OrderStatus.Pending)]
    [InlineData(OrderType.Pickup, OrderStatus.Preparing)]
    [InlineData(OrderType.Pickup, OrderStatus.OutForDelivery)]
    [InlineData(OrderType.Pickup, OrderStatus.ReadyForPickup)]
    [InlineData(OrderType.Pickup, OrderStatus.ReadyForDelivery)]
    [InlineData(OrderType.Pickup, OrderStatus.Delivered)]
    [InlineData(OrderType.Pickup, OrderStatus.UnableToDeliver)]
    [InlineData(OrderType.Pickup, OrderStatus.Cancelled)]
    [InlineData(OrderType.Pickup, OrderStatus.PickedUp)]
    public void Transition_InValidTransitions(OrderType type, OrderStatus status)
    {
        var order = Helpers.CreateOrder(type);
        order.Status = Status;
        var state = new PickedUpState(order);
        var (newState, error) = state.Transition(status);
        Assert.Equal(state, newState);
        Assert.False(string.IsNullOrEmpty(error));
    }
    
    [Fact]
    public void SetDeliveryStaffId_ShouldReturnError_WhenStateIsNotReadyForDelivery()
    {
        var order = Helpers.CreateOrder(OrderType.Pickup);
        order.Status = Status;
        var state = new PickedUpState(order);
        var (newState, error) = state.SetDeliveryStaffId("staff123");
        Assert.Equal(state, newState);
        Assert.False(string.IsNullOrEmpty(error));
        Helpers.AssertEqual(order, (BaseState)newState, Helpers.UpdatedTestStrategy.Equal);
    }
}