using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Entities.OrderStatePattern;

namespace OrderManagementService.Core.Tests.Entities.OrderStatePattern;

public class PendingStateTests
{
    private const OrderStatus Status = OrderStatus.Pending;
    
    [Theory]
    [InlineData(OrderType.Delivery, OrderStatus.Preparing, typeof(PreparingState))]
    [InlineData(OrderType.Delivery, OrderStatus.Cancelled, typeof(CancelledState))]
    [InlineData(OrderType.Pickup, OrderStatus.Preparing, typeof(PreparingState))]
    [InlineData(OrderType.Pickup, OrderStatus.Cancelled, typeof(CancelledState))]
    public void Transition_ValidTransitions(OrderType type, OrderStatus status, Type expectedType)
    {
        var order = Helpers.CreateOrder(type);
        order.Status = Status;
        var state = new PendingState(order);
        var (newState, error) = state.Transition(status);
        Assert.IsType(expectedType, newState);
        Assert.Equal(error, string.Empty);
        Helpers.AssertEqual(order, (BaseState)newState, Helpers.UpdatedTestStrategy.LaterThanPrevious);
    }
    
    [Theory]
    [InlineData(OrderType.Delivery, OrderStatus.Pending)]
    [InlineData(OrderType.Delivery, OrderStatus.ReadyForPickup)]
    [InlineData(OrderType.Delivery, OrderStatus.ReadyForDelivery)]
    [InlineData(OrderType.Delivery, OrderStatus.OutForDelivery)]
    [InlineData(OrderType.Delivery, OrderStatus.Delivered)]
    [InlineData(OrderType.Delivery, OrderStatus.UnableToDeliver)]
    [InlineData(OrderType.Delivery, OrderStatus.PickedUp)]
    [InlineData(OrderType.Pickup, OrderStatus.Pending)]
    [InlineData(OrderType.Pickup, OrderStatus.ReadyForPickup)]
    [InlineData(OrderType.Pickup, OrderStatus.ReadyForDelivery)]
    [InlineData(OrderType.Pickup, OrderStatus.OutForDelivery)]
    [InlineData(OrderType.Pickup, OrderStatus.Delivered)]
    [InlineData(OrderType.Pickup, OrderStatus.UnableToDeliver)]
    [InlineData(OrderType.Pickup, OrderStatus.PickedUp)]
    public void Transition_InValidTransitions(OrderType type, OrderStatus status)
    {
        var order = Helpers.CreateOrder(type);
        order.Status = Status;
        var state = new PendingState(order);
        var (newState, error) = state.Transition(status);
        Assert.Equal(state, newState);
        Assert.False(string.IsNullOrEmpty(error));
    }
    
    [Theory]
    [InlineData(OrderType.Delivery)]
    [InlineData(OrderType.Pickup)]
    public void SetDeliveryStaffId_ShouldReturnError_WhenStateIsNotReadyForDelivery(OrderType orderType)
    {
        var order = Helpers.CreateOrder(orderType);
        order.Status = Status;
        var state = new PendingState(order);
        var (newState, error) = state.SetDeliveryStaffId("staff123");
        Assert.Equal(state, newState);
        Assert.False(string.IsNullOrEmpty(error));
        Helpers.AssertEqual(order, (BaseState)newState, Helpers.UpdatedTestStrategy.Equal);
    }
}