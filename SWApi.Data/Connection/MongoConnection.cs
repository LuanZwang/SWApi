using MongoDB.Driver;
using NLog;
using SWApi.Data.Connection.Interface;
using SWApi.Domain.Configuration.Logging;

namespace SWApi.Data.Connection
{
    public sealed class MongoConnection : IMongoConnection
    {
        private readonly ILogger _logger;
        private IMongoDatabase _mongoDatabase;
        private static readonly object SyncingDb = new();
        private readonly IMongoClient _client;
        private readonly IMongoConnectionConfig _config;

        public MongoConnection(ILogControl logger, IMongoConnectionConfig mongoConnectionConfig)
        {
            _logger = logger.GetLogger(GetType().Name);
            _config = mongoConnectionConfig;

            _client = mongoConnectionConfig.GetMongoClient();
        }

        public IMongoDatabase Db
        {
            get
            {
                if (_mongoDatabase is null) 
                {
                    lock (SyncingDb)
                    {
                        _mongoDatabase = Connect();
                    }
                }

                return _mongoDatabase;
            }
        }

        private IMongoDatabase Connect()
        {
            _logger.Debug("Attempting to connect to Mongo");
            return _client.GetDatabase(_config.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName, Action<IMongoCollection<T>> createCollectionIndexes = null)
        {
            if (!Db.ListCollectionNames().ToList().Any(x => x == collectionName))
                Db.CreateCollection(
                    name: collectionName,
                    options: new CreateCollectionOptions
                    {
                        Collation = new Collation(locale: "en", caseFirst: CollationCaseFirst.Off, strength: CollationStrength.Secondary)
                    });

            var collection = Db.GetCollection<T>(collectionName);

            createCollectionIndexes?.Invoke(collection);

            return collection;
        }
    }
}