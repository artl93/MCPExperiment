using System.Text.Json.Serialization;

namespace Microsoft.Extensions.ModelContextProtocol.Resources
{
    /// <summary>
    /// A template description for resources available on the server.
    /// </summary>
    public class ResourceTemplate
    {
        /// <summary>
        /// Gets or sets a URI template (according to RFC 6570) that can be used to construct resource URIs.
        /// </summary>
        [JsonPropertyName("uriTemplate")]
        public string UriTemplate { get; set; } = default!;

        /// <summary>
        /// Gets or sets a human-readable name for the type of resource this template refers to.
        /// This can be used by clients to populate UI elements.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; } = default!;

        /// <summary>
        /// Gets or sets a description of what this template is for.
        /// This can be used by clients to improve the LLM's understanding of available resources.
        /// It can be thought of like a "hint" to the model.
        /// </summary>
        [JsonPropertyName("description")]
        public string Description { get; set; } = default!;

        /// <summary>
        /// Gets or sets the MIME type for all resources that match this template.
        /// This should only be included if all resources matching this template have the same type.
        /// </summary>
        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; } = default!;
    }
}
