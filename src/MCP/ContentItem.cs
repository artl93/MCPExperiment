using System.Text.Json.Serialization;

namespace MCP
{
    // Represents a piece of content (text or binary) exchanged via MCP.
    public class ContentItem
    {
        [JsonPropertyName("uri")] public string? Uri { get; set; }
        [JsonPropertyName("text")] public string? Text { get; set; }
        [JsonPropertyName("data")] public byte[]? Data { get; set; }
        [JsonPropertyName("mimeType")] public string? MimeType { get; set; }
        [JsonPropertyName("type")] public string? Type { get; set; }
    }
}