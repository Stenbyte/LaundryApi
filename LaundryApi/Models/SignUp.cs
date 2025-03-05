using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LaundryBooking.Models
{
    public class SignUp : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("firstName")]
        public required string firstName { get; set; }
        [BsonElement("lastName")]
        public required string lastName { get; set; }

        [BsonElement("phoneNumber")]
        public required PhoneNumber phoneNumber { get; set; }

        [BsonElement("password")]
        public required string password { get; set; }

        [BsonElement("adress")]
        public required Adress adress { get; set; }
    }
}
