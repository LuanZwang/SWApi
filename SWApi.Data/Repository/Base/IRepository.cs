using SWApi.Domain.EntityBase;

namespace SWApi.Data.Repository.Base
{
    public interface IRepository<T> where T : Entity
    {
        Task<T> GetById(string id);
        Task<bool> Remove(string id);
        Task<List<Domain.Planet.Planet>> GetByName(string name);
        (long TotalCount, List<T> Items) GetAllPaginated(int? page, int? pageSize);
    }
}