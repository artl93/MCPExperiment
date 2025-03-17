using System.Text.Json.Serialization;

namespace MCPExperiment.Annotations
{
    /// <summary>
    /// Represents resource contents stored as a binary blob.
    /// </summary>
    /// <remarks>
    /// Used for embedding binary data directly in the resource. The blob is typically a base64-encoded string.
    /// </remarks>
    public class BlobResourceContents : ResourceContents
    {
        /// <summary>
        /// Gets or sets the binary data of the resource as a string (typically base64-encoded).
        /// </summary>
        [JsonPropertyName("blob")]
        public string Blob { get; set; } = default!;
    }
}
