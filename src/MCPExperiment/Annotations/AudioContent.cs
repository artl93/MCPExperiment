using System.Text.Json.Serialization;

namespace MCPExperiment.Annotations
{
    /// <summary>
    /// Represents audio content in the context.
    /// </summary>
    /// <remarks>
    /// Used for embedding audio directly in the context, typically as base64-encoded data.
    /// </remarks>
    public class AudioContent : Annotated, IContentItem
    {
        /// <summary>
        /// Gets or sets the type of the content. Always "audio" for audio content.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = "audio";

        /// <summary>
        /// Gets or sets the audio data as a string (typically base64-encoded).
        /// </summary>
        [JsonPropertyName("data")]
        public string Data { get; set; } = default!;

        /// <summary>
        /// Gets or sets the MIME type of the audio (e.g., "audio/mp3", "audio/wav").
        /// </summary>
        [JsonPropertyName("mimeType")]
        public string MimeType { get; set; } = default!;
    }
}