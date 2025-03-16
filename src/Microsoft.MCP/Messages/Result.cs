using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Microsoft.MCP.Messages
{
    /// <summary>
    /// Base class for all result types returned from requests.
    /// </summary>
    public class Result
    {
        /// <summary>
        /// Additional metadata attached to the response.
        /// This property is reserved by the protocol to allow clients and servers 
        /// to attach additional metadata to their responses.
        /// </summary>
        [JsonPropertyName("_meta")]
        public Dictionary<string, object> Meta { get; set; } = default!;
    }
}
