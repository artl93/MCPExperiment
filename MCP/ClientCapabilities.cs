using System.Text.Json.Serialization;

namespace MCP
{
    // Capabilities a client can advertise (for brevity, a few key flags).
    public class ClientCapabilities
    {
        [JsonPropertyName("sampling")] public bool? SupportsSampling { get; set; }
        [JsonPropertyName("roots")] public bool? SupportsRoots { get; set; }
        // (Could include more, e.g. whether client supports certain notifications)
    }
}