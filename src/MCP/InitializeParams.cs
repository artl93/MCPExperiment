using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MCP
{
    // Parameters for the initialize request.
    public class InitializeParams
    {
        [JsonPropertyName("client")] public ImplementationInfo Client { get; set; } = new ImplementationInfo();
        [JsonPropertyName("capabilities")] public ClientCapabilities Capabilities { get; set; } = new ClientCapabilities();
        [JsonPropertyName("protocolVersion")] public string ProtocolVersion { get; set; } = "1.0";
        [JsonPropertyName("roots")] public List<Root>? Roots { get; set; }
    }
}