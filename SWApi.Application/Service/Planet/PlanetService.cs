using NLog;
using SWApi.Application.Service.Interface;
using SWApi.Data.Repository.Interface;
using SWApi.Domain.Configuration.Logging;
using SWApi.Domain.Dto.Api.Commom;
using SWApi.Domain.Dto.Api.Planet;
using SWApi.Domain.Planet;
using SWApi.Domain.Utils;

namespace SWApi.Application.Service.Planet
{

    public sealed class PlanetService : IPlanetService
    {
        private readonly IPlanetRepository _planetRepository;
        private readonly ILogger _logger;

        public PlanetService(
            ILogControl logger,
            IPlanetRepository planetRepository)
        {
            _logger = logger.GetLogger(GetType().Name);
            _planetRepository = planetRepository;
        }

        public bool Delete(Guid id)
        {
            _logger.Info($"Method: Delete");

            if (id == Guid.Empty)
                return false;

            return _planetRepository.Remove(id.ToString());
        }

        public GetAllDto<PlanetDto> GetAll(int? page, int? pageSize)
        {
            _logger.Info($"Method: GetAll - page: {page} - pageSize: {pageSize}");

            (int actualPage, int actualPageSize) = PaginationUtils.GetRealPaginationValues(page, pageSize);

            (long totalCount, List<Domain.Planet.Planet> planets) = _planetRepository.GetAllPaginated(
                page: actualPage,
                pageSize: actualPageSize);

            (int? previousPage, int? nextPage) = PaginationUtils.GetPreviousAndNextPages(
                totalCount: totalCount,
                page: actualPage,
                pageSize: actualPageSize);

            var planetsDtos = ConvertToPlanetDtos(planets);

            return new GetAllDto<PlanetDto>
            {
                PageSize = planets.Count,
                TotalCount = totalCount,
                NextPage = nextPage,
                PreviousPage = previousPage,
                Items = planetsDtos
            };
        }

        public PlanetDto GetById(Guid id)
        {
            _logger.Info($"Method: GetById");

            if (id == Guid.Empty)
                return default;

            var planet = _planetRepository.GetById(id.ToString());

            return ConvertToPlanetDto(planet);
        }

        public IEnumerable<PlanetDto> GetByName(string name)
        {
            _logger.Info($"Method: GetByName");

            if (string.IsNullOrWhiteSpace(name))
                return Enumerable.Empty<PlanetDto>();

            var planets = _planetRepository.GetByName(name);

            return ConvertToPlanetDtos(planets);
        }

        private static IEnumerable<FilmDto> ConvertToFilmDtos(IEnumerable<Film> planetFilms)
        {
            if (planetFilms is null)
                yield break;

            foreach (var film in planetFilms)
                yield return new FilmDto
                {
                    Title = film.Title,
                    Director = film.Director,
                    ReleaseDate = film.ReleaseDate
                };
        }

        private static IEnumerable<PlanetDto> ConvertToPlanetDtos(List<Domain.Planet.Planet> planets)
        {
            if (!planets.Any())
                yield break;

            foreach (var planet in planets)
                yield return ConvertToPlanetDto(planet);
        }

        private static PlanetDto ConvertToPlanetDto(Domain.Planet.Planet planet)
        {
            if (planet is null)
                return default;

            return new PlanetDto
            {
                Id = planet.Id,
                Name = planet.Name,
                Climate = planet.Climate,
                Terrain = planet.Terrain,
                Films = ConvertToFilmDtos(planet.Films)
            };
        }
    }
}