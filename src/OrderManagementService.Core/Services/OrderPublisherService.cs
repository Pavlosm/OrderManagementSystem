using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Interfaces;
using OrderManagementService.Core.Interfaces.Repositories;

namespace OrderManagementService.Core.Services;

public class OrderPublisherService : IOrderPublisherService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrderPublisherService> _logger;

    public OrderPublisherService(IOrderRepository orderRepository, ILogger<OrderPublisherService> logger)
    {
        _orderRepository = orderRepository;
        _logger = logger;
    }

    public async Task PublishOrderAsync(OrderDomainEventOutbox eventOutbox)
    {
        try
        {
            // var connectionsStr = Environment.GetEnvironmentVariable("KAFKA_BOOTSTRAP_SERVERS");
            // _logger.LogInformation("Kafka bootstrap servers: {0}", connectionsStr);
            // var config = new ProducerConfig { BootstrapServers = connectionsStr };
            // using var producer = new ProducerBuilder<Null, string>(config).Build();
            // var message = new Message<Null, string> { Value = eventOutbox.Message };
            // var deliveryResult = await producer.ProduceAsync("OrderDomainEvents", message);
            // _logger.LogInformation("Delivered '{0}' to '{1}' with result {2}", deliveryResult.Value,
            //     deliveryResult.Topic, deliveryResult.Status);
            // if (deliveryResult.Status == PersistenceStatus.Persisted)
            // {
                await _orderRepository.DeleteDomainEventAsync(eventOutbox);
            // }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while publishing the order event: {0}", e.Message);
        }
        
    }
}