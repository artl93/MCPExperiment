using System.Text.Json.Serialization;

namespace MCP
{
    // Capabilities a server can advertise.
    public class ServerCapabilities
    {
        public class ResourcesCaps 
        { 
            [JsonPropertyName("subscribe")] public bool Subscribe { get; set; }
            [JsonPropertyName("listChanged")] public bool ListChanged { get; set; }
        }
        [JsonPropertyName("resources")] public ResourcesCaps? Resources { get; set; }
        [JsonPropertyName("tools")] public object? Tools { get; set; }
        [JsonPropertyName("prompts")] public object? Prompts { get; set; }
        [JsonPropertyName("sampling")] public object? Sampling { get; set; }
    }
}