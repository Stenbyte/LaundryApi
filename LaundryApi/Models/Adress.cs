using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LaundryBooking.Models
{
    public class Adress
    {
        [BsonElement("streetName")]
        public required string streetName { get; set; }

        [BsonElement("houseNumber")]
        public required string houseNumber { get; set; }

    }
}