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
        public string userId { get; set; } = null!;
        [BsonElement("machineId")]
        public string? machineId { get; set; }

        [BsonElement("timeSlot")]
        public DateTime timeSlot { get; set; }
    }
}
