using System.Collections.Generic;
using System.Text.Json.Serialization;
using MCPExperiment.Server.Models;

namespace MCPExperiment.Capabilities
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

        /// <summary>
        /// The list of tools available on the server.
        /// </summary>
        [JsonPropertyName("availableTools")]
        public List<ToolDefinition> AvailableTools { get; set; } = new List<ToolDefinition>();
    }
}
