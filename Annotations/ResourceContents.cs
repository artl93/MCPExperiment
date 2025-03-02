using System.Text.Json.Serialization;

namespace Microsoft.Extensions.MCP.Annotations
{
    /// <summary>
    /// Base class for resource contents.
    /// </summary>
    /// <remarks>
    /// Provides common properties for all resource types. Can be extended with specific content types.
    /// </remarks>
    public class ResourceContents
    {
        /// <summary>
        /// Gets or sets the URI of the resource.
        /// </summary>
        /// <remarks>
        /// Can be a web URL or a local file path reference.
        /// </remarks>
        [JsonPropertyName("uri")]
        public string Uri { get; set; } = default!;

        /// <summary>
        /// Gets or sets the MIME type of the resource.
        /// </summary>
        /// <remarks>
        /// Describes the format of the resource (e.g., "application/pdf", "image/png").
        /// </remarks>
        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; } = default!;
    }
}
