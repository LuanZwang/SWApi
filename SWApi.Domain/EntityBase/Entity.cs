using MongoDB.Bson.Serialization.Attributes;

namespace SWApi.Domain.EntityBase
{
    public abstract class Entity
    {
        [BsonElement("_id")]
        public string Id { get; private set; }

        protected Entity(string id)
        {
            Id = id;
        }
    }
}