using System.Text.Json.Serialization;
using Microsoft.Extensions.AI.MCP.Capabilities;
using Microsoft.Extensions.AI.MCP.Messages;

namespace Microsoft.Extensions.AI.MCP.Initialization
{
    /// <summary>
    /// After receiving an initialize request from the client, the server sends this response.
    /// </summary>
    public class InitializeResult : Result, IServerResult
    {
        /// <summary>
        /// The version of the Model Context Protocol that the server wants to use.
        /// This may not match the version that the client requested.
        /// If the client cannot support this version, it MUST disconnect.
        /// </summary>
        [JsonPropertyName("protocolVersion")]
        public string ProtocolVersion { get; set; } = default!;

        /// <summary>
        /// Capabilities that the server supports.
        /// </summary>
        [JsonPropertyName("capabilities")]
        public ServerCapabilities Capabilities { get; set; } = default!;

        /// <summary>
        /// For backward compatibility with existing implementation.
        /// </summary>
        public ServerCapabilities ServerCapabilities 
        { 
            get => Capabilities; 
            set => Capabilities = value;
        }

        /// <summary>
        /// Information about the server implementation.
        /// </summary>
        [JsonPropertyName("serverInfo")]
        public Implementation ServerInfo { get; set; } = default!;

        /// <summary>
        /// Instructions describing how to use the server and its features.
        /// This can be used by clients to improve the LLM's understanding of available tools, resources, etc.
        /// It can be thought of like a "hint" to the model and may be added to the system prompt.
        /// </summary>
        [JsonPropertyName("instructions")]
        public string Instructions { get; set; } = default!;
    }
}
