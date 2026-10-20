using Microsoft.Extensions.DependencyInjection;

using Tenisu.Application.Players;

namespace Tenisu.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<PlayerQueries>();
        services.AddScoped<PlayerCommands>();

        return services;
    }
}