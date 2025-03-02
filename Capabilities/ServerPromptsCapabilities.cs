using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Extensions.AI.MCP.Server.Models;

namespace Microsoft.Extensions.AI.MCP.Capabilities
{
    /// <summary>
    /// Capabilities related to prompts that the server supports.
    /// </summary>
    public class ServerPromptsCapabilities
    {
        /// <summary>
        /// Gets or sets a value indicating whether this server supports notifications for changes to the prompt list.
        /// </summary>
        [JsonPropertyName("listChanged")]
        public bool? ListChanged { get; set; }

        /// <summary>
        /// The list of prompts available on the server.
        /// </summary>
        [JsonPropertyName("availablePrompts")]
        public List<PromptDefinition> AvailablePrompts { get; set; } = new List<PromptDefinition>();
    }
}
