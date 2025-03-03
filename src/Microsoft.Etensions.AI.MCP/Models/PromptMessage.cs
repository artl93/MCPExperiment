using System.Text.Json.Serialization;
using Microsoft.Extensions.AI.MCP.Annotations;

namespace Microsoft.Extensions.AI.MCP.Models
{
    /// <summary>
    /// Describes a message returned as part of a prompt.
    /// </summary>
    /// <remarks>
    /// This is similar to SamplingMessage, but also supports the embedding of resources from the MCP server.
    /// </remarks>
    public class PromptMessage
    {
        /// <summary>
        /// Gets or sets the role (user or assistant) associated with this message.
        /// </summary>
        [JsonPropertyName("role")]
        public string Role { get; set; } = default!;

        /// <summary>
        /// Gets or sets the content of the message.
        /// </summary>
        /// <remarks>
        /// This can be TextContent, ImageContent, AudioContent, or EmbeddedResource.
        /// </remarks>
        [JsonPropertyName("content")]
        public object Content { get; set; } = default!;
    }
}