using OrderManagementService.Core.Entities;

namespace OrderManagementService.Core.Interfaces;

public interface IOrderPublisherService
{
    Task PublishOrderAsync(OrderDomainEventOutbox eventOutbox);
}