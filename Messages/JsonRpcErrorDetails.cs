using System.Text.Json.Serialization;

namespace Microsoft.Extensions.MCP.Messages
{
    /// <summary>
    /// Details about an error that occurred during a JSON-RPC request.
    /// </summary>
    public class JsonRpcErrorDetails
    {
        /// <summary>
        /// The error type that occurred.
        /// </summary>
        [JsonPropertyName("code")]
        public int Code { get; set; }

        /// <summary>
        /// A short description of the error.
        /// </summary>
        [JsonPropertyName("message")]
        public string Message { get; set; } = default!;

        /// <summary>
        /// Additional information about the error.
        /// </summary>
        [JsonPropertyName("data")]
        public object Data { get; set; } = default!;
    }
}
