using System.ComponentModel.DataAnnotations;

namespace OrderManagementService.Core.Entities;

public class OrderDeliveryAddress
{
    [Key]
    public int Id { get; set; }
    
    public int OrderId { get; set; }
    
    [Required]
    [StringLength(100, ErrorMessage = "Street address length can't be more than 100.")]
    public string Street { get; set; } = string.Empty;
    
    [Required]
    [Range(1, int.MaxValue)]
    public int BuildingNumber { get; set; }

    [Required]
    [StringLength(50, ErrorMessage = "City length can't be more than 50.")]
    public string City { get; set; } = string.Empty;

    [Required]
    [StringLength(10, ErrorMessage = "Postal code length can't be more than 10.")]
    public string PostalCode { get; set; } = string.Empty;

    [Required]
    [StringLength(50, ErrorMessage = "Country length can't be more than 50.")]
    public string Country { get; set; } = string.Empty;
}