using System.Text.Json.Serialization;

namespace MCP
{
    // Represents a root (a filesystem or context root) for scope limitation.
    public class Root
    {
        [JsonPropertyName("uri")] public string Uri { get; set; } = "";
        [JsonPropertyName("name")] public string? Name { get; set; }
    }
}