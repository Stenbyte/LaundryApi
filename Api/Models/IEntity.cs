using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TenantApi.Models
{
    public interface IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        string? _id { get; set; }
    }
}