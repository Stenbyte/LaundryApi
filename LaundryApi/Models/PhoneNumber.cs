using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LaundryApi.Models
{
    public class PhoneNumber
    {
        [BsonElement("countryCode")]
        public required string countryCode { get; set; }

        [BsonElement("number")]
        public required string number { get; set; }

    }
}