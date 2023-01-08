using MongoDB.Bson.Serialization.Attributes;
using SWApi.Domain.EntityBase;

namespace SWApi.Domain.Planet;

public class Planet : Entity
{
	[BsonElement("name")]
	public string Name { get; init; }

	[BsonElement("climate")]
	public string Climate { get; init; }

	[BsonElement("terrain")]
	public string Terrain { get; init; }

    [BsonElement("films")]
    public IEnumerable<Film> Films { get; init; }
}