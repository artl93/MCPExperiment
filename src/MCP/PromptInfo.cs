using System.Text.Json.Serialization;

namespace MCP
{
    // Info about a prompt for listing.
    public class PromptInfo
    {
        [JsonPropertyName("name")] public string Name { get; set; } = "";
        [JsonPropertyName("description")] public string? Description { get; set; }
    }
}