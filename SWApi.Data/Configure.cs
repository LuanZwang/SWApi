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
            ConfigureMongoDbConnection(ref services, configuration);
            ConfigureRepositories(ref services);
        }

        private static void ConfigureMongoDbConnection(ref IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(x=> MongoConnectionConfig.ConfigureFromEnvs(configuration, (ILogControl)x.GetService(typeof(ILogControl))));
            services.AddSingleton<IMongoConnection, MongoConnection>();
        }

        private static void ConfigureRepositories(ref IServiceCollection services)
        {
            services.AddScoped<IPlanetRepository, PlanetRepository>();
        }
    }
}