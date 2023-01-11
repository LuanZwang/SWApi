using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using SWApi.Application.Service.Planet;
using SWApi.Data.Repository.Interface;
using SWApi.Domain.Configuration.Logging;
using SWApi.Domain.Planet;
using SWApi.Domain.Utils;
using SWApi.Test.TestUtils;

namespace SWApi.Test.Application.Service
{
    [TestClass]
    public class PlanetServiceTest
    {
        private Mock<ILogger> _mockLogger;
        private Mock<IPlanetRepository> _mockPlanetRepository;
        private PlanetService _planetService;

        [TestInitialize]
        public void Init()
        {
            _mockLogger = new();
            
            var mockLogControl = new Mock<ILogControl>();
            mockLogControl.Setup(x => x.GetLogger(It.IsAny<string>())).Returns(_mockLogger.Object);

            _mockPlanetRepository = new();

            _planetService = new(mockLogControl.Object, _mockPlanetRepository.Object);
        }

        [TestMethod]
        public void Delete_Should_Return_False_When_Parameter_Is_Empty()
        {
            Assert.IsFalse(_planetService.Delete(Guid.Empty));
            
            _mockLogger.Verify(x => x.Info(It.IsAny<string>()), Times.Once);

            _mockPlanetRepository.Verify(x => x.Remove(It.IsAny<string>()), Times.Never);
            _mockPlanetRepository.Verify(x => x.GetById(It.IsAny<string>()), Times.Never);
            _mockPlanetRepository.Verify(x => x.GetByName(It.IsAny<string>()), Times.Never);
            _mockPlanetRepository.Verify(x => x.GetAllPaginated(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        [DataRow(false)]
        [DataRow(true)]
        public void Delete_Should_Return_Expected_Result(bool expectedResult)
        {
            var id = Guid.NewGuid();

            _mockPlanetRepository.Setup(x => x.Remove(It.IsAny<string>())).Returns(expectedResult);

            Assert.AreEqual(expectedResult, _planetService.Delete(id));

            _mockLogger.Verify(x => x.Info(It.IsAny<string>()), Times.Once);

            _mockPlanetRepository.Verify(x => x.Remove(It.IsAny<string>()), Times.Once);
            _mockPlanetRepository.Verify(x => x.Remove(id.ToString()), Times.Once);

            _mockPlanetRepository.Verify(x => x.GetById(It.IsAny<string>()), Times.Never);
            _mockPlanetRepository.Verify(x => x.GetByName(It.IsAny<string>()), Times.Never);
            _mockPlanetRepository.Verify(x => x.GetAllPaginated(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void GetById_Should_Return_Default_When_Parameter_Is_Empty()
        {
            var result = _planetService.GetById(Guid.Empty);

            Assert.IsNull(result);

            _mockLogger.Verify(x => x.Info(It.IsAny<string>()), Times.Once);

            _mockPlanetRepository.Verify(x => x.Remove(It.IsAny<string>()), Times.Never);
            _mockPlanetRepository.Verify(x => x.GetById(It.IsAny<string>()), Times.Never);
            _mockPlanetRepository.Verify(x => x.GetByName(It.IsAny<string>()), Times.Never);
            _mockPlanetRepository.Verify(x => x.GetAllPaginated(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void GetById_Should_Return_Default_When_Planet_Not_Found()
        {
            var id = Guid.NewGuid();

            _mockPlanetRepository.Setup(x => x.GetById(It.IsAny<string>())).Returns(default(Planet));

            var result = _planetService.GetById(id);

            Assert.IsNull(result);

            _mockLogger.Verify(x => x.Info(It.IsAny<string>()), Times.Once);

            _mockPlanetRepository.Verify(x => x.GetById(It.IsAny<string>()), Times.Once);
            _mockPlanetRepository.Verify(x => x.GetById(id.ToString()), Times.Once);

            _mockPlanetRepository.Verify(x => x.Remove(It.IsAny<string>()), Times.Never);
            _mockPlanetRepository.Verify(x => x.GetByName(It.IsAny<string>()), Times.Never);
            _mockPlanetRepository.Verify(x => x.GetAllPaginated(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void GetById_Should_Return_PlanetDto()
        {
            var film = new Film
            {
                Director = "Planet 123 Film Director",
                Title = "Planet 123 Film Title",
                ReleaseDate = "2022-01-05"
            };

            var id = Guid.NewGuid();

            var planet = new Planet(id.ToString())
            {
                Name = "Planet 123",
                Climate = "Planet 123 climate",
                Terrain = "Planet 123 Terrain",
                Films = new[] { film }
            };

            _mockPlanetRepository.Setup(x => x.GetById(It.IsAny<string>())).Returns(planet);

            var result = _planetService.GetById(id);

            Assert.IsNotNull(result);

            Assert.IsTrue(planet.GetEnumeratedPlanetProps().SequenceEqual(result.GetEnumeratedPlanetDtoProps()));
            Assert.IsTrue(film.GetEnumeratedFilmProps().SequenceEqual((result.Films.First().GetEnumeratedFilmDtoProps())));

            _mockLogger.Verify(x => x.Info(It.IsAny<string>()), Times.Once);

            _mockPlanetRepository.Verify(x => x.GetById(It.IsAny<string>()), Times.Once);
            _mockPlanetRepository.Verify(x => x.GetById(id.ToString()), Times.Once);

            _mockPlanetRepository.Verify(x => x.Remove(It.IsAny<string>()), Times.Never);
            _mockPlanetRepository.Verify(x => x.GetByName(It.IsAny<string>()), Times.Never);
            _mockPlanetRepository.Verify(x => x.GetAllPaginated(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("    ")]
        [DataRow(null)]
        public void GetByName_Should_Return_Empty_Enumerable_When_Parameter_Is_Null_Or_White_Space(string planetName)
        {
            var result = _planetService.GetByName(planetName);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Any());

            _mockLogger.Verify(x => x.Info(It.IsAny<string>()), Times.Once);

            _mockPlanetRepository.Verify(x => x.Remove(It.IsAny<string>()), Times.Never);
            _mockPlanetRepository.Verify(x => x.GetById(It.IsAny<string>()), Times.Never);
            _mockPlanetRepository.Verify(x => x.GetByName(It.IsAny<string>()), Times.Never);
            _mockPlanetRepository.Verify(x => x.GetAllPaginated(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void GetByName_Should_Return_PlanetDto()
        {
            var planetName = "Planet 123";

            var planet = new Planet("planet_id")
            {
                Name = planetName,
                Climate = "Planet 123 climate",
                Terrain = "Planet 123 Terrain",
            };

            var noFilmPlanet = new Planet("planet_id")
            {
                Name = "Planet 1234",
                Climate = "Planet 1234 climate",
                Terrain = "Planet 1234 Terrain",
                Films = Array.Empty<Film>()
            };

            _mockPlanetRepository.Setup(x => x.GetByName(It.IsAny<string>())).Returns(new List<Planet> { planet, noFilmPlanet });

            var result = _planetService.GetByName(planetName).ToList();

            Assert.IsNotNull(result);

            var firstPlanetFromResult = result.First();

            Assert.IsTrue(planet.GetEnumeratedPlanetProps().SequenceEqual(firstPlanetFromResult.GetEnumeratedPlanetDtoProps()));
            Assert.AreEqual(0, firstPlanetFromResult.Films.Count());

            var secondPlanetFromResult = result[1];
            
            Assert.IsTrue(noFilmPlanet.GetEnumeratedPlanetProps().SequenceEqual(secondPlanetFromResult.GetEnumeratedPlanetDtoProps()));
            Assert.AreEqual(0, secondPlanetFromResult.Films.Count());

            _mockLogger.Verify(x => x.Info(It.IsAny<string>()), Times.Once);

            _mockPlanetRepository.Verify(x => x.GetByName(It.IsAny<string>()), Times.Once);
            _mockPlanetRepository.Verify(x => x.GetByName(planetName), Times.Once);

            _mockPlanetRepository.Verify(x => x.Remove(It.IsAny<string>()), Times.Never);
            _mockPlanetRepository.Verify(x => x.GetById(It.IsAny<string>()), Times.Never);
            _mockPlanetRepository.Verify(x => x.GetAllPaginated(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [TestMethod]
        public void GetAll_Should_Return_GetAllDto_When_Pagination_Values_Are_Null()
        {
            var totalCount = 2;

            var firstPlanetFilm = new Film
            {
                Title = "film1 title",
                Director = "film1 director",
                ReleaseDate = "2022-01-01"
            };

            var firstPlanet = new Planet("planet_id")
            {
                Name = "planet name",
                Climate = "planet climate",
                Terrain = "planet terrain",
                Films = new[] { firstPlanetFilm }
            };

            var secondPlanetFilm = new Film
            {
                Title = "film1 title",
                Director = "film1 director",
                ReleaseDate = "2022-01-02"
            };

            var secondPlanet = new Planet("planet1_id")
            {
                Name = "planet1 name",
                Climate = "planet1 climate",
                Terrain = "planet1 terrain",
                Films = new[] { secondPlanetFilm }
            };

            var planets = new List<Planet>
        {
            firstPlanet,
            secondPlanet
        };

            _mockPlanetRepository.Setup(x => x.GetAllPaginated(It.IsAny<int>(), It.IsAny<int>())).Returns((totalCount, planets));

            var result = _planetService.GetAll(null, null);

            Assert.IsNotNull(result);

            Assert.AreEqual(planets.Count, result.PageSize);
            Assert.AreEqual(totalCount, result.TotalCount);
            Assert.IsNull(result.NextPage);
            Assert.IsNull(result.PreviousPage);
            Assert.AreEqual(totalCount, result.Items.Count());

            var firstGetAllPlanet = result.Items.First();

            Assert.IsTrue(firstPlanet.GetEnumeratedPlanetProps().SequenceEqual(firstGetAllPlanet.GetEnumeratedPlanetDtoProps()));
            Assert.IsTrue(firstPlanet.Films.First().GetEnumeratedFilmProps().SequenceEqual(firstGetAllPlanet.Films.First().GetEnumeratedFilmDtoProps()));

            var secondGetAllPlanet = result.Items.ToArray()[1];

            Assert.IsTrue(secondPlanet.GetEnumeratedPlanetProps().SequenceEqual(secondGetAllPlanet.GetEnumeratedPlanetDtoProps()));
            Assert.IsTrue(secondPlanet.Films.First().GetEnumeratedFilmProps().SequenceEqual(secondGetAllPlanet.Films.First().GetEnumeratedFilmDtoProps()));

            _mockLogger.Verify(x => x.Info(It.IsAny<string>()), Times.Once);

            _mockPlanetRepository.Verify(x => x.GetAllPaginated(It.IsAny<int?>(), It.IsAny<int?>()), Times.Once);
            _mockPlanetRepository.Verify(x => x.GetAllPaginated(
                    PaginationUtils.DefaultPageIndex,
                    PaginationUtils.DefaultPageSize),
                Times.Once);

            _mockPlanetRepository.Verify(x => x.Remove(It.IsAny<string>()), Times.Never);
            _mockPlanetRepository.Verify(x => x.GetByName(It.IsAny<string>()), Times.Never);
            _mockPlanetRepository.Verify(x => x.GetById(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void GetAll_Should_Return_GetAllDto_When_Pagination_Values_Are_Not_Null()
        {
            var firstPlanetFilm = new Film
            {
                Title = "film1 title",
                Director = "film1 director",
                ReleaseDate = "2022-01-01"
            };

            var firstPlanet = new Planet("planet_id")
            {
                Name = "planet name",
                Climate = "planet climate",
                Terrain = "planet terrain",
                Films = new[] { firstPlanetFilm }
            };

            var secondPlanetFilm = new Film
            {
                Title = "film1 title",
                Director = "film1 director",
                ReleaseDate = "2022-01-02"
            };

            var secondPlanet = new Planet("planet1_id")
            {
                Name = "planet1 name",
                Climate = "planet1 climate",
                Terrain = "planet1 terrain",
                Films = new[] { secondPlanetFilm }
            };

            var planets = new List<Planet>
        {
            firstPlanet,
            secondPlanet
        };

            var totalCount = 40;
            var page = 20;
            var previousPage = 19;
            var pageSize = 2;

            _mockPlanetRepository.Setup(x => x.GetAllPaginated(It.IsAny<int>(), It.IsAny<int>())).Returns((totalCount, planets));

            var result = _planetService.GetAll(page, pageSize);

            Assert.IsNotNull(result);

            Assert.AreEqual(planets.Count, result.PageSize);
            Assert.AreEqual(totalCount, result.TotalCount);
            Assert.IsNull(result.NextPage);
            Assert.AreEqual(previousPage, result.PreviousPage);
            Assert.AreEqual(pageSize, result.Items.Count());

            var firstGetAllPlanet = result.Items.First();

            Assert.IsTrue(firstPlanet.GetEnumeratedPlanetProps().SequenceEqual(firstGetAllPlanet.GetEnumeratedPlanetDtoProps()));
            Assert.IsTrue(firstPlanet.Films.First().GetEnumeratedFilmProps().SequenceEqual(firstGetAllPlanet.Films.First().GetEnumeratedFilmDtoProps()));

            var secondGetAllPlanet = result.Items.ToArray()[1];

            Assert.IsTrue(secondPlanet.GetEnumeratedPlanetProps().SequenceEqual(secondGetAllPlanet.GetEnumeratedPlanetDtoProps()));
            Assert.IsTrue(secondPlanet.Films.First().GetEnumeratedFilmProps().SequenceEqual(secondGetAllPlanet.Films.First().GetEnumeratedFilmDtoProps()));

            _mockLogger.Verify(x => x.Info(It.IsAny<string>()), Times.Once);

            _mockPlanetRepository.Verify(x => x.GetAllPaginated(It.IsAny<int?>(), It.IsAny<int?>()), Times.Once);
            _mockPlanetRepository.Verify(x => x.GetAllPaginated(
                    page,
                    pageSize),
                Times.Once);

            _mockPlanetRepository.Verify(x => x.Remove(It.IsAny<string>()), Times.Never);
            _mockPlanetRepository.Verify(x => x.GetByName(It.IsAny<string>()), Times.Never);
            _mockPlanetRepository.Verify(x => x.GetById(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        [DataRow(400, 20, 19)]
        [DataRow(0, 1, null)]
        [DataRow(0, 0, null)]
        [DataRow(0, -1, null)]
        public void GetAll_Should_Return_GetAllDto_With_No_Values_When_No_Planets_Found(int totalCount, int page, int? previousPage)
        {
            var planets = new List<Planet>();
            var pageSize = 60;

            _mockPlanetRepository.Setup(x => x.GetAllPaginated(It.IsAny<int>(), It.IsAny<int>())).Returns((totalCount, planets));

            var result = _planetService.GetAll(page, pageSize);

            Assert.IsNotNull(result);

            Assert.AreEqual(0, result.PageSize);
            Assert.AreEqual(totalCount, result.TotalCount);
            Assert.IsNull(result.NextPage);
            Assert.AreEqual(previousPage, result.PreviousPage);
            Assert.AreEqual(0, result.Items.Count());

            var expectedCalledPage = page < 1 ? 1 : page;

            _mockLogger.Verify(x => x.Info(It.IsAny<string>()), Times.Once);

            _mockPlanetRepository.Verify(x => x.GetAllPaginated(It.IsAny<int?>(), It.IsAny<int?>()), Times.Once);
            _mockPlanetRepository.Verify(x => x.GetAllPaginated(
                    expectedCalledPage,
                    pageSize),
                Times.Once);

            _mockPlanetRepository.Verify(x => x.Remove(It.IsAny<string>()), Times.Never);
            _mockPlanetRepository.Verify(x => x.GetByName(It.IsAny<string>()), Times.Never);
            _mockPlanetRepository.Verify(x => x.GetById(It.IsAny<string>()), Times.Never);
        }
    }
}