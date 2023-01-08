using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Configuration;
using MongoDB.Driver.Core.Events;
using SWApi.Domain.Configuration.Logging;

namespace SWApi.Data.Connection
{
    public class MongoConnectionConfig
    {
        public string DatabaseName { get; init; }
        public MongoClientSettings MongoClientSettings { get; init; }

        public static MongoConnectionConfig ConfigureFromEnvs(IConfiguration configuration, ILogControl logger)
        {
            var connStr = configuration.GetConnectionString("MONGODB_CONNECTIONSTRING");
            var dbName = configuration["MONGODB_DATABASE_NAME"];

            ArgumentException.ThrowIfNullOrEmpty(connStr);
            ArgumentException.ThrowIfNullOrEmpty(dbName);

            var mongoClientSettings = MongoClientSettings.FromUrl(new MongoUrl(connStr));

            mongoClientSettings.ClusterConfigurator = cb => {
                cb.Subscribe<CommandStartedEvent>(e =>
                {
                    logger.GetLogger(nameof(MongoConnectionConfig)).Debug($"{e.CommandName} - {e.Command.ToJson()}");
                });
            };

            return new()
            {
                DatabaseName = dbName,
                MongoClientSettings = mongoClientSettings
            };
        }

        //public void SetLogging(ILogger logger) =>
        //    MongoClientSettings.ClusterConfigurator = cb => {
        //        cb.Subscribe<CommandStartedEvent>(e => {
        //            logger.LogInformation($"{e.CommandName} - {e.Command.ToJson()}");
        //        });
        //    };
    }
}