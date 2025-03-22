using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LaundryApi.Models
{
    public class Booking : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        public required string userId { get; set; } = string.Empty;
        [BsonElement("machineId")]
        public string? machineId { get; set; }

        [BsonElement("timeSlot")]
        public required string timeSlot { get; set; } = string.Empty;

        [BsonElement("day")]
        public required DateTime day { get; set; }

        [BsonElement("reservationsLeft")]
        public int reservationsLeft { get; set; } = 3;
    }
}
