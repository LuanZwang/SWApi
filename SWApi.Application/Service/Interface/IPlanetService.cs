using SWApi.Domain.Dto.Api.Commom;
using SWApi.Domain.Dto.Api.Planet;

namespace SWApi.Application.Service.Interface
{
    public interface IPlanetService
    {
        PlanetDto GetById(Guid id);
        GetAllDto<PlanetDto> GetAll(int? page, int? pageSize);
        bool Delete(Guid id);
        IEnumerable<PlanetDto> GetByName(string name);
    }
}