using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using SWApi.Data.Connection.Interface;
using SWApi.Domain.Configuration.Logging;

namespace SWApi.Data.Connection
{
    public class MongoConnectionConfig : IMongoConnectionConfig
    {
        public string DatabaseName { get; init; }
        private MongoClientSettings MongoClientSettings { get; init; }
        private IMongoClient MongoClient { get; init; }

        public static MongoConnectionConfig ConfigureFromEnvs(IConfiguration configuration, ILogControl logger)
        {
            var connStr = configuration.GetConnectionString("MONGODB_CONNECTIONSTRING");
            var dbName = configuration["MONGODB_DATABASE_NAME"];

            ArgumentException.ThrowIfNullOrEmpty(connStr);
            ArgumentException.ThrowIfNullOrEmpty(dbName);

            var mongoClientSettings = MongoClientSettings.FromUrl(new MongoUrl(connStr));

            mongoClientSettings.ClusterConfigurator = cb =>
            {
                cb.Subscribe<CommandStartedEvent>(e =>
                {
                    logger.GetLogger(nameof(MongoConnectionConfig)).Debug($"{e.CommandName} - {e.Command.ToJson()}");
                });
            };

            return new()
            {
                DatabaseName = dbName,
                MongoClientSettings = mongoClientSettings,
                MongoClient = new MongoClient(mongoClientSettings)
            };
        }

        public IMongoClient GetMongoClient() => MongoClient;
    }
}