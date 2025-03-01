using System.Text.Json.Serialization;

namespace Microsoft.Extensions.ModelContextProtocol.Annotations
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum Role
    {
        User,
        Assistant
    }
}
