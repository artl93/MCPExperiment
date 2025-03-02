using System.Text.Json.Serialization;

namespace MCP
{
    // Info about a tool for listing.
    public class ToolInfo
    {
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        [JsonPropertyName("description")] public string? Description { get; set; }
        // (Parameter schema could be included if needed)
    }
}