using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderManagementService.Core.Interfaces.Services;

namespace OrderManagementService.Core;

/// <summary>
/// Handles unpublished domain events
/// </summary>
public class OrderOutboxService : BackgroundService
{
    private readonly ILogger<OrderOutboxService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public OrderOutboxService(
        ILogger<OrderOutboxService> logger, 
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await RecurringJobAsync(stoppingToken);
    }

    private async Task RecurringJobAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
                await orderService.PublishUnPublishedDomainEventsAsync();
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(OrderOutboxService)}.{nameof(RecurringJobAsync)} threw an exception");
            }
        }
    }
}