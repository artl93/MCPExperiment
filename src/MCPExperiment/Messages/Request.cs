using System.Text.Json.Serialization;

namespace MCPExperiment.Messages
{
    /// <summary>
    /// Base class for all request messages.
    /// </summary>
    /// <typeparam name="TParams">The type of the parameters.</typeparam>
    public abstract class Request<TParams> 
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
