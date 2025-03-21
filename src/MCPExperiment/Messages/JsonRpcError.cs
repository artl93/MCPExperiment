using System.Text.Json.Serialization;
using MCPExperiment.Union;

namespace MCPExperiment.Messages
{
    /// <summary>
    /// Represents a JSON-RPC error response.
    /// </summary>
    public class JsonRpcError
    {
        public string JsonRpc { get; set; } = Constants.JsonRpcVersion;

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
        
        /// <summary>
        /// The error code property for compatibility with existing code.
        /// </summary>
        public int Code 
        { 
            get => Error?.Code ?? -1;
            set 
            {
                if (Error == null) 
                    Error = new JsonRpcErrorDetails();
                Error.Code = value;
            }
        }
        
        /// <summary>
        /// The error message property for compatibility with existing code.
        /// </summary>
        public string Message 
        { 
            get => Error?.Message ?? string.Empty;
            set 
            {
                if (Error == null) 
                    Error = new JsonRpcErrorDetails();
                Error.Message = value;
            }
        }
        
        /// <summary>
        /// The error data property for compatibility with existing code.
        /// </summary>
        public object Data 
        { 
            get => Error?.Data ?? default!;
            set 
            {
                if (Error == null) 
                    Error = new JsonRpcErrorDetails();
                Error.Data = value;
            }
        }
    }
}
