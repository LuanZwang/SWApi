using SWApi.Domain.EntityBase;

namespace SWApi.Data.Repository.Base;

public interface IRepository<T> where T : Entity
{
    T GetById(string id);
    bool Remove(string id);
    Domain.Planet.Planet GetByName(string name);
    public (List<Domain.Planet.Planet> Planets, long TotalCount) GetAllPaginated(int page = 1, int pageSize = 60);
}