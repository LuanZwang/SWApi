using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SWApi.Api.Controllers;
using SWApi.Application.Service.Interface;
using SWApi.Domain.Dto.Api.Commom;
using SWApi.Domain.Dto.Api.Planet;
using SWApi.Domain.Utils;
using SWApi.Test.TestUtils;

namespace SWApi.Test.Api.Controllers
{
    [TestClass]
    public class PlanetsControllerTest
    {
        private Mock<IPlanetService> _mockPlanetService;
        private PlanetsController _planetsController;

        [TestInitialize]
        public void Init()
        {
            _mockPlanetService = new();

            _planetsController = new(_mockPlanetService.Object);
        }

        [TestMethod]
        public void GetById_Should_Return_BadRequest_When_Param_Is_Empty()
        {
            var result = _planetsController.GetById(Guid.Empty);

            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(BadRequestObjectResult), result.GetType());

            var messageFromResult = ((BadRequestObjectResult)result).Value as string;

            Assert.IsNotNull(messageFromResult);
            Assert.AreEqual("Only valid GUIDs are allowed.", messageFromResult);

            _mockPlanetService.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Never);
            _mockPlanetService.Verify(x => x.GetAll(It.IsAny<int?>(), It.IsAny<int?>()), Times.Never);
            _mockPlanetService.Verify(x => x.Delete(It.IsAny<Guid>()), Times.Never);
            _mockPlanetService.Verify(x => x.GetByName(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void GetById_Should_Return_NotFound_When_Planet_Not_Found()
        {
            var id = Guid.NewGuid();

            _mockPlanetService.Setup(x => x.GetById(It.IsAny<Guid>())).Returns(default(PlanetDto));

            var result = _planetsController.GetById(id);

            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(NotFoundResult), result.GetType());

            _mockPlanetService.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Once);
            _mockPlanetService.Verify(x => x.GetById(id), Times.Once);

            _mockPlanetService.Verify(x => x.GetAll(It.IsAny<int?>(), It.IsAny<int?>()), Times.Never);
            _mockPlanetService.Verify(x => x.Delete(It.IsAny<Guid>()), Times.Never);
            _mockPlanetService.Verify(x => x.GetByName(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void GetById_Should_Return_OkHttpObject_PlanetDto()
        {
            var expectedPlanetResult = new PlanetDto
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Planet 123",
                Climate = "Planet 123 climate",
                Terrain = "Planet 123 Terrain",
                Films = new[]
                {
                new FilmDto
                {
                    Director = "Planet 123 Film Director",
                    Title = "Planet 123 Film Title",
                    ReleaseDate = "2022-01-05"
                }
            }
            };

            var id = Guid.NewGuid();

            _mockPlanetService.Setup(x => x.GetById(It.IsAny<Guid>())).Returns(expectedPlanetResult);

            var result = _planetsController.GetById(id);

            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(OkObjectResult), result.GetType());

            var planetFromResult = ((OkObjectResult)result).Value as PlanetDto;

            Assert.IsNotNull(planetFromResult);

            Assert.IsTrue(expectedPlanetResult.GetEnumeratedPlanetDtoProps().SequenceEqual(planetFromResult.GetEnumeratedPlanetDtoProps()));
            Assert.IsTrue(expectedPlanetResult.Films.First().GetEnumeratedFilmDtoProps().SequenceEqual(planetFromResult.Films.First().GetEnumeratedFilmDtoProps()));

            _mockPlanetService.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Once);
            _mockPlanetService.Verify(x => x.GetById(id), Times.Once);

            _mockPlanetService.Verify(x => x.GetAll(It.IsAny<int?>(), It.IsAny<int?>()), Times.Never);
            _mockPlanetService.Verify(x => x.Delete(It.IsAny<Guid>()), Times.Never);
            _mockPlanetService.Verify(x => x.GetByName(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("    ")]
        [DataRow(null)]
        public void GetByName_Should_Return_BadRequest_For_Empty_Or_Null_Parameter(string planetName)
        {
            var result = _planetsController.GetByName(planetName);

            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(BadRequestObjectResult), result.GetType());

            var messageFromResult = ((BadRequestObjectResult)result).Value as string;

            Assert.IsNotNull(messageFromResult);
            Assert.AreEqual("Planet name for search should be informed.", messageFromResult);

            _mockPlanetService.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Never);
            _mockPlanetService.Verify(x => x.GetAll(It.IsAny<int?>(), It.IsAny<int?>()), Times.Never);
            _mockPlanetService.Verify(x => x.Delete(It.IsAny<Guid>()), Times.Never);
            _mockPlanetService.Verify(x => x.GetByName(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(50)]
        public void GetByName_Should_Return_OkHttpObject_PlanetDto(int charsQuantity)
        {
            var param = new string('P', charsQuantity);

            var expectedPlanetResult = new PlanetDto
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Planet 123",
                Climate = "Planet 123 climate",
                Terrain = "Planet 123 Terrain",
                Films = new[]
                    {
                    new FilmDto
                    {
                        Director = "Planet 123 Film Director",
                        Title = "Planet 123 Film Title",
                        ReleaseDate = "2022-01-05"
                    }
            }
            };

            _mockPlanetService.Setup(x => x.GetByName(It.IsAny<string>())).Returns(new[] { expectedPlanetResult });

            var result = _planetsController.GetByName(param);

            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(OkObjectResult), result.GetType());

            var planetsFromResult = ((OkObjectResult)result).Value as IEnumerable<PlanetDto>;

            Assert.IsNotNull(planetsFromResult);

            var planetFromPlanets = planetsFromResult.First();

            Assert.IsTrue(expectedPlanetResult.GetEnumeratedPlanetDtoProps().SequenceEqual(planetFromPlanets.GetEnumeratedPlanetDtoProps()));
            Assert.IsTrue(expectedPlanetResult.Films.First().GetEnumeratedFilmDtoProps().SequenceEqual(planetFromPlanets.Films.First().GetEnumeratedFilmDtoProps()));

            _mockPlanetService.Verify(x => x.GetByName(It.IsAny<string>()), Times.Once);
            _mockPlanetService.Verify(x => x.GetByName(param), Times.Once);

            _mockPlanetService.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Never);
            _mockPlanetService.Verify(x => x.GetAll(It.IsAny<int?>(), It.IsAny<int?>()), Times.Never);
            _mockPlanetService.Verify(x => x.Delete(It.IsAny<Guid>()), Times.Never);
        }

        [TestMethod]
        public void GetByName_Should_Return_BadRequest_For_Parameter_Greater_Than_50_Characters()
        {
            var result = _planetsController.GetByName(new string('P', 51));

            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(BadRequestObjectResult), result.GetType());

            var messageFromResult = ((BadRequestObjectResult)result).Value as string;

            Assert.IsNotNull(messageFromResult);
            Assert.AreEqual("Planet name for search should not be greater than 50 characters.", messageFromResult);

            _mockPlanetService.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Never);
            _mockPlanetService.Verify(x => x.GetAll(It.IsAny<int?>(), It.IsAny<int?>()), Times.Never);
            _mockPlanetService.Verify(x => x.Delete(It.IsAny<Guid>()), Times.Never);
            _mockPlanetService.Verify(x => x.GetByName(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void GetAll_Should_Return_BadRequest_When_PageSize_Is_Greater_Than_100()
        {
            var result = _planetsController.GetAll(pageSize: 101, page: null);

            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(BadRequestObjectResult), result.GetType());

            var messageFromResult = ((BadRequestObjectResult)result).Value as string;

            Assert.IsNotNull(messageFromResult);
            Assert.AreEqual("Page size cannot be greater than 100.", messageFromResult);

            _mockPlanetService.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Never);
            _mockPlanetService.Verify(x => x.GetAll(It.IsAny<int?>(), It.IsAny<int?>()), Times.Never);
            _mockPlanetService.Verify(x => x.Delete(It.IsAny<Guid>()), Times.Never);
            _mockPlanetService.Verify(x => x.GetByName(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        [DataRow(null, null)]
        [DataRow(2, null)]
        [DataRow(5, 20)]
        [DataRow(null, 100)]
        public void GetAll_Should_Return_Page(int? page, int? pageSize)
        {
            (_, int actualPageSize) = PaginationUtils.GetRealPaginationValues(page, pageSize);

            var expectedFilmPlanetGetAllResult = new FilmDto
            {
                Director = "Planet 123 Film Director",
                Title = "Planet 123 Film Title",
                ReleaseDate = "2022-01-05"
            };

            var expectedPlanetGetAllResult = new PlanetDto
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Planet 123",
                Climate = "Planet 123 climate",
                Terrain = "Planet 123 Terrain",
                Films = new[] { expectedFilmPlanetGetAllResult }
            };

            var expectedGetAllResult = new GetAllDto<PlanetDto>
            {
                PageSize = actualPageSize,
                NextPage = 6,
                PreviousPage = 4,
                TotalCount = 95,
                Items = new[] { expectedPlanetGetAllResult }
            };

            _mockPlanetService.Setup(x => x.GetAll(It.IsAny<int?>(), It.IsAny<int?>())).Returns(expectedGetAllResult);

            var result = _planetsController.GetAll(page, pageSize);

            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(OkObjectResult), result.GetType());

            var getAllDtoFromResult = ((OkObjectResult)result).Value as GetAllDto<PlanetDto>;

            Assert.IsNotNull(getAllDtoFromResult);

            Assert.IsTrue(expectedGetAllResult.GetEnumeratedGetAllDtoProps().SequenceEqual(getAllDtoFromResult.GetEnumeratedGetAllDtoProps()));

            var planetDtoFromResult = getAllDtoFromResult.Items.First();

            Assert.IsTrue(expectedPlanetGetAllResult.GetEnumeratedPlanetDtoProps().SequenceEqual(planetDtoFromResult.GetEnumeratedPlanetDtoProps()));
            Assert.IsTrue(expectedFilmPlanetGetAllResult.GetEnumeratedFilmDtoProps().SequenceEqual(planetDtoFromResult.Films.First().GetEnumeratedFilmDtoProps()));

            _mockPlanetService.Verify(x => x.GetAll(It.IsAny<int?>(), It.IsAny<int?>()), Times.Once);
            _mockPlanetService.Verify(x => x.GetAll(page, pageSize), Times.Once);

            _mockPlanetService.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Never);
            _mockPlanetService.Verify(x => x.Delete(It.IsAny<Guid>()), Times.Never);
            _mockPlanetService.Verify(x => x.GetByName(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        public void Delete_Should_Return_BadRequest_When_Param_Is_Empty()
        {
            var result = _planetsController.Delete(Guid.Empty);

            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(BadRequestObjectResult), result.GetType());

            var messageFromResult = ((BadRequestObjectResult)result).Value as string;

            Assert.IsNotNull(messageFromResult);
            Assert.AreEqual("Only valid GUIDs are allowed.", messageFromResult);

            _mockPlanetService.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Never);
            _mockPlanetService.Verify(x => x.GetAll(It.IsAny<int?>(), It.IsAny<int?>()), Times.Never);
            _mockPlanetService.Verify(x => x.Delete(It.IsAny<Guid>()), Times.Never);
            _mockPlanetService.Verify(x => x.GetByName(It.IsAny<string>()), Times.Never);
        }

        [TestMethod]
        [DataRow(typeof(NoContentResult), true)]
        [DataRow(typeof(NotFoundResult), false)]
        public void Delete_Should_Return_Expected_Result(Type expectedTypeResult, bool serviceResult)
        {
            var id = Guid.NewGuid();

            _mockPlanetService.Setup(x => x.Delete(It.IsAny<Guid>())).Returns(serviceResult);

            var result = _planetsController.Delete(id);

            Assert.IsNotNull(result);
            Assert.AreEqual(expectedTypeResult, result.GetType());

            _mockPlanetService.Verify(x => x.Delete(It.IsAny<Guid>()), Times.Once);
            _mockPlanetService.Verify(x => x.Delete(id), Times.Once);

            _mockPlanetService.Verify(x => x.GetById(It.IsAny<Guid>()), Times.Never);
            _mockPlanetService.Verify(x => x.GetAll(It.IsAny<int?>(), It.IsAny<int?>()), Times.Never);
            _mockPlanetService.Verify(x => x.GetByName(It.IsAny<string>()), Times.Never);
        }
    }
}