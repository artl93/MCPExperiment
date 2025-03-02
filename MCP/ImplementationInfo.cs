using System.Text.Json.Serialization;

namespace MCP
{
    // Basic implementation metadata (name and version).
    public class ImplementationInfo
    {
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        [JsonPropertyName("version")] public string Version { get; set; } = "";
    }
}