using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace LaundryApi.Models
{
    public class User : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? id { get; set; }

        [BsonElement("firstName")]
        public required string firstName { get; set; }
        [BsonElement("lastName")]
        public required string lastName { get; set; }
        [BsonElement("email")]
        public required string email { get; set; }

        // [BsonElement("phoneNumber")]
        // public required PhoneNumber phoneNumber { get; set; }

        [BsonElement("password")]
        public required string password { get; set; }

        [BsonElement("adress")]
        public required Adress adress { get; set; }
        [BsonElement("dbName")]
        public string dbName { get; set; } = "";
        [BsonElement("refreshToken")]
        public string? refreshToken { get; set; }
        [BsonElement("refreshTokenExpiry")]
        public DateTime? refreshTokenExpiry { get; set; }
    }
}
