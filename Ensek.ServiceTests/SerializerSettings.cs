using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ensek.ServiceTests;

public static class SerializerSettings
{
    public static JsonSerializerOptions Default => new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) },
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };
}
