using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Entities.OrderStatePattern;

namespace OrderManagementService.Core.Tests.Entities.OrderStatePattern;

public class OutForDeliveryStateTests
{
    private const OrderStatus Status = OrderStatus.OutForDelivery;
    
    [Theory]
    [InlineData(OrderType.Delivery, OrderStatus.Delivered, typeof(DeliveredState))]
    [InlineData(OrderType.Delivery, OrderStatus.UnableToDeliver, typeof(UnableToDeliverState))]
    public void Transition_ValidTransitions(OrderType type, OrderStatus status, Type expectedType)
    {
        var order = Helpers.CreateOrder(type);
        order.Status = Status;
        var state = new OutForDeliveryState(order);
        var (newState, error) = state.Transition(status);
        Assert.IsType(expectedType, newState);
        Assert.Equal(error, string.Empty);
        Helpers.AssertEqual(order, (BaseState)newState, Helpers.UpdatedTestStrategy.LaterThanPrevious, true);
    }
    
    [Theory]
    [InlineData(OrderType.Delivery, OrderStatus.Pending)]
    [InlineData(OrderType.Delivery, OrderStatus.Preparing)]
    [InlineData(OrderType.Delivery, OrderStatus.ReadyForPickup)]
    [InlineData(OrderType.Delivery, OrderStatus.ReadyForDelivery)]
    [InlineData(OrderType.Delivery, OrderStatus.PickedUp)]
    [InlineData(OrderType.Delivery, OrderStatus.Cancelled)]
    [InlineData(OrderType.Delivery, OrderStatus.OutForDelivery)]
    public void Transition_InValidTransitions(OrderType type, OrderStatus status)
    {
        var order = Helpers.CreateOrder(type);
        order.Status = Status;
        var state = new OutForDeliveryState(order);
        var (newState, error) = state.Transition(status);
        Assert.Equal(state, newState);
        Assert.False(string.IsNullOrEmpty(error));
    }
    
    [Fact]
    public void SetDeliveryStaffId_ShouldReturnError_WhenStateIsNotReadyForDelivery()
    {
        var order = Helpers.CreateOrder(OrderType.Delivery);
        order.Status = Status;
        var state = new OutForDeliveryState(order);
        var (newState, error) = state.SetDeliveryStaffId("staff123");
        Assert.Equal(state, newState);
        Assert.False(string.IsNullOrEmpty(error));
        Helpers.AssertEqual(order, (BaseState)newState, Helpers.UpdatedTestStrategy.Equal);
    }
}