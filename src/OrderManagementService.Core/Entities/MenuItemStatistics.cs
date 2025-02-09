namespace OrderManagementService.Core.Entities;

public class MenuItemStatistics
{
    public DateTime SessionStart { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public int UnitsSold { get; set; }
}