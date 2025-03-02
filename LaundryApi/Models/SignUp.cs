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

        [BsonElement("phone")]
        public required string phone { get; set; }

        [BsonElement("password")]
        public required string password { get; set; }
        [BsonElement("streetAdress")]
        public required string streetadress { get; set; }
        [BsonElement("adressNumber")]
        public required string aadressNumber { get; set; }
    }
}
