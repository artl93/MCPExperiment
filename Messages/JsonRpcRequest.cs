using System.Text.Json.Serialization;
using Microsoft.Extensions.MCP.Union;

namespace Microsoft.Extensions.MCP.Messages
{
    /// <summary>
    /// Represents a JSON-RPC request that expects a response.
    /// </summary>
    /// <typeparam name="TParams">The type of the parameters.</typeparam>
    public class JsonRpcRequest<TParams> : Request<TParams>
    {
        /// <summary>
        /// A uniquely identifying ID for the request.
        /// Can be a string or number.
        /// </summary>
        [JsonPropertyName("id")]
        public UnionValue Id { get; set; } = default!;
    }
}
