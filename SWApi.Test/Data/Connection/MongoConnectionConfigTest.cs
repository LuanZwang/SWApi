using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SWApi.Data.Connection;
using SWApi.Domain.Configuration.Logging;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace SWApi.Test.Data.Connection
{
    [TestClass]
    public class MongoConnectionConfigTest
    {
        private IConfiguration _configuration;
        private Mock<ILogControl> _mockLogControl;
        
        private const string _user = "user_admin";
        private const string _password = "password_admin";
        private const string _host = "127.0.0.1";
        private const int _port = 27017;
        
        private const string _databaseName = "star-wars";

        [TestInitialize]
        public void Init()
        {
            var inMemorySettings = new Dictionary<string, string>
            {
                {"ConnectionStrings:MONGODB_CONNECTIONSTRING", $"mongodb://{_user}:{_password}@{_host}:{_port}" },
                { "MONGODB_DATABASE_NAME", _databaseName }
            };

            _configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

            _mockLogControl = new();
        }

        [TestMethod]
        public void ConfigureFromEnvs_Should_Return_New_Instance_Of_MongoConnectionConfig()
        {
            var config = MongoConnectionConfig.ConfigureFromEnvs(_configuration, _mockLogControl.Object);

            Assert.IsNotNull(config);

            Assert.AreEqual(_databaseName, config.DatabaseName);
        }

        [TestMethod]
        public void GetMongoClient_Should_Return_Always_Same_Instance_Of_MongoClient()
        {
            var config = MongoConnectionConfig.ConfigureFromEnvs(_configuration, _mockLogControl.Object);

            Assert.IsTrue(ReferenceEquals(config.GetMongoClient(), config.GetMongoClient()));
        }
    }
}