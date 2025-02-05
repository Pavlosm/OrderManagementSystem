using System.ComponentModel.DataAnnotations;

namespace OrderManagementService.Core.Entities;

public class OrderItem
{
    [Key]
    public int Id { get; set; }
    
    public int MenuItemId { get; set; }
    
    public int OrderId { get; set; }
    
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
    
    [Range(1, int.MaxValue)]
    public decimal UnitPrice { get; set; }
    
    public string? SpecialInstructions { get; set; }
}