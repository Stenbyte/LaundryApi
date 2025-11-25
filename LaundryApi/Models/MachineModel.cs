using LaundryApi.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LaundryApi.Models
{
    public class MachineModel : IEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? _id { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MachineStatus status { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public MachineName name { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public required string buildingId { get; set; }
    }
}