using System.Text.Json.Serialization;
using Microsoft.Extensions.ModelContextProtocol.Union;

namespace Microsoft.Extensions.ModelContextProtocol.Messages
{
    /// <summary>
    /// Represents a JSON-RPC error response.
    /// </summary>
    public class JsonRpcError : JsonRpcMessage
    {
        /// <summary>
        /// The ID of the request this error response corresponds to.
        /// </summary>
        [JsonPropertyName("id")]
        public UnionValue Id { get; set; } = default!;

        /// <summary>
        /// Details about the error that occurred.
        /// </summary>
        [JsonPropertyName("error")]
        public JsonRpcErrorDetails Error { get; set; } = default!;
    }
}
