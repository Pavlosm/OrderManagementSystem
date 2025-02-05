using System.ComponentModel.DataAnnotations;

namespace OrderManagementService.Core.Models;

public class MenuItemRequestData
{
    [Required]
    [StringLength(200, ErrorMessage = "Name length can't be more than 200.")]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [Range(0, 2000.00)]
    public decimal Price { get; set; }
    
    public bool Deleted { get; set; }
}