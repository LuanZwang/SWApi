using Microsoft.Extensions.DependencyInjection;
using SWApi.Application.Service.Interface;
using SWApi.Application.Service.Planet;

namespace SWApi.Application;

public static class Configure
{
    public static void ConfigureApplication(this IServiceCollection services)
    {
        ConfigureServices(services);
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<IPlanetService, PlanetService>();
    }
}