using Microsoft.Extensions.DependencyInjection;
using OrderManagementService.Core.Entities.OrderStatePattern;
using OrderManagementService.Core.Interfaces;
using OrderManagementService.Core.Interfaces.Services;
using OrderManagementService.Core.Services;

namespace OrderManagementService.Core;

public static class DiExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IMenuItemService, MenuItemService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IMapService, DummyMapService>();
        services.AddSingleton<IStateFactory, StateFactory>();
        services.AddScoped<IOrderPublisherService, OrderPublisherService>();
        services.AddHostedService<OrderOutboxService>();
        return services;
    }
}