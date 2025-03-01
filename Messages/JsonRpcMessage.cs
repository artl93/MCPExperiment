using System.Text.Json.Serialization;

namespace Microsoft.Extensions.ModelContextProtocol.Messages
{
    /// <summary>
    /// Base class for all JSON-RPC messages.
    /// </summary>
    public abstract class JsonRpcMessage
    {
        /// <summary>
        /// The JSON-RPC protocol version.
        /// </summary>
        [JsonPropertyName("jsonrpc")]
        public string JsonRpc { get; set; } = Constants.JsonRpcVersion;
    }
}
