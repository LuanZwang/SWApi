using SWApi.Domain.Dto.Api.Commom;
using SWApi.Domain.Dto.Api.Planet;

namespace SWApi.Application.Service.Interface
{
    public interface IPlanetService
    {
        Task<PlanetDto> GetById(Guid id);
        GetAllDto<PlanetDto> GetAll(int? page, int? pageSize);
        Task<bool> Delete(Guid id);
        Task<IEnumerable<PlanetDto>> GetByName(string name);
    }
}