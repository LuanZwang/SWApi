using SWApi.Application.Service.Interface;
using SWApi.Data.Repository.Interface;
using SWApi.Domain.Dto.Api.Commom;
using SWApi.Domain.Dto.Api.Planet;
using SWApi.Domain.Planet;
using SWApi.Domain.Utils;

namespace SWApi.Application.Service.Planet;

public sealed class PlanetService : IPlanetService
{
    private readonly IPlanetRepository _planetRepository;

    public PlanetService(
        IPlanetRepository planetRepository)
    {
        _planetRepository = planetRepository;
    }

    public bool Delete(Guid id)
    {
        if (id == Guid.Empty)
            return false;

        return _planetRepository.Remove(id.ToString());
    }

    public GetAllDto<PlanetDto> GetAll(int? page, int? pageSize)
    {
        (int actualPage, int actualPageSize) = PaginationUtils.GetRealPaginationValues(page, pageSize);

        var planets = _planetRepository.GetAllPaginated(
            page: actualPage,
            pageSize: actualPageSize);

        var totalPages = (int)Math.Ceiling(((double)planets.TotalCount / actualPageSize));

        int? nextPage = actualPage + 1 > totalPages ? null : actualPage + 1;
        int? previousPage = actualPage == 1 ? null : actualPage - 1;

        var result = planets.Planets.Select(ConvertToPlanetDto);

        return new GetAllDto<PlanetDto>
        {
            PageSize = planets.Planets.Count,
            TotalCount = planets.TotalCount,
            NextPage = nextPage,
            PreviousPage = previousPage,
            Items = result
        };
    }

    public PlanetDto GetById(Guid id)
    {
        var planet = _planetRepository.GetById(id.ToString());

        return ConvertToPlanetDto(planet);
    }

    public PlanetDto GetByName(string name)
    {
        var planet = _planetRepository.GetByName(name);

        return ConvertToPlanetDto(planet);
    }

    private PlanetDto ConvertToPlanetDto(Domain.Planet.Planet planet)
    {
         return new PlanetDto
         {
             Id = planet.Id,
             Name = planet.Name,
             Climate = planet.Climate,
             Terrain = planet.Terrain,
             Films = ConvertToFilmsDto(planet.Films)
         };
    }

    private IEnumerable<FilmDto> ConvertToFilmsDto(IEnumerable<Film> planetFilms)
    {
        foreach (var film in planetFilms)
            yield return new FilmDto
            {
                Title = film.Title,
                Director = film.Director,
                ReleaseDate = film.ReleaseDate
            };
    }
}