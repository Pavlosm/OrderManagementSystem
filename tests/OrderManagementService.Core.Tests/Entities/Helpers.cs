using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Entities.OrderStatePattern;

namespace OrderManagementService.Core.Tests.Entities;

public static class Helpers
{
    public static OrderBasic CreateOrder(OrderType type)
    {
        return new OrderBasic
        {
            Id = 1234,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            CreatedBy = "user123",
            LastUpdatedAt = DateTime.UtcNow.AddHours(-1),
            Type = type
        };
    }

    public static void AssertEqual(
        OrderBasic order, 
        BaseState state, 
        UpdatedTestStrategy updatedTestStrategy = UpdatedTestStrategy.LaterThanPrevious,
        bool hasFulfilmentTime = false,
        string? deliveryStaffId = null)
    {
        Assert.Equal(order.Type, state.Type);
        Assert.Equal(order.CreatedAt, state.CreatedAt);
        Assert.Equal(deliveryStaffId ?? order.DeliveryStaffId, state.DeliveryStaffId);
        
        switch (updatedTestStrategy)
        {
            case UpdatedTestStrategy.Equal:
                Assert.Equal(order.LastUpdatedAt, state.UpdatedAt);
                break;
            case UpdatedTestStrategy.EarlierThanPrevious:
                Assert.True(state.UpdatedAt < order.LastUpdatedAt);
                break;
            case UpdatedTestStrategy.LaterThanPrevious:
                Assert.True(state.UpdatedAt > order.LastUpdatedAt);
                break;
        }

        var fulfilmentTimeMinutes = !hasFulfilmentTime
            ?order.FulfillmentTimeMinutes
            : state.UpdatedAt.HasValue 
                ? (int)state.UpdatedAt.Value.Subtract(order.CreatedAt).TotalMinutes 
                : (int)DateTime.UtcNow.Subtract(order.CreatedAt).TotalMinutes;
        
        Assert.Equal(fulfilmentTimeMinutes, state.FulfilmentTimeMinutes);
    }
    
    public enum UpdatedTestStrategy
    {
        Equal,
        EarlierThanPrevious,
        LaterThanPrevious
    }
}