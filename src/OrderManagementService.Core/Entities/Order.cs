using System.ComponentModel.DataAnnotations;

namespace OrderManagementService.Core.Entities;

/// <summary>
/// Represents an order in the order management system.
/// </summary>
public class Order : OrderBasic
{
    /// <summary>
    /// Gets or sets any special instructions for the order.
    /// </summary>
    [MaxLength(2000)]
    public string? SpecialInstructions { get; set; }

    /// <summary>
    /// Gets or sets the total amount for the order.
    /// </summary>
    [Required]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the list of items in the order.
    /// </summary>
    public virtual List<OrderItem> Items { get; set; } = [];

    /// <summary>
    /// Gets or sets the contact details for the order.
    /// </summary>
    public virtual OrderContactDetails ContactDetails { get; set; } = null!;

    /// <summary>
    /// Gets or sets the delivery address for the order.
    /// </summary>
    public OrderDeliveryAddress? DeliveryAddress { get; set; } = null!;
    
    /// <summary>
    /// Holds the unpublished domain events for the order. It should not be in the context of the order... but for simplicity, it is here.
    /// </summary>
    public virtual List<OrderDomainEventOutbox> UnpublishedEvents { get; set; } = [];
}