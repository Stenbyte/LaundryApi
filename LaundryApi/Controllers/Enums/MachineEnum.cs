using System.Text.Json.Serialization;

namespace LaundryApi.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MachineStatus
{
    available,
    maintenance
}

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MachineName {
    washing,
    dryer
}

