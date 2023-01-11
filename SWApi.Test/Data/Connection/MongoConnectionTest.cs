using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using NLog;
using SWApi.Data.Connection;
using SWApi.Data.Connection.Interface;
using SWApi.Domain.Configuration.Logging;
using SWApi.Domain.Planet;
using SWApi.Test.Data.MongoDb;

namespace SWApi.Test.Data.Connection
{
    [TestClass]
    public class MongoConnectionTest
    {
        private Mongo2GoConnection _connection;
        private Mock<IMongoConnectionConfig> _mockMongoConnectionConfig;
        private Mock<IMongoClient> _mockMongoClient;
        private Mock<ILogControl> _mockLogControl;
        private Mock<ILogger> _mockLogger;
        private MongoConnection _mongoConnection;

        [TestInitialize]
        public void Init()
        {
            _connection = new();
            _mockMongoConnectionConfig = new();
            _mockLogControl = new();
            _mockLogger = new();

            _mockLogControl.Setup(x => x.GetLogger(It.IsAny<string>())).Returns(_mockLogger.Object);

            _mockMongoClient = new();

            _mockMongoConnectionConfig.Setup(x => x.GetMongoClient()).Returns(_mockMongoClient.Object);

            _mongoConnection = new(_mockLogControl.Object, _mockMongoConnectionConfig.Object);

        }

        [TestMethod]
        public void Accessing_Db_For_First_Time_Should_Set_New_Client()
        {
            var db = _connection.Db;

            _mockMongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>())).Returns(db);
            
            _ = _mongoConnection.Db.Settings;

            _mockLogger.Verify(x => x.Debug(It.IsAny<string>()), Times.Once);
            _mockMongoClient.Verify(x => x.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>()), Times.Once);

            Assert.AreEqual(db, _connection.Db);
        }

        [TestMethod]
        public void GetCollection_Should_Return_Already_Existing_Collection()
        {
            var dbName = "test";

            var collection = _connection.GetCollection<Planet>(dbName, null);

            var mockMongoDatabase = new Mock<IMongoDatabase>();

            _mockMongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>())).Returns(mockMongoDatabase.Object);

            mockMongoDatabase.Setup(x => x.GetCollection<Planet>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>())).Returns(collection);
            
            mockMongoDatabase.Setup(x => x.ListCollectionNames(
                    It.IsAny<ListCollectionNamesOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(_connection.Db.ListCollectionNames());

            _mongoConnection = new(_mockLogControl.Object, _mockMongoConnectionConfig.Object);

            var mockAction = new Mock<Action<IMongoCollection<Planet>>>();

            var result = _mongoConnection.GetCollection(dbName, mockAction.Object);

            Assert.AreEqual(collection, result);

            mockMongoDatabase.Verify(x => x.GetCollection<Planet>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()), Times.Once);
            mockMongoDatabase.Verify(x => x.GetCollection<Planet>(dbName, null), Times.Once);

            mockAction.Verify(x => x.Invoke(It.IsAny<IMongoCollection<Planet>>()), Times.Once);
            mockAction.Verify(x => x.Invoke(collection), Times.Once);
        }

        [TestMethod]
        public void GetCollection_Should_Return_New_Collection()
        {
            var dbName = "test_123";
            var newDbName = dbName + "4";

            var mockMongoDatabase = new Mock<IMongoDatabase>();

            mockMongoDatabase.Setup(x => x.GetCollection<Planet>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>())).Returns(_connection.GetCollection<Planet>(dbName, null));

            mockMongoDatabase.Setup(x => x.ListCollectionNames(
                    It.IsAny<ListCollectionNamesOptions>(),
                    It.IsAny<CancellationToken>()))
                .Returns(_connection.Db.ListCollectionNames());

            _mockMongoClient.Setup(x => x.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>())).Returns(mockMongoDatabase.Object);

            _mongoConnection = new(_mockLogControl.Object, _mockMongoConnectionConfig.Object);

            var result = _mongoConnection.GetCollection<Planet>(newDbName);

            var collection = _connection.GetCollection<Planet>(dbName);

            Assert.AreEqual(collection.CollectionNamespace.CollectionName, result.CollectionNamespace.CollectionName);

            mockMongoDatabase.Verify(x => x.CreateCollection(
                    It.IsAny<string>(),
                    It.IsAny<CreateCollectionOptions>(),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            mockMongoDatabase.Verify(x => x.CreateCollection(
                    newDbName,
                    It.Is<CreateCollectionOptions>(
                        x => x.Collation.Locale == "en"
                        && x.Collation.CaseFirst == CollationCaseFirst.Off
                        && x.Collation.Strength == CollationStrength.Secondary),
                    default),
                Times.Once);
        }
    }
}