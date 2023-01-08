using SWApi.Domain.Configuration.Logging;

namespace SWApi.Api;

public static class Configure
{
    public static void AddConfiguration(this IServiceCollection services)
    {
        ConfigureLogging(services);
    }

    private static void ConfigureLogging(IServiceCollection services)
    {
        services.AddSingleton<ILogControl, LogControl>();
    }
}