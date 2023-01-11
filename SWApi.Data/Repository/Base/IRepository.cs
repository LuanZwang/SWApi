using SWApi.Domain.EntityBase;

namespace SWApi.Data.Repository.Base
{
    public interface IRepository<T> where T : Entity
    {
        T GetById(string id);
        bool Remove(string id);
        List<Domain.Planet.Planet> GetByName(string name);
        public (long TotalCount, List<T> Items) GetAllPaginated(int? page, int? pageSize);
    }
}