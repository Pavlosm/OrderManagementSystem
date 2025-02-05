using System.ComponentModel.DataAnnotations;

namespace OrderManagementService.Core.Entities;

/// <summary>
/// Represents a category in the order management system.
/// </summary>
public class Category : AuditInfo
{
    /// <summary>
    /// Gets or sets the unique identifier for the category.
    /// </summary>
    [Key]
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the category.
    /// </summary>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the list of menu items associated with the category.
    /// </summary>
    public virtual List<MenuItem> MenuItems { get; set; } = [];
}