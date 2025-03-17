using System.Text.Json.Serialization;

namespace MCPExperiment.Models
{
    /// <summary>
    /// Base class for all message types in the MCP protocol.
    /// </summary>
    public class BaseMessage
    {
        /// <summary>
        /// Gets or sets the role of the message sender (e.g., "user", "assistant").
        /// </summary>
        [JsonPropertyName("role")]
        public string Role { get; set; } = default!;
    }
}