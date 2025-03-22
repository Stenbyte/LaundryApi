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

        [BsonElement("slots")]
        public List<BookingSlot> slots { get; set; } = new();

        [BsonElement("reservationsLeft")]
        public int reservationsLeft { get; set; } = 3;
    }
    public class BookingSlot
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        [BsonElement("day")]
        public required DateTime day { get; set; }

        [BsonElement("timeSlots")]
        public List<string> timeSlots { get; set; } = new();
    }
}