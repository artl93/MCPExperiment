using System.Text.Json.Serialization;

namespace MCP
{
    // Result of the initialize request.
    public class InitializeResult
    {
        [JsonPropertyName("server")] public ImplementationInfo Server { get; set; } = new ImplementationInfo();
        [JsonPropertyName("capabilities")] public ServerCapabilities Capabilities { get; set; } = new ServerCapabilities();
        [JsonPropertyName("protocolVersion")] public string ProtocolVersion { get; set; } = "1.0";
    }
}