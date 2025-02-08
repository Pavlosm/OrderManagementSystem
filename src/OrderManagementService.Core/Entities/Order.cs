using System.ComponentModel.DataAnnotations;

namespace OrderManagementService.Core.Entities;

public class OrderState
{
    
}

public class OrderBasic : AuditInfo
{
    /// <summary>
    /// Gets or sets the unique identifier for the order.
    /// </summary>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the status of the order.
    /// </summary>
    [Required]
    public OrderStatus Status { get; set; }
    
    /// <summary>
    /// Gets or sets the type of the order.
    /// </summary>
    [Required]
    public OrderType Type { get; set; }
    
    /// <summary>
    /// Gets or sets the unique identifier of the assigned delivery staff, if any.
    /// </summary>
    public string? DeliveryStaffId { get; set; }
    
    /// <summary>
    /// Fulfillment time in minutes.
    /// </summary>
    public int FulfillmentTimeMinutes { get; set; }
}

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
    public virtual OrderDeliveryAddress DeliveryAddress { get; set; } = null!;

    /// <summary>
    /// Gets or sets the row version for concurrency control.
    /// </summary>
    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;
}