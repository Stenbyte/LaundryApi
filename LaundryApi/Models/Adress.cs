using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LaundryApi.Models
{
    public class Adress
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? streetNameId { get; set; }
        [BsonElement("streetName")]
        public required string streetName { get; set; }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? buildingId { get; set; }
        [BsonElement("housebuildingNumberNumber")]
        public required string buildingNumber { get; set; }

    }
}