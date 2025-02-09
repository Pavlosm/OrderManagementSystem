using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Interfaces;
using OrderManagementService.Core.Interfaces.Repositories;

namespace OrderManagementService.Core.Services;

public class OrderPublisherService : IOrderPublisherService
{
    private readonly IOrderRepository _orderRepository;

    public OrderPublisherService(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task PublishOrderAsync(OrderDomainEventOutbox eventOutbox)
    {
        // Publish order to Kafka or RabbitMQ
        await _orderRepository.DeleteDomainEventAsync(eventOutbox);
    }
}