using MongoDB.Bson.Serialization.Attributes;

namespace SWApi.Domain.Planet
{
    public class Film
    {
        [BsonElement("title")]
        public string Title { get; init; }

        [BsonElement("director")]
        public string Director { get; init; }

        [BsonElement("release_date")]
        public string ReleaseDate { get; init; }
    }
}