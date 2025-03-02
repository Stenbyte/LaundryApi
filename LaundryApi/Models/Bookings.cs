using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LaundryBooking.Models
{
    public class Booking : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("UserId")]
        public string UserId { get; set; } = null!;
        [BsonElement("MachineId")]
        public string MachineId { get; set; } = null!;

        [BsonElement("StartTime")]
        public DateTime StartTime { get; set; }

        [BsonElement("EndTime")]
        public DateTime EndTime { get; set; }
    }
}
