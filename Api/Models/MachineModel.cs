using TenantApi.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace TenantApi.Models
{
    public class MachineModel : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MachineStatus status { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public MachineName name { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public required string buildingId { get; set; }
    }
}