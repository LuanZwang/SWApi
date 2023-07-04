using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Driver;
using Moq;
using NLog;
using SWApi.Data.Repository.Planet;
using SWApi.Domain.Configuration.Logging;
using SWApi.Domain.Planet;
using SWApi.Test.Data.MongoDb;
using SWApi.Test.TestUtils;

namespace SWApi.Test.Data.Repository
{
    [TestClass]
    public class PlanetRepositoryTest
    {
        private Mock<ILogger> _mockLogger;
        private Mock<ILogControl> _mockLogControl;
        private Mock<PlanetRepository> _mockPlanetRepository;
        private Mongo2GoConnection _connection;

        [TestInitialize]
        public void Init()
        {
            _mockLogger = new();
            _mockLogControl = new();
            _connection = new();

            _mockLogControl.Setup(x => x.GetLogger(It.IsAny<string>())).Returns(_mockLogger.Object);

            _mockPlanetRepository = new(_mockLogControl.Object, _connection, null) { CallBase = true };

            _mockPlanetRepository.Setup(x => x.CreateCollectionIndexes(
                    It.IsAny<IMongoCollection<Planet>>()))
                .Callback((IMongoCollection<Planet> collection) => collection.Indexes.DropAll());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _connection.Dispose();
        }

        [TestMethod]
        public void CreateCollectionIndexes_Should_Handle_Error()
        {
            var mockMongoConnection = new Mock<Mongo2GoConnection>();

            mockMongoConnection.Setup(x => x.GetCollection(It.IsAny<string>(), It.IsAny<Action<IMongoCollection<Planet>>>()))
                .Returns<string, Action<IMongoCollection<Planet>>>((dbName, action) =>
                {
                    var collection = _connection.Db.GetCollection<Planet>(dbName + "_test", null);

                    return collection;
                });

            _mockPlanetRepository = new(_mockLogControl.Object, mockMongoConnection.Object, null);

            Assert.IsNotNull(_mockPlanetRepository.Object.Collection.Indexes);
            Assert.AreEqual(0, _mockPlanetRepository.Object.Collection.Indexes.List().ToList().Count);

            _mockPlanetRepository.Setup(x => x.CreateCollectionIndexes(It.IsAny<IMongoCollection<Planet>>())).CallBase();

            _mockPlanetRepository.Object.CreateCollectionIndexes(null);

            Assert.IsNotNull(_mockPlanetRepository.Object.Collection.Indexes);
            Assert.AreEqual(0, _mockPlanetRepository.Object.Collection.Indexes.List().ToList().Count);

            _mockLogger.Verify(x => x.Info(It.IsAny<string>()), Times.Once);
            _mockLogger.Verify(x => x.Error(It.IsAny<string>()), Times.Once);
        }

        [TestMethod]
        public void CreateCollectionIndexes_Should_Create_Index()
        {
            _ = _mockPlanetRepository.Object.Collection;

            _mockPlanetRepository.Setup(x => x.CreateCollectionIndexes(
                    It.IsAny<IMongoCollection<Planet>>()))
                .CallBase();

            var collection = _connection.GetCollection<Planet>("foo_collection");

            _mockPlanetRepository.Object.CreateCollectionIndexes(collection);

            Assert.IsNotNull(collection.Indexes);
            Assert.AreEqual(1, collection.Indexes.List().ToList().Count);

            _mockLogger.Verify(x => x.Info(It.IsAny<string>()), Times.Once);
            _mockLogger.Verify(x => x.Error(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public async Task GetById_Should_Return_Planet()
        {
            var expectedResult = new Planet(Guid.NewGuid().ToString())
            {
                Name = "planet 0 name",
                Climate = "planet 0 climate",
                Terrain = "planet 0 terrain",
                Films = new[]
                    {
                    new Film
                    {
                        Title = "film 0 title",
                        Director = "film 0 director",
                        ReleaseDate = "2022-01-01"
                    }
                }
            };

            var planets = new List<Planet>
        {
            expectedResult,
            new Planet(Guid.NewGuid().ToString())
            {
                Name = "planet 1 name",
                Climate = "planet 1 climate",
                Terrain = "planet 1 terrain",
                Films = new[]
                {
                    new Film
                    {
                        Title = "film 1 title",
                        Director = "film 1 director",
                        ReleaseDate = "2022-01-00"
                    }
                }
            }
        };

            _mockPlanetRepository.Object.Collection.InsertMany(planets);

            var result = await _mockPlanetRepository.Object.GetById(planets[0].Id);

            Assert.IsNotNull(result);

            Assert.IsTrue(expectedResult.GetEnumeratedPlanetProps().SequenceEqual(result.GetEnumeratedPlanetProps()));
            Assert.IsTrue(expectedResult.Films.First().GetEnumeratedFilmProps().SequenceEqual(result.Films.First().GetEnumeratedFilmProps()));
        }

        [TestMethod]
        [DataRow("ea749f31-b15c-418e-b9af-74ce2c085f28", false)]
        [DataRow("ea749f31-b15c-418e-b9af-74ce2c085f29", true)]
        public async Task Remove_Should_Return_Expected_Result(string idToDelete, bool expectedResult)
        {
            var planets = new List<Planet>
        {
            new Planet("ea749f31-b15c-418e-b9af-74ce2c085f29")
            {
                Name = "planet 0 name",
                Climate = "planet 0 climate",
                Terrain = "planet 0 terrain",
                Films = new[]
                {
                    new Film
                    {
                        Title = "film 0 title",
                        Director = "film 0 director",
                        ReleaseDate = "2022-01-01"
                    }
                }
            },
            new Planet("ea749f31-b15c-418e-b9af-74ce2c085f27")
            {
                Name = "planet 1 name",
                Climate = "planet 1 climate",
                Terrain = "planet 1 terrain",
                Films = new[]
                {
                    new Film
                    {
                        Title = "film 1 title",
                        Director = "film 1 director",
                        ReleaseDate = "2022-01-00"
                    }
                }
            }
        };

            _mockPlanetRepository.Object.Collection.InsertMany(planets);

            var result = await _mockPlanetRepository.Object.Remove(idToDelete);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public async Task GetByName_Should_Return_Planets()
        {
            var planets = new List<Planet>
        {
            new Planet("ea749f31-b15c-418e-b9af-74ce2c085f29")
            {
                Name = "planet 0 name",
                Climate = "planet 0 climate",
                Terrain = "planet 0 terrain",
                Films = new[]
                {
                    new Film
                    {
                        Title = "film 0 title",
                        Director = "film 0 director",
                        ReleaseDate = "2022-01-02"
                    }
                }
            },
            new Planet("ea749f31-b15c-418e-b9af-74ce2c085f27")
            {
                Name = "planet 1 name",
                Climate = "planet 1 climate",
                Terrain = "planet 1 terrain",
                Films = new[]
                {
                    new Film
                    {
                        Title = "film 1 title",
                        Director = "film 1 director",
                        ReleaseDate = "2022-01-01"
                    }
                }
            },
            new Planet("ea749f31-b15c-418e-b9af-74ce2c085f25")
            {
                Name = "planet 0 NAME",
                Climate = "planet 2 climate",
                Terrain = "planet 2 terrain",
                Films = Array.Empty<Film>()
            }
        };

            _mockPlanetRepository.Object.Collection.InsertMany(planets);

            var result = await _mockPlanetRepository.Object.GetByName(planets[0].Name);

            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);

            var firstPlanetFromResult = result[0];
            var secondPlanetFromResult = result[1];

            Assert.IsTrue(planets[0].GetEnumeratedPlanetProps().SequenceEqual(firstPlanetFromResult.GetEnumeratedPlanetProps()));
            Assert.IsTrue(planets[0].Films.First().GetEnumeratedFilmProps().SequenceEqual(firstPlanetFromResult.Films.First().GetEnumeratedFilmProps()));

            Assert.IsTrue(planets[2].GetEnumeratedPlanetProps().SequenceEqual(secondPlanetFromResult.GetEnumeratedPlanetProps()));
        }

        [TestMethod]
        public void GetAllPaginated_Should_Return_Paginated_Values()
        {
            var expectedResult = new Planet(Guid.NewGuid().ToString())
            {
                Name = "planet 0 name",
                Climate = "planet 0 climate",
                Terrain = "planet 0 terrain",
                Films = new[]
                    {
                    new Film
                    {
                        Title = "film 0 title",
                        Director = "film 0 director",
                        ReleaseDate = "2022-01-01"
                    }
                }
            };

            var planets = new List<Planet>
            {
                expectedResult,
                new Planet(Guid.NewGuid().ToString())
                {
                    Name = "planet 1 name",
                    Climate = "planet 1 climate",
                    Terrain = "planet 1 terrain",
                    Films = new[]
                    {
                        new Film
                        {
                            Title = "film 1 title",
                            Director = "film 1 director",
                            ReleaseDate = "2022-01-00"
                        }
                    }
                }
            };

            _mockPlanetRepository.Object.Collection.InsertMany(planets);

            var page = 1;

            var result = _mockPlanetRepository.Object.GetAllPaginated(page, 1);

            Assert.IsNotNull(result);
            Assert.AreEqual(planets.Count, result.TotalCount);
            Assert.AreEqual(1, result.Items.Count);

            var planetFromResult = result.Items.First();

            Assert.IsTrue(planets[0].GetEnumeratedPlanetProps().SequenceEqual(planetFromResult.GetEnumeratedPlanetProps()));
            Assert.IsTrue(planets[0].Films.First().GetEnumeratedFilmProps().SequenceEqual(planetFromResult.Films.First().GetEnumeratedFilmProps()));
        }

        public class LoggerWrapper : Logger
        {
            public new virtual void Error(string message) { }
            public new virtual void Info(string message) { }
        }
    }
}