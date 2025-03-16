using System.Text.Json.Serialization;

namespace Microsoft.MCP.Annotations
{
    /// <summary>
    /// Represents text content in the context.
    /// </summary>
    /// <remarks>
    /// The most common content type, used for normal text messages in the conversation.
    /// </remarks>
    public class TextContent : Annotated, IContentItem
    {
        /// <summary>
        /// Gets or sets the type of the content. Always "text" for text content.
        /// </summary>
        [JsonPropertyName("type")]
        public string Type { get; set; } = "text";

        /// <summary>
        /// Gets or sets the text content as a string.
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; } = default!;
    }
}
