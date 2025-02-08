using System.ComponentModel.DataAnnotations;

namespace OrderManagementService.Core.Entities;

/// <summary>
/// Represents an item in an order.
/// </summary>
public class OrderItem
{
    /// <summary>
    /// Gets or sets the unique identifier for the order item.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the menu item associated with the order item.
    /// </summary>
    public int MenuItemId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the order associated with the order item.
    /// </summary>
    public int OrderId { get; set; }

    /// <summary>
    /// Gets or sets the quantity of the menu item ordered.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the menu item.
    /// </summary>
    [Range(1, int.MaxValue)]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets any special instructions for the order item.
    /// </summary>
    public string? SpecialInstructions { get; set; }
}