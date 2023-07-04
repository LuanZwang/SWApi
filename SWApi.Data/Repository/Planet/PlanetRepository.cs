using MongoDB.Driver;
using MongoDB.Driver.Linq;
using NLog;
using SWApi.Data.Connection.Interface;
using SWApi.Data.Repository.Interface;
using SWApi.Domain.Configuration.Logging;
using SWApi.Domain.Utils;

namespace SWApi.Data.Repository.Planet
{
    public class PlanetRepository : IPlanetRepository
    {
        public IMongoCollection<Domain.Planet.Planet> Collection { get; }
        private readonly ILogger _logger;

        public PlanetRepository(
            ILogControl logger,
            IMongoConnection mongoConnection,
            IMongoCollection<Domain.Planet.Planet> mongoCollection = default)
        {
            _logger = logger.GetLogger(GetType().Name);
            Collection = mongoCollection ?? mongoConnection.GetCollection<Domain.Planet.Planet>("planets", CreateCollectionIndexes);
        }

        public virtual void CreateCollectionIndexes(IMongoCollection<Domain.Planet.Planet> collection)
        {
            try
            {
                _logger.Info($"Creating index to {GetType().Name}");

                var index = new CreateIndexModel<Domain.Planet.Planet>(
                    keys: Builders<Domain.Planet.Planet>.IndexKeys.Ascending(x => x.Id),
                    options: new CreateIndexOptions
                    {
                        Collation = new Collation(locale: "en", caseFirst: CollationCaseFirst.Off, strength: CollationStrength.Secondary)
                    });

                collection.Indexes.CreateOne(index);
            }
            catch (Exception ex)
            {
                _logger.Error($"Creating index to {GetType().Name} error: {ex.Message}");
            }
        }

        public async Task<Domain.Planet.Planet> GetById(string id) =>
            await Collection.AsQueryable().SingleOrDefaultAsync(x => x.Id == id);

        public async Task<bool> Remove(string id)
        {
            var result = await Collection.DeleteOneAsync(x => x.Id == id);

            return result.DeletedCount == 1;
        }

        public async Task<List<Domain.Planet.Planet>> GetByName(string name) =>
            await Collection.Find(filter: Builders<Domain.Planet.Planet>.Filter.Where(x => x.Name == name)).ToListAsync();

        public (long TotalCount, List<Domain.Planet.Planet> Items) GetAllPaginated(int? page, int? pageSize)
        {
            var totalCount = Collection.CountDocuments(Builders<Domain.Planet.Planet>.Filter.Empty);

            var result = Collection.AsQueryable().Paginate(page, pageSize).ToList();

            return (totalCount, result);
        }
    }
}