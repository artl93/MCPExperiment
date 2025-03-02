using System.Text.Json.Serialization;

namespace Microsoft.Extensions.MCP.Capabilities
{
    /// <summary>
    /// Capabilities related to tools that the server supports.
    /// </summary>
    public class ServerToolsCapabilities
    {
        /// <summary>
        /// Gets or sets a value indicating whether this server supports notifications for changes to the tool list.
        /// </summary>
        [JsonPropertyName("listChanged")]
        public bool? ListChanged { get; set; }
    }
}
