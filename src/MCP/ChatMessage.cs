using System.Text.Json.Serialization;

namespace MCP
{
    // Represents a chat message (role + content), used in prompts or sampling.
    public class ChatMessage
    {
        [JsonPropertyName("role")] public string Role { get; set; } = "";
        [JsonPropertyName("content")] public ContentItem Content { get; set; } = new ContentItem();
    }
}