using MongoDB.Driver;
using NLog;
using SWApi.Data.Connection.Interface;
using SWApi.Data.Repository.Interface;
using SWApi.Domain.Configuration.Logging;
using SWApi.Domain.Utils;

namespace SWApi.Data.Repository.Planet;

public sealed class PlanetRepository : IPlanetRepository
{
    public IMongoCollection<Domain.Planet.Planet> Collection { get; }
    private readonly Logger _logger;

    public PlanetRepository(
        ILogControl logger,
        IMongoConnection mongoConnection)
    {
        _logger = logger.GetLogger(GetType().Name);
        Collection = mongoConnection.GetCollection<Domain.Planet.Planet>("planets", CreateCollectionIndexes);
    }

    public void CreateCollectionIndexes(IMongoCollection<Domain.Planet.Planet> collection)
    {
        try
        {
            _logger.Info($"Creating index to {GetType().Name}");

            var index = new CreateIndexModel<Domain.Planet.Planet>(Builders<Domain.Planet.Planet>.IndexKeys.Ascending(x => x.Id));

            collection.Indexes.CreateOne(index);
        }
        catch (Exception ex)
        {
            _logger.Error($"Creating index to {GetType().Name} error: {ex.Message}");
        }
    }

    public Domain.Planet.Planet GetById(string id) =>
        Collection.AsQueryable().SingleOrDefault(x => x.Id == id);

    public bool Remove(string id)
    {
        var result = Collection.DeleteOne(x => x.Id == id);

        return result.DeletedCount == 1;
    }

    public Domain.Planet.Planet GetByName(string name)
    {
        var planets = Collection.AsQueryable().Where(x => x.Name == name).ToList();

        if (planets.Count > 1)
        {
            

            return null;
        }

        return planets.First();
    }

    public (List<Domain.Planet.Planet> Planets, long TotalCount) GetAllPaginated(int page = 1, int pageSize = 60)
    {
        var totalCount = Collection.CountDocuments(Builders<Domain.Planet.Planet>.Filter.Empty);

        var result = Collection.AsQueryable().Paginate(page, pageSize).ToList();

        return (result, totalCount);
    }
}