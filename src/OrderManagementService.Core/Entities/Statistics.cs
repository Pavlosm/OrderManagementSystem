namespace OrderManagementService.Core.Entities;

public class Statistics
{
    public List<OrderStatistics> OrderStatistics { get; set; } = new ();
    public List<MenuItemStatistics> MenuItemStatistics { get; set; } = new ();
}