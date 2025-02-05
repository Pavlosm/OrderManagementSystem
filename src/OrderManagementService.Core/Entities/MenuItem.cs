using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OrderManagementService.Core.Entities;

/// <summary>
/// Represents a menu item in the order management system.
/// </summary>
[Table("MenuItem")]
public class MenuItem : AuditInfo
{
    /// <summary>
    /// Gets or sets the unique identifier for the menu item.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the menu item.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the price of the menu item.
    /// </summary>
    [Range(0, int.MaxValue)]
    public decimal Price { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the menu item is available.
    /// </summary>
    public bool Deleted { get; set; }

    /// <summary>
    /// Gets or sets the row version for concurrency control.
    /// </summary>
    [Timestamp]
    public byte[] RowVersion { get; set; } = [];

    /// <summary>
    /// Gets or sets the list of categories associated with the menu item.
    /// </summary>
    public virtual List<Category> Categories { get; set; } = [];
}