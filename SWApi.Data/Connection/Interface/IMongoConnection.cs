using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace SWApi.Data.Connection.Interface;

public interface IMongoConnection
{
    IMongoCollection<T> GetCollection<T>(string collectionName, Action<IMongoCollection<T>> createCollectionIndexes = null);
}
