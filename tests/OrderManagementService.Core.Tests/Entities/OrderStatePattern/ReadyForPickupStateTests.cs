using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Entities.OrderStatePattern;

namespace OrderManagementService.Core.Tests.Entities.OrderStatePattern;

public class ReadyForPickupStateTests
{
    private const OrderStatus Status = OrderStatus.ReadyForPickup;
    
    [Theory]
    [InlineData(OrderType.Pickup, OrderStatus.PickedUp, typeof(PickedUpState))]
    public void Transition_ValidTransitions(OrderType type, OrderStatus status, Type expectedType)
    {
        var order = Helpers.CreateOrder(type);
        order.Status = Status;
        var state = new ReadyForPickupState(order);
        var (newState, error) = state.Transition(status);
        Assert.IsType(expectedType, newState);
        Assert.Equal(error, string.Empty);
        Helpers.AssertEqual(order, (BaseState)newState, Helpers.UpdatedTestStrategy.LaterThanPrevious, true);
    }
    
    [Theory]
    [InlineData(OrderType.Pickup, OrderStatus.Pending)]
    [InlineData(OrderType.Pickup, OrderStatus.Preparing)]
    [InlineData(OrderType.Pickup, OrderStatus.OutForDelivery)]
    [InlineData(OrderType.Pickup, OrderStatus.ReadyForPickup)]
    [InlineData(OrderType.Pickup, OrderStatus.ReadyForDelivery)]
    [InlineData(OrderType.Pickup, OrderStatus.Delivered)]
    [InlineData(OrderType.Pickup, OrderStatus.UnableToDeliver)]
    [InlineData(OrderType.Pickup, OrderStatus.Cancelled)]
    public void Transition_InValidTransitions(OrderType type, OrderStatus status)
    {
        var order = Helpers.CreateOrder(type);
        order.Status = Status;
        var state = new ReadyForPickupState(order);
        var (newState, error) = state.Transition(status);
        Assert.Equal(state, newState);
        Assert.False(string.IsNullOrEmpty(error));
    }
    
    [Fact]
    public void SetDeliveryStaffId_ShouldReturnError_WhenStateIsNotReadyForDelivery()
    {
        var order = Helpers.CreateOrder(OrderType.Pickup);
        order.Status = Status;
        var state = new ReadyForPickupState(order);
        var (newState, error) = state.SetDeliveryStaffId("staff123");
        Assert.Equal(state, newState);
        Assert.False(string.IsNullOrEmpty(error));
        Helpers.AssertEqual(order, (BaseState)newState, Helpers.UpdatedTestStrategy.Equal);
    }
}