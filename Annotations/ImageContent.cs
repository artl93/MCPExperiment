using System.Text.Json.Serialization;

namespace Microsoft.Extensions.ModelContextProtocol.Annotations
{
    /// <summary>
    /// Represents image content in the context.
    /// </summary>
    /// <remarks>
    /// Used for embedding images directly in the context, typically as base64-encoded data.
    /// </remarks>
    public class ImageContent : Annotated
    {
        /// <summary>
        /// Gets or sets the type of the content. Always "image" for image content.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = "image";

        /// <summary>
        /// Gets or sets the image data as a string (typically base64-encoded).
        /// </summary>
        [JsonPropertyName("data")]
        public string Data { get; set; } = default!;

        /// <summary>
        /// Gets or sets the MIME type of the image (e.g., "image/png", "image/jpeg").
        /// </summary>
        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; } = default!;
    }
}
