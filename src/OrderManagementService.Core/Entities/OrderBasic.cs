using System.ComponentModel.DataAnnotations;

namespace OrderManagementService.Core.Entities;

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
    public int? FulfillmentTimeMinutes { get; set; }
    
    /// <summary>
    /// Gets or sets the row version for concurrency control.
    /// </summary>
    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;
}