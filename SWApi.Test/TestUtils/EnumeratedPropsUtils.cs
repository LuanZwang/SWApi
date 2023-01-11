using SWApi.Domain.Dto.Api.Commom;
using SWApi.Domain.Dto.Api.Planet;
using SWApi.Domain.Planet;

namespace SWApi.Test.TestUtils
{
    public static class EnumeratedPropsUtils
    {
        public static IEnumerable<object> GetEnumeratedPlanetProps(this Planet planet)
        {
            yield return planet.Id;
            yield return planet.Name;
            yield return planet.Climate;
            yield return planet.Terrain;
        }

        public static IEnumerable<object> GetEnumeratedFilmProps(this Film film)
        {
            yield return film.Director;
            yield return film.Title;
            yield return film.ReleaseDate;
        }

        public static IEnumerable<object> GetEnumeratedPlanetDtoProps(this PlanetDto planetDto)
        {
            yield return planetDto.Id;
            yield return planetDto.Name;
            yield return planetDto.Climate;
            yield return planetDto.Terrain;
        }

        public static IEnumerable<object> GetEnumeratedFilmDtoProps(this FilmDto filmDto)
        {
            yield return filmDto.Director;
            yield return filmDto.Title;
            yield return filmDto.ReleaseDate;
        }

        public static IEnumerable<object> GetEnumeratedGetAllDtoProps<T>(this GetAllDto<T> getAllDto)
        {
            yield return getAllDto.PageSize;
            yield return getAllDto.NextPage;
            yield return getAllDto.PreviousPage;
            yield return getAllDto.TotalCount;
        }
    }
}