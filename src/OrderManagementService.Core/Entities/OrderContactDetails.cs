using System.ComponentModel.DataAnnotations;

namespace OrderManagementService.Core.Entities;

public class OrderContactDetails
{
    [Key]
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    
    [Required]
    [StringLength(100, ErrorMessage = "Name length can't be more than 100.")]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MinLength(10, ErrorMessage = "Phone number length can't be less than 10.")]
    [StringLength(10, ErrorMessage = "Phone number length can't be more than 10.")]
    public string PhoneNumber { get; set; } = string.Empty;
}