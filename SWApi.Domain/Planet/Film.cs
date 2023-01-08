using MongoDB.Bson.Serialization.Attributes;
using SWApi.Domain.EntityBase;

namespace SWApi.Domain.Planet;

public class Film
{
    [BsonElement("title")]
    public string Title { get; init; }

    [BsonElement("director")]
    public string Director { get; init; }

    [BsonElement("release_date")]
    public string ReleaseDate { get; init; }
}