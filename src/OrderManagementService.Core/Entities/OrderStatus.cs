namespace OrderManagementService.Core.Entities;

public enum OrderStatus
{
    Pending,
    Preparing,
    ReadyForPickup,
    ReadyForDelivery,
    OutForDelivery,
    Delivered,
    Cancelled,
    UnableToDeliver,
    PickedUp
}