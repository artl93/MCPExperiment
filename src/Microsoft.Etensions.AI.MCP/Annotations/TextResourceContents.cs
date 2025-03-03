using System.Text.Json.Serialization;

namespace Microsoft.Extensions.AI.MCP.Annotations
{
    /// <summary>
    /// Represents resource contents stored as text.
    /// </summary>
    /// <remarks>
    /// Used for embedding textual resources directly in the context.
    /// </remarks>
    public class TextResourceContents : ResourceContents
    {
        /// <summary>
        /// Gets or sets the text content of the resource.
        /// </summary>
        [JsonPropertyName("text")]
        public string Text { get; set; } = default!;
    }
}
