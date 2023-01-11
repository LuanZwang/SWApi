using SWApi.Domain.Configuration.Logging;

namespace SWApi.Api
{
    public static class Configure
    {
        public static void AddConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigureLogging(services, configuration);
        }

        private static void ConfigureLogging(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ILogControl, LogControl>(x => new LogControl(configuration["Logging:LogLevel:Default"]));
        }
    }
}