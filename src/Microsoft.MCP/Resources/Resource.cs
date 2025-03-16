using System.Text.Json.Serialization;

namespace Microsoft.MCP.Resources
{
    /// <summary>
    /// A known resource that the server is capable of reading.
    /// </summary>
    public class Resource
    {
        /// <summary>
        /// Gets or sets the URI of this resource.
        /// </summary>
        [JsonPropertyName("uri")]
        public string Uri { get; set; } = default!;

        /// <summary>
        /// Gets or sets a human-readable name for this resource.
        /// This can be used by clients to populate UI elements.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets a description of what this resource represents.
        /// This can be used by clients to improve the LLM's understanding of available resources.
        /// It can be thought of like a "hint" to the model.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = default!;

        /// <summary>
        /// Gets or sets the MIME type of this resource, if known.
        /// </summary>
        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; } = default!;

        /// <summary>
        /// Gets or sets the size of the raw resource content, in bytes (i.e., before base64 encoding or any tokenization), if known.
        /// This can be used by Hosts to display file sizes and estimate context window usage.
        /// </summary>
        [JsonPropertyName("size")]
        public long? Size { get; set; }
    }
}
