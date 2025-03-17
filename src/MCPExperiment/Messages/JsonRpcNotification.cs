using System.Text.Json.Serialization;

namespace MCPExperiment.Messages
{
    /// <summary>
    /// Represents a JSON-RPC notification which does not expect a response.
    /// </summary>
    /// <typeparam name="TParams">The type of the parameters.</typeparam>
    public class JsonRpcNotification<TParams> 
    {
        /// <summary>
        /// The JSON-RPC protocol version.
        /// </summary>
        public string JsonRpc { get; set; } = Constants.JsonRpcVersion;

        /// <summary>
        /// The method name to be invoked.
        /// </summary>
        [JsonPropertyName("method")]
        public string Method { get; set; } = default!;

        /// <summary>
        /// The parameters that are passed to the method.
        /// </summary>
        [JsonPropertyName("params")]
        public TParams Params { get; set; } = default!;
    }
}
