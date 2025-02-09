namespace OrderManagementService.Core.Entities;

public enum OrderStatus
{
    Pending = 0,
    Preparing = 1,
    ReadyForPickup = 2,
    ReadyForDelivery = 3, 
    OutForDelivery = 4, 
    Delivered = 5,
    Cancelled = 6,
    UnableToDeliver = 7,
    PickedUp = 8 
}