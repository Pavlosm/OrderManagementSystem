using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderManagementService.Core.Interfaces.Repositories;

namespace OrderManagementService.Infrastructure;

public static class InfrastructureDiExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            var connectionString = Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING");
            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IMenuItemRepository, MenuItemRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        return services;
    }
}