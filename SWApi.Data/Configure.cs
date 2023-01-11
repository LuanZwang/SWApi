using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SWApi.Data.Connection;
using SWApi.Data.Connection.Interface;
using SWApi.Data.Repository.Interface;
using SWApi.Data.Repository.Planet;
using SWApi.Domain.Configuration.Logging;

namespace SWApi.Data
{
    public static class Configure
    {
        public static void ConfigureData(this IServiceCollection services, IConfiguration configuration)
        {
            ConfigureMongoDbConnection(services, configuration);
            ConfigureRepositories(services);
        }

        private static void ConfigureMongoDbConnection(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IMongoConnectionConfig>(
                x=> MongoConnectionConfig.ConfigureFromEnvs(configuration, (ILogControl)x.GetService(typeof(ILogControl))));

            services.AddSingleton<IMongoConnection, MongoConnection>();
        }

        private static void ConfigureRepositories(IServiceCollection services)
        {
            services.AddScoped<IPlanetRepository, PlanetRepository>();
        }
    }
}