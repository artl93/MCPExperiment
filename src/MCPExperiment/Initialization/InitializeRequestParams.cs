using System.Text.Json.Serialization;
using MCPExperiment.Capabilities;
using MCPExperiment.Messages;

namespace MCPExperiment.Initialization
{
    /// <summary>
    /// Parameters for the initialize request sent from client to server.
    /// </summary>
    public class InitializeRequestParams : IClientRequest
    {
        /// <summary>
        /// The latest version of the Model Context Protocol that the client supports.
        /// The client MAY decide to support older versions as well.
        /// </summary>
        [JsonPropertyName("protocolVersion")]
        public string ProtocolVersion { get; set; } = default!;

        /// <summary>
        /// Capabilities that the client supports.
        /// </summary>
        [JsonPropertyName("capabilities")]
        public ClientCapabilities Capabilities { get; set; } = default!;

        /// <summary>
        /// Information about the client implementation.
        /// </summary>
        [JsonPropertyName("clientInfo")]
        public Implementation ClientInfo { get; set; } = default!;
    }
}
