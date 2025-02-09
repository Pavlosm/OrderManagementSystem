namespace OrderManagementService.Core.Entities;

public class OrderStatistics
{
    public DateTime SessionStart { get; set; }
    public int CompletedOrdersCount { get; set; }
    public int AvgTimeToCompletionMin { get; set; }
}