using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrderManagementService.Core.Entities;
using OrderManagementService.Core.Interfaces;
using OrderManagementService.Core.Interfaces.Repositories;

namespace OrderManagementService.Core.Services;

public class OrderPublisherService : IOrderPublisherService
{
    private readonly ILogger<OrderPublisherService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public OrderPublisherService(
        ILogger<OrderPublisherService> logger, 
        IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task PublishOrderAsync(OrderDomainEventOutbox eventOutbox)
    {
        try
        {
            
            using var scope = _scopeFactory.CreateScope();
            
            var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
            
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
                await orderRepository.DeleteDomainEventAsync(eventOutbox);
            // }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "An error occurred while publishing the order event: {0}", e.Message);
        }
    }
}