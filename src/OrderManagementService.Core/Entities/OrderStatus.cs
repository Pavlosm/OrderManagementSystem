namespace OrderManagementService.Core.Entities;

public enum OrderStatus
{
    Pending, // both
    Preparing, // both
    ReadyForPickup, // Pickup
    ReadyForDelivery, // Delivery
    OutForDelivery, // Delivery
    Delivered, // Pickup
    Cancelled, // both
    UnableToDeliver, // Delivery
    PickedUp // Pickup
}