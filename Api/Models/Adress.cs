using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TenantApi.Models
{
    public class Adress
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }
        [BsonElement("streetName")]
        public required string streetName { get; set; }

        [BsonElement("buildingNumber")]
        public required string buildingNumber { get; set; }

    }
}