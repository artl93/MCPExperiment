using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Microsoft.Extensions.ModelContextProtocol
{
    /// <summary>
    /// Base class for all result objects in the Model Context Protocol.
    /// </summary>
    /// <remarks>
    /// A Result object is returned in response to a request and may contain
    /// metadata and other properties specific to the particular request.
    /// </remarks>
    public class Result
    {
        /// <summary>
        /// Additional metadata attached to the response.
        /// </summary>
        /// <remarks>
        /// This property is reserved by the protocol to allow clients and servers
        /// to attach additional metadata to their responses.
        /// </remarks>
        [JsonPropertyName("_meta")]
        public Dictionary<string, object>? Meta { get; set; }
    }
}
