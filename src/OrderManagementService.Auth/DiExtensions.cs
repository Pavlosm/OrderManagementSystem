using Microsoft.Extensions.DependencyInjection;

namespace OrderManagementService.Auth;

public static class DiExtensions
{
    public static IServiceCollection AddAuthServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        return services;
    }
}