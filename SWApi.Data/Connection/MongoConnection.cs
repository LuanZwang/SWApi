using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using SWApi.Data.Connection.Interface;
using System.Runtime;

namespace SWApi.Data.Connection;    

public sealed class MongoConnection : IMongoConnection
{
    private readonly ILogger<MongoConnection> _logger;
    private IMongoDatabase _mongoDatabase;
    private static readonly object SyncingDb = new object();
    private readonly IMongoClient _client;
    private readonly MongoConnectionConfig _config;
    
    public MongoConnection(ILogger<MongoConnection> logger, MongoConnectionConfig mongoConnectionConfig)
    {
        _logger = logger;
        _config = mongoConnectionConfig;

        _client = new MongoClient(_config.MongoClientSettings);
    }

    public IMongoDatabase Db
    {
        get
        {
            if (_mongoDatabase != null)
            {
                return _mongoDatabase;
            }

            lock (SyncingDb)
            {
                _mongoDatabase ??= Connect();
            }

            return _mongoDatabase;
        }
    }

    private IMongoDatabase Connect()
    {
        _logger.LogDebug("Attempting to connect to Mongo");
        return _client.GetDatabase(_config.DatabaseName);
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName, Action<IMongoCollection<T>> createCollectionIndexes = null)
    {
        IMongoCollection<T> collection = Db.GetCollection<T>(collectionName);
        createCollectionIndexes?.Invoke(collection);
        
        return collection;
    }
}