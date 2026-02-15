using TenantApi.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TenantApi.Models
{
    public class MachineModel : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }

        public MachineStatus status { get; set; }

        public MachineName name { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public required string buildingId { get; set; }
    }
}