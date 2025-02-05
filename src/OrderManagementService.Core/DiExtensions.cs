using Microsoft.Extensions.DependencyInjection;
using OrderManagementService.Core.Interfaces.Services;
using OrderManagementService.Core.Services;

namespace OrderManagementService.Core;

public static class DiExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IMenuItemService, MenuItemService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IMapService, DummyMapService>();
        return services;
    }
}