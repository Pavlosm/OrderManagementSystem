namespace OrderManagementService.Core.Entities;

/// <summary>
/// It is supposed to represent a domain event related to an order. For simplicity, it only contains a JSON string,
/// update or create.
/// </summary>
public class OrderDomainEventOutbox
{
    public long Id { get; set; }
    public MessageType Type { get; set; }
    public int OrderId { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}