using System.Text.Json.Serialization;
using Microsoft.Extensions.AI.MCP.Union;

namespace Microsoft.Extensions.AI.MCP.Messages
{
    /// <summary>
    /// Represents a successful (non-error) response to a JSON-RPC request.
    /// </summary>
    /// <typeparam name="T">The type of the result.</typeparam>
    public class JsonRpcResponse<T> : JsonRpcMessage
    {
        /// <summary>
        /// The ID of the request this response corresponds to.
        /// </summary>
        [JsonPropertyName("id")]
        public UnionValue Id { get; set; } = default!;

        /// <summary>
        /// The result of the request.
        /// </summary>
        [JsonPropertyName("result")]
        public T Result { get; set; } = default!;
        
        /// <summary>
        /// Error property for compatibility with existing code.
        /// In an actual JSON-RPC response, either result or error exists, not both.
        /// This property is only for compatibility and is not serialized.
        /// </summary>
        [JsonIgnore]
        public JsonRpcError? Error { get; set; }
    }
}
