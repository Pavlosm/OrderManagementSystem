using OrderManagementService.Core.Entities;

namespace OrderManagementService.Core.Models;

public class OrderPlacementRequest
{
    public OrderType OrderType { get; set; }
    
    public OrderContactDetails ContactDetails { get; set; } = new();
    public OrderDeliveryAddress? DeliveryAddress { get; set; }
    
    public string? SpecialInstructions { get; set; }
    public Dictionary<int, Item> Items { get; set; } = new();
    
    public class Item
    {
        public ushort Quantity { get; set; }
        public string? SpecialInstructions { get; set; }
    }
}