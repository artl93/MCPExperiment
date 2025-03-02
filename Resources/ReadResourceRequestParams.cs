using System.Text.Json.Serialization;

namespace Microsoft.Extensions.MCP.Resources
{
    /// <summary>
    /// Parameters for a request to read a specific resource URI.
    /// </summary>
    public class ReadResourceRequestParams
    {
        /// <summary>
        /// Gets or sets the URI of the resource to read. 
        /// The URI can use any protocol; it is up to the server how to interpret it.
        /// </summary>
        [JsonPropertyName("uri")]
        public string Uri { get; set; } = default!;
    }
}
