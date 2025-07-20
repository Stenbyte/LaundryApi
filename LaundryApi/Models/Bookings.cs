using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LaundryApi.Models
{
    public class Booking : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? id { get; set; }

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string userId { get; set; } = string.Empty;
        [BsonElement("machineId")]
        public string? machineId { get; set; }
        [BsonElement("startTime")]
        public DateTime? startTime { get; set; }
        public DateTime? endTime { get; set; }
        public bool booked { get; set; } = false;

        [BsonElement("slots")]
        public List<BookingSlot> slots { get; set; } = new();
        [BsonElement("buildingId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? buildingId { get; set; }

        [BsonElement("reservationsLeft")]
        public int reservationsLeft { get; set; } = 3;
        public void ConvertToUtc()
        {
            if (startTime.HasValue)
                startTime = DateTime.SpecifyKind(startTime.Value, DateTimeKind.Utc);
            if (endTime.HasValue)
                endTime = DateTime.SpecifyKind(endTime.Value, DateTimeKind.Utc);
        }
    }
    public class BookingSlot
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string? id { get; set; }
        [BsonElement("day")]
        public required DateTime day { get; set; }
        public bool booked { get; set; } = false;
        public void ConvertToUtc()
        {
            day = DateTime.SpecifyKind(day, DateTimeKind.Utc);
        }

        [BsonElement("timeSlots")]
        public List<string> timeSlots { get; set; } = new();
    }
    public class EditBookingRequest
    {
        public string id { get; set; } = null!;
    }
}