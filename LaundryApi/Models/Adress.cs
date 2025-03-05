using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LaundryBooking.Models
{
    public class Adress
    {
        [BsonElement("streetName")]
        public required string countryCode { get; set; }

        [BsonElement("houseNumber")]
        public required string number { get; set; }

    }
}