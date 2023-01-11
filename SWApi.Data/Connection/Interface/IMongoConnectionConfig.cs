using MongoDB.Driver;

namespace SWApi.Data.Connection.Interface
{
    public interface IMongoConnectionConfig
    {
        public string DatabaseName { get; init; }
        IMongoClient GetMongoClient();
    }
}