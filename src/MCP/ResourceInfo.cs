using System.Text.Json.Serialization;

namespace MCP
{
    // Info about a resource for listing.
    public class ResourceInfo
    {
        [JsonPropertyName("uri")] public string Uri { get; set; } = "";
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("description")] public string? Description { get; set; }
        [JsonPropertyName("mimeType")] public string? MimeType { get; set; }
    }
}