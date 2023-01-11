using Mongo2Go;
using MongoDB.Driver;
using SWApi.Data.Connection.Interface;

namespace SWApi.Test.Data.MongoDb
{
    public class Mongo2GoConnection : IMongoConnection, IDisposable
    {
        private readonly string _databaseName = "database_test";
        private readonly MongoDbRunner _runner;
        private IMongoDatabase _database;

        public MongoClientSettings Settings { get; }
        public IMongoDatabase Db
        {
            get
            {
                _database ??= Connect();

                return _database;
            }
        }

        public Mongo2GoConnection()
        {
            _runner = MongoDbRunner.Start(binariesSearchDirectory: Mongo2GoSettings.GetBinariesSearchDirectory());
        }

        public virtual IMongoDatabase Connect()
        {
            return new MongoClient(_runner.ConnectionString).GetDatabase(_databaseName);
        }

        public virtual IMongoCollection<T> GetCollection<T>(string collectionName, Action<IMongoCollection<T>> createCollectionIndexes = null)
        {
            Db.DropCollection(collectionName);

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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                _runner?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}