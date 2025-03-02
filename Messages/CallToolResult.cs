using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Extensions.AI.MCP.Annotations;

namespace Microsoft.Extensions.AI.MCP.Messages
{
    /// <summary>
    /// The server's response to a tool call.
    /// </summary>
    /// <remarks>
    /// Any errors that originate from the tool SHOULD be reported inside the result
    /// object, with `isError` set to true, _not_ as an MCP protocol-level error
    /// response. Otherwise, the LLM would not be able to see that an error occurred
    /// and self-correct.
    /// </remarks>
    public class CallToolResult : IServerResult
    {
        /// <summary>
        /// Gets or sets the content returned from the tool call.
        /// </summary>
        /// <remarks>
        /// Can include TextContent, ImageContent, AudioContent, or EmbeddedResource objects.
        /// </remarks>
        [JsonPropertyName("content")]
        public List<object> Content { get; set; } = new List<object>();

        /// <summary>
        /// Gets or sets a value indicating whether the tool call ended in an error.
        /// </summary>
        /// <remarks>
        /// If not set, this is assumed to be false (the call was successful).
        /// </remarks>
        [JsonPropertyName("isError")]
        public bool? IsError { get; set; }
    }
}